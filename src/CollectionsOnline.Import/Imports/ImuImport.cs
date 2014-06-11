using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Models;
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
            
            // Check for existing import in case we need to resume.
            using (var documentSession = _documentStore.OpenSession())
            {
                var importStatus = documentSession.Load<Application>(Constants.ApplicationId).GetImportStatus(GetType().ToString());

                // Exit current import if it had completed previous time it was run.
                if (importStatus.IsFinished)
                {
                    return;
                }

                _log.Debug("Starting {0} import", typeof(T).Name);

                // Cache the search results
                if (importStatus.CachedResult == null)
                {
                    var terms = _imuFactory.Terms;
                    importStatus.CachedResult = new List<long>();

                    if (importStatus.PreviousDateRun.HasValue)
                        terms.Add("AdmDateModified", importStatus.PreviousDateRun.Value.ToString("MMM dd yyyy"), ">=");

                    importStatus.CachedResultDate = DateTime.Now;

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

                        importStatus.CachedResult.AddRange(results.Rows.Select(x => long.Parse(x.GetString("irn"))));

                        cachedCurrentOffset += results.Count;

                        _log.Debug("{0} cache progress... {1}/{2}", typeof(T).Name, cachedCurrentOffset, hits);
                    }

                    // Store cached result
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

                    var cachedResultBatch = importStatus.CachedResult
                        .Skip(importStatus.CurrentOffset)
                        .Take(Constants.DataBatchSize)
                        .ToList();

                    if (cachedResultBatch.Count == 0)
                        break;

                    module.FindKeys(cachedResultBatch);

                    var results = module.Fetch("start", 0, -1, _imuFactory.Columns);

                    // If import has run before, update existing documents, otherwise simply store new documents.
                    if (importStatus.PreviousDateRun.HasValue)
                    {
                        var newDocuments = results.Rows.Select(_imuFactory.MakeDocument).ToList();
                        var existingDocuments = documentSession.Load<T>(newDocuments.Select(x => x.Id));

                        for (var i = 0; i < newDocuments.Count; i++)
                        {
                            if (existingDocuments[i] != null)
                            {
                                // Update existing
                                Mapper.Map(newDocuments[i], existingDocuments[i]);
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

                    importStatus.CurrentOffset += results.Count;

                    _log.Debug("{0} import progress... {1}/{2}", typeof(T).Name, importStatus.CurrentOffset, importStatus.CachedResult.Count);
                    documentSession.SaveChanges();
                }
            }

            using (var documentSession = _documentStore.OpenSession())
            {
                documentSession.Load<Application>(Constants.ApplicationId).ImportFinished(GetType().ToString());
                documentSession.SaveChanges();
            }
        }

        private bool ImportCanceled()
        {
            if (DateTime.Now.TimeOfDay > Constants.ImuOfflineTimeSpan)
            {
                _log.Warn("Imu about to go offline, canceling all imports");
                Program.ImportCanceled = true;
            }

            return Program.ImportCanceled;
        }
    }
}