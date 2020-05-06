using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Core.Utilities;
using CollectionsOnline.Import.Extensions;
using CollectionsOnline.Import.Factories;
using CollectionsOnline.Import.Infrastructure;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Serilog;

namespace CollectionsOnline.Import.Imports
{
    public class ImuImport<T> : ImuImportBase where T : EmuAggregateRoot
    {        
        private readonly IDocumentStore _documentStore;
        private readonly IImuSessionProvider _imuSessionProvider;
        private readonly IEmuAggregateRootFactory<T> _imuFactory;

        public ImuImport(
            IDocumentStore documentStore,
            IImuSessionProvider imuSessionProvider,
            IEmuAggregateRootFactory<T> imuFactory)
        {
            _documentStore = documentStore;
            _imuSessionProvider = imuSessionProvider;
            _imuFactory = imuFactory;
        }

        public override void Run()
        {
            using (Log.Logger.BeginTimedOperation($"{typeof(T).Name} Imu Import starting", "ImuImport<T>.Run"))
            { 
                ImportCache importCache;            

                // Check for existing import in case we need to resume.
                using (var documentSession = _documentStore.OpenSession())
                using (var imuSession = _imuSessionProvider.CreateInstance(_imuFactory.ModuleName))
                {
                    var importStatus = documentSession.Load<Application>(Constants.ApplicationId).GetImportStatus(GetType().ToString());

                    // Exit current import if it had completed previous time it was run.
                    if (importStatus.IsFinished)
                    {
                        Log.Logger.Information("Import finished last time... skipping");
                        return;
                    }

                    // Check for existing cached results
                    importCache = documentSession.Load<ImportCache>($"importCaches/{typeof(T).Name.ToLower()}");
                    if (importCache == null)
                    {
                        Log.Logger.Information("Caching {Name} results", typeof(T).Name);
                        importCache = new ImportCache { Id = $"importCaches/{typeof(T).Name.ToLower()}"};

                        var terms = _imuFactory.Terms;
                        if (importStatus.PreviousDateRun.HasValue)
                            terms.Add("AdmDateModified", importStatus.PreviousDateRun.Value.ToString("MMM dd yyyy"), ">=");

                        var hits = imuSession.FindTerms(terms);

                        if (importStatus.PreviousDateRun.HasValue)
                            Log.Logger.Information("Found {Hits} records where {@Terms}", hits, terms.List);

                        var cachedCurrentOffset = 0;
                        while (true)
                        {
                            if (ImportCanceled())
                                return;

                            var results = imuSession.Fetch("start", cachedCurrentOffset, Constants.CachedDataBatchSize, new[] { "irn" });

                            if (results.Count == 0)
                                break;

                            var irnResults = results.Rows.Select(x => long.Parse(x.GetEncodedString("irn"))).ToList();

                            // Check to see if we are skipping existing data, used for testing and debugging purposes only
                            if (bool.Parse(ConfigurationManager.AppSettings["SkipExistingDocuments"]))
                            {
                                var existingIrnResults = new List<long>();
                                using (var subDocumentSession = _documentStore.OpenSession())
                                {
                                    existingIrnResults.AddRange(
                                        subDocumentSession.Load<T>(
                                            irnResults.Select(
                                                x =>
                                                    $"{Inflector.Pluralize(typeof(T).Name).ToLower()}/{x}"))
                                            .Where(x => x != null)
                                            .Select(x => long.Parse(x.Id.Substring(x.Id.IndexOf('/') + 1))));
                                }
                                irnResults = irnResults.Except(existingIrnResults).ToList();
                            }

                            importCache.Irns.AddRange(irnResults);

                            cachedCurrentOffset += results.Count;

                            Log.Logger.Information("{Name} cache progress... {Offset}/{TotalResults}", typeof(T).Name, cachedCurrentOffset, hits);
                        }

                        // Store cached result
                        documentSession.Store(importCache);
                        documentSession.SaveChanges();

                        Log.Logger.Information("Caching of {Name} search results complete", typeof(T).Name);
                    }
                    else
                    {
                        Log.Logger.Information("Cached {Name} results found... resuming", typeof(T).Name);
                    }
                }
            
                // Perform import
                while (true)
                {
                    using (var documentSession = _documentStore.OpenSession())
                    using (var imuSession = _imuSessionProvider.CreateInstance(_imuFactory.ModuleName))
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

                        imuSession.FindKeys(cachedResultBatch);

                        var results = imuSession.Fetch("start", 0, -1, _imuFactory.Columns);

                        Log.Logger.Debug("Fetched {RecordCount} {Name} records from Imu", cachedResultBatch.Count, typeof(T).Name);                        

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

                        importStatus.CurrentImportCacheOffset += results.Count;

                        Log.Logger.Information("{Name} import progress... {Offset}/{TotalResults}", typeof(T).Name, importStatus.CurrentImportCacheOffset, importCache.Irns.Count);
                        documentSession.SaveChanges();
                    }
                }

                using (var documentSession = _documentStore.OpenSession())
                {
                    // Mark import status as finished
                    documentSession.Load<Application>(Constants.ApplicationId).ImportFinished(GetType().ToString(), importCache.DateCreated);
                    documentSession.SaveChanges();
                }
            }
        }

        public override int Order => 100;
    }
}