using System;
using System.Diagnostics;
using System.Linq;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Extensions;
using CollectionsOnline.Import.Factories;
using CollectionsOnline.Import.Infrastructure;
using IMu;
using NLog;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Raven.Json.Linq;
using Constants = CollectionsOnline.Core.Config.Constants;

namespace CollectionsOnline.Import.Imports
{
    public class TaxonomyUpdateImport : IImport
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
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

        public void Run()
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
                    return;
                }

                _log.Debug("Starting {0} import", GetType().Name);

                // Check for existing cached results
                importCache = documentSession.Load<ImportCache>("importCaches/taxonomy");
                if (importCache == null)
                {
                    importCache = new ImportCache { Id = "importCaches/taxonomy" };

                    var terms = new Terms();
                    terms.Add("AdmDateModified", previousDateRun.Value.ToString("MMM dd yyyy"), ">=");

                    var hits = imuSession.FindTerms(terms);

                    _log.Debug("Caching {0} search results. {1} Hits", GetType().Name, hits);

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

                        _log.Debug("{0} cache progress... {1}/{2}", GetType().Name, cachedCurrentOffset, hits);
                    }

                    // Store cached result
                    documentSession.Store(importCache);
                    documentSession.SaveChanges();

                    _log.Debug("Caching of {0} search results complete, beginning import.", GetType().Name);
                }
                else
                {
                    _log.Debug("Cached search results found, resuming {0} import.", GetType().Name);
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

                    var stopwatch = Stopwatch.StartNew();

                    imuSession.FindKeys(cachedResultBatch);

                    var results = imuSession.Fetch("start", 0, -1, columns);

                    stopwatch.Stop();
                    _log.Trace("Found and fetched {0} taxonomy records in {1} ms", cachedResultBatch.Count, stopwatch.ElapsedMilliseconds);

                    foreach (var row in results.Rows)
                    {
                        // Update taxonomy on items, species and specimens
                        _documentStore.DatabaseCommands.UpdateByIndex(
                            "Combined",
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

                    _log.Debug("{0} import progress... {1}/{2}", GetType().Name, importStatus.CurrentImportCacheOffset, importCache.Irns.Count);
                    documentSession.SaveChanges();
                }
            }

            _log.Debug("{0} import complete", GetType().Name);

            using (var documentSession = _documentStore.OpenSession())
            {
                documentSession.Load<Application>(Constants.ApplicationId).ImportFinished(GetType().ToString(), importCache.DateCreated);
                documentSession.SaveChanges();
            }
        }

        public int Order
        {
            get { return 10; }
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