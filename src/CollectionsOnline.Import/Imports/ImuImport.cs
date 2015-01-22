using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Core.Utilities;
using CollectionsOnline.Import.Extensions;
using CollectionsOnline.Import.Factories;
using IMu;
using NLog;
using Raven.Abstractions.Extensions;
using Raven.Client;

namespace CollectionsOnline.Import.Imports
{
    public class ImuImport<T> : IImport where T : EmuAggregateRoot
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly IDocumentStore _documentStore;
        private readonly Session _session;
        private readonly IEmuAggregateRootFactory<T> _imuFactory;

        public ImuImport(
            IDocumentStore documentStore,
            Session session,
            IEmuAggregateRootFactory<T> imuFactory)
        {
            _documentStore = documentStore;
            _session = session;
            _imuFactory = imuFactory;
        }

        public void Run()
        {
            var module = new Module(_imuFactory.ModuleName, _session);
            ImportCache importCache;

            // Check for existing import in case we need to resume.
            using (var documentSession = _documentStore.OpenSession())
            {
                var importStatus = documentSession.Load<Application>(Constants.ApplicationId).GetImportStatus(GetType().ToString());

                // Exit current import if it had completed previous time it was run.
                if (importStatus.IsFinished)
                {
                    return;
                }

                _log.Debug("Starting {0} import", typeof (T).Name);

                // Check for existing cached results
                importCache = documentSession.Load<ImportCache>(string.Format("importCaches/{0}", typeof(T).Name.ToLower()));
                if (importCache == null)
                {
                    importCache = new ImportCache { Id = string.Format("importCaches/{0}", typeof(T).Name.ToLower()) };

                    var terms = _imuFactory.Terms;

                    if (importStatus.PreviousDateRun.HasValue)
                        terms.Add("AdmDateModified", importStatus.PreviousDateRun.Value.ToString("MMM dd yyyy"), ">=");

                    var hits = module.FindTerms(terms);

                    _log.Debug("Caching {0} search results. {1} Hits", typeof(T).Name, hits);

                    var cachedCurrentOffset = 0;
                    while (true)
                    {
                        if (ImportCanceled())
                            return;

                        var results = module.Fetch("start", cachedCurrentOffset, Constants.CachedDataBatchSize, new[] { "irn" });

                        if (results.Count == 0)
                            break;

                        var irnResults = results.Rows.Select(x => long.Parse(x.GetEncodedString("irn"))).ToList();

                        // First check to see if we are not overwriting existing data, 
                        // then find any existing documents and remove them from our cached irn list so we only add new results
                        if (!bool.Parse(ConfigurationManager.AppSettings["OverwriteExistingDocuments"]))
                        {
                            var existingIrnResults = new List<long>();
                            using (var subDocumentSession = _documentStore.OpenSession())
                            {
                                existingIrnResults.AddRange(
                                    subDocumentSession.Load<T>(
                                        irnResults.Select(
                                            x =>
                                                string.Format("{0}/{1}", Inflector.Pluralize(typeof(T).Name).ToLower(),
                                                    x)))
                                        .Where(x => x != null)
                                        .Select(x => long.Parse(x.Id.Substring(x.Id.IndexOf('/') + 1))));
                            }
                            irnResults = irnResults.Except(existingIrnResults).ToList();
                        }

                        importCache.Irns.AddRange(irnResults);

                        cachedCurrentOffset += results.Count;

                        _log.Debug("{0} cache progress... {1}/{2}", typeof(T).Name, cachedCurrentOffset, hits);
                    }

                    // Store cached result
                    documentSession.Store(importCache);
                    documentSession.SaveChanges();

                    _log.Debug("Caching of {0} search results complete, beginning import.", typeof(T).Name);
                }
                else
                {
                    _log.Debug("Cached search results found, resuming {0} import.", typeof(T).Name);
                }
            }
            
            // Perform import
            while (true)
            {
                using (var documentSession = _documentStore.OpenSession())
                {
                    if (ImportCanceled())
                        return;

                    var importStatus = documentSession.Load<Application>(Constants.ApplicationId).GetImportStatus(GetType().ToString());

                    var cachedResultBatch = importCache.Irns
                        .Skip(importStatus.CurrentImportCacheOffset)
                        .Take(Constants.DataBatchSize)
                        .ToList();
                    
                    if (cachedResultBatch.Count == 0)
                        break;

                    var stopwatch = Stopwatch.StartNew();

                    module.FindKeys(cachedResultBatch);

                    var results = module.Fetch("start", 0, -1, _imuFactory.Columns);

                    stopwatch.Stop();
                    _log.Trace("Found and fetched {0} {1} records in {2} ms",cachedResultBatch.Count, typeof(T).Name, stopwatch.ElapsedMilliseconds);

                    // If import has run before, update existing documents, otherwise simply store new documents.
                    stopwatch = Stopwatch.StartNew();
                    if (importStatus.PreviousDateRun.HasValue)
                    {
                        var newDocuments = results.Rows.Select(_imuFactory.MakeDocument).ToList();
                        var existingDocuments = documentSession.Load<T>(newDocuments.Select(x => x.Id));

                        for (var i = 0; i < newDocuments.Count; i++)
                        {
                            if (existingDocuments[i] != null)
                            {
                                // Update existing
                                _imuFactory.UpdateDocument(newDocuments[i], existingDocuments[i], documentSession);
                            }
                            else
                            {
                                // Create new
                                documentSession.Store(newDocuments[i]);
                            }
                        }
                    }
                    else
                    {
                        results.Rows
                            .Select(_imuFactory.MakeDocument)
                            .ToList()
                            .ForEach(documentSession.Store);
                    }
                    stopwatch.Stop();
                    _log.Trace("Updated/Created {0} {1} records in {2} ms", cachedResultBatch.Count, typeof(T).Name, stopwatch.ElapsedMilliseconds);

                    importStatus.CurrentImportCacheOffset += results.Count;
                    
                    documentSession.SaveChanges();

                    _log.Debug("{0} import progress... {1}/{2}", typeof(T).Name, importStatus.CurrentImportCacheOffset, importCache.Irns.Count);
                }
            }

            _log.Debug("{0} import complete", typeof(T).Name);

            using (var documentSession = _documentStore.OpenSession())
            {
                // Mark import status as finished
                documentSession.Load<Application>(Constants.ApplicationId).ImportFinished(GetType().ToString(), importCache.DateCreated);
                documentSession.SaveChanges();
            }
        }

        public int Order
        {
            get { return 100; }
        }

        private bool ImportCanceled()
        {
            if (DateTime.Now.TimeOfDay > Constants.ImuOfflineTimeSpanStart && DateTime.Now.TimeOfDay < Constants.ImuOfflineTimeSpanEnd)
            {
                _log.Warn("Imu about to go offline, canceling all imports");
                Program.ImportCanceled = true;
            }

            return Program.ImportCanceled;
        }        
    }
}