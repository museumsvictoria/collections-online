using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Factories;
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
        private readonly Session _session;
        private readonly ITaxonomyFactory _taxonomyFactory;

        public TaxonomyUpdateImport(
            IDocumentStore documentStore,
            Session session,
            ITaxonomyFactory taxonomyFactory)
        {
            _documentStore = documentStore;
            _session = session;
            _taxonomyFactory = taxonomyFactory;
        }

        public void Run()
        {
            var module = new Module("etaxonomy", _session);
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
                                    "comname=[ComName_tab,ComStatus_tab]",
                                    //"catalogue=<ecatalogue:TaxTaxonomyRef_tab>.(irn,sets=MdaDataSets_tab,identification=[taxa=TaxTaxonomyRef_tab.(irn)])",
                                    //"narrative=<enarratives:TaxTaxaRef_tab>.(irn,sets=DetPurpose_tab)"
                                };

            using (var documentSession = _documentStore.OpenSession())
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

                // Cache the search results
                if (importStatus.CachedResult == null)
                {
                    var terms = new Terms();                    
                    terms.Add("AdmDateModified", previousDateRun.Value.ToString("MMM dd yyyy"), ">=");
                    importStatus.CachedResult = new List<long>();
                    importStatus.CachedResultDate = DateTime.Now;

                    var hits = module.FindTerms(terms);

                    _log.Debug("Caching {0} search results. {1} Hits", GetType().Name, hits);

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

                        _log.Debug("{0} cache progress... {1}/{2}", GetType().Name, cachedCurrentOffset, hits);
                    }

                    // Store cached result
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

                    var results = module.Fetch("start", 0, -1, columns);

                    foreach (var row in results.Rows)
                    {
                        _documentStore.DatabaseCommands.UpdateByIndex(
                            "Combined",
                            new IndexQuery {Query = string.Format("TaxonomyIrn:{0}", row.GetString("irn"))},
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

                    importStatus.CurrentOffset += results.Count;

                    _log.Debug("{0} import progress... {1}/{2}", GetType().Name, importStatus.CurrentOffset, importStatus.CachedResult.Count);
                    documentSession.SaveChanges();
                }

            }
        }

        public int Order
        {
            get { return 10; }
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