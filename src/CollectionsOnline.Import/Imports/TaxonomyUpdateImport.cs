using System;
using System.Diagnostics;
using System.Linq;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Extensions;
using CollectionsOnline.Import.Factories;
using CollectionsOnline.Import.Infrastructure;
using IMu;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Raven.Json.Linq;
using Serilog;
using Constants = CollectionsOnline.Core.Config.Constants;

namespace CollectionsOnline.Import.Imports
{
    public class TaxonomyUpdateImport : ImuImportBase
    {
        private readonly IDocumentStore _documentStore;
        private readonly IImuSessionProvider _imuSessionProvider;
        private readonly ITaxonomyFactory _taxonomyFactory;

        public TaxonomyUpdateImport(
            IDocumentStore documentStore,
            IImuSessionProvider imuSessionProvider,
            ITaxonomyFactory taxonomyFactory)
        {
            _documentStore = documentStore;
            _imuSessionProvider = imuSessionProvider;
            _taxonomyFactory = taxonomyFactory;
        }

        public override void Run()
        {
            using (Log.Logger.BeginTimedOperation("Taxonomy Update Import starting", "TaxonomyUpdateImport.Run"))
            { 
                ImportCache importCache;
                var columns = new[]
                                    {
                                        "irn",
                                        "ClaKingdom",
                                        "ClaPhylum",
                                        "ClaSubphylum",
                                        "ClaSuperclass",
                                        "ClaClass",
                                        "ClaSubclass",
                                        "ClaSuperorder",
                                        "ClaOrder",
                                        "ClaSuborder",
                                        "ClaInfraorder",
                                        "ClaSuperfamily",
                                        "ClaFamily",
                                        "ClaSubfamily",
                                        "ClaGenus",
                                        "ClaSubgenus",
                                        "ClaSpecies",
                                        "ClaSubspecies",
                                        "AutAuthorString",
                                        "ClaApplicableCode",
                                        "comname=[ComName_tab,ComStatus_tab]"
                                    };
                var moduleName = "etaxonomy";

                using (var documentSession = _documentStore.OpenSession())
                using (var imuSession = _imuSessionProvider.CreateInstance(moduleName))
                {
                    // Check to see whether we need to run import, so grab the previous date run of any imports that utilize taxonomy.
                    var previousDateRun = documentSession
                        .Load<Application>(Constants.ApplicationId)
                        .ImportStatuses.Where(x => x.ImportType.Contains(typeof(Species).Name, StringComparison.OrdinalIgnoreCase) || x.ImportType.Contains(typeof(Specimen).Name, StringComparison.OrdinalIgnoreCase) || x.ImportType.Contains(typeof(Item).Name, StringComparison.OrdinalIgnoreCase))
                        .Select(x => x.PreviousDateRun)
                        .OrderBy(x => x)
                        .FirstOrDefault(x => x.HasValue);

                    // Exit current import if it has never run.
                    if (!previousDateRun.HasValue)
                        return;

                    // Check for existing import in case we need to resume.
                    var importStatus = documentSession.Load<Application>(Constants.ApplicationId).GetImportStatus(GetType().ToString());

                    // Exit current import if it had completed previous time it was run.
                    if (importStatus.IsFinished)
                    {
                        Log.Logger.Information("Import finished last time... skipping");
                        return;
                    }                    

                    // Check for existing cached results
                    importCache = documentSession.Load<ImportCache>("importCaches/taxonomy");
                    if (importCache == null)
                    {
                        Log.Logger.Information("Caching {Name} results", GetType().Name);
                        importCache = new ImportCache { Id = "importCaches/taxonomy" };

                        var terms = new Terms();
                        terms.Add("AdmDateModified", previousDateRun.Value.ToString("MMM dd yyyy"), ">=");

                        var hits = imuSession.FindTerms(terms);

                        Log.Logger.Information("Found {Hits} records where {@Terms}", hits, terms.List);

                        var cachedCurrentOffset = 0;
                        while (true)
                        {
                            if (ImportCanceled())
                                return;

                            var results = imuSession.Fetch("start", cachedCurrentOffset, Constants.CachedDataBatchSize, new[] { "irn" });

                            if (results.Count == 0)
                                break;

                            importCache.Irns.AddRange(results.Rows.Select(x => long.Parse(x.GetEncodedString("irn"))));

                            cachedCurrentOffset += results.Count;

                            Log.Logger.Information("{Name} cache progress... {Offset}/{TotalResults}", GetType().Name, cachedCurrentOffset, hits);
                        }

                        // Store cached result
                        documentSession.Store(importCache);
                        documentSession.SaveChanges();

                        Log.Logger.Information("Caching of {Name} results complete", GetType().Name);
                    }
                    else
                    {
                        Log.Logger.Information("Cached results found, resuming {Name} import.", GetType().Name);
                    }
                }

                // Perform import
                while (true)
                {
                    using (var documentSession = _documentStore.OpenSession())
                    using (var imuSession = _imuSessionProvider.CreateInstance(moduleName))
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

                        var results = imuSession.Fetch("start", 0, -1, columns);

                        Log.Logger.Debug("Fetched {RecordCount} taxonomy records from Imu", cachedResultBatch.Count);

                        foreach (var row in results.Rows)
                        {
                            // Update taxonomy on items, species and specimens
                            _documentStore.DatabaseCommands.UpdateByIndex(
                                "CombinedIndex",
                                new IndexQuery { Query = string.Format("TaxonomyIrn:{0}", row.GetString("irn")) },
                                new[]
                                {
                                    new PatchRequest
                                    {
                                        Type = PatchCommandType.Set,
                                        Name = "Taxonomy",
                                        Value = RavenJObject.FromObject(_taxonomyFactory.Make(row))
                                    }
                                });
                        }

                        importStatus.CurrentImportCacheOffset += results.Count;

                        Log.Logger.Information("{Name} import progress... {Offset}/{TotalResults}", GetType().Name, importStatus.CurrentImportCacheOffset, importCache.Irns.Count);
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

        public override int Order
        {
            get { return 10; }
        }        
    }
}