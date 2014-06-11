using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Factories;
using IMu;
using NLog;
using Raven.Abstractions.Extensions;
using Raven.Client;

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
                                    "ClaScientificName",
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
                                    "ClaTribe",
                                    "ClaSubtribe",
                                    "ClaGenus",
                                    "ClaSubgenus",
                                    "ClaSpecies",
                                    "ClaSubspecies",
                                    "ClaRank",
                                    "AutAuthorString",
                                    "ClaApplicableCode",
                                    "comname=[ComName_tab,ComStatus_tab]",
                                    "catalogue=<ecatalogue:TaxTaxonomyRef_tab>.(irn,sets=MdaDataSets_tab,identification=[taxa=TaxTaxonomyRef_tab.(irn)])",
                                    "narrative=<enarratives:TaxTaxaRef_tab>.(irn,sets=DetPurpose_tab)"
                                };            

            using (var documentSession = _documentStore.OpenSession())
            {
                // Check to see whether we need to run import, so grab the previous date run of any imports that utilize taxonomy.
                var previousDateRun = documentSession
                    .Load<Application>(Constants.ApplicationId)
                    .ImportStatuses.Where(x => x.ImportType.Contains("species", StringComparison.OrdinalIgnoreCase) || x.ImportType.Contains("specimen", StringComparison.OrdinalIgnoreCase) || x.ImportType.Contains("item", StringComparison.OrdinalIgnoreCase))
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
                using (var tx = new TransactionScope())
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
                        // Update linked specimens
                        var count = 0;
                        var catalogues = row.GetMaps("catalogue");
                        var taxonomyIrn = long.Parse(row.GetString("irn"));
                        while (true)
                        {
                            using (var catalogueDocumentSession = _documentStore.OpenSession())
                            {
                                if (ImportCanceled())
                                    return;

                                var catalogueBatch = catalogues
                                    .Skip(count)
                                    .Take(Constants.DataBatchSize)
                                    .ToList();

                                if (catalogueBatch.Count == 0)
                                    break;

                                foreach (var catalogue in catalogueBatch)
                                {
                                    var catalogueIrn = long.Parse(catalogue.GetString("irn"));
                                    var sets = catalogue.GetStrings("sets");

                                    // Check to see whether it is a specimen record.
                                    if (sets.Any(x => x == Constants.ImuSpecimenQueryString))
                                    {
                                        var specimen = catalogueDocumentSession.Load<Specimen>(catalogueIrn);

                                        if (specimen != null && specimen.Taxonomy.Irn == taxonomyIrn)
                                        {
                                            specimen.Taxonomy = _taxonomyFactory.Make(row);
                                        }
                                    }
                                    // Check to see whether it is an item record.
                                    if (sets.Any(x => x == Constants.ImuItemQueryString))
                                    {
                                        var item = catalogueDocumentSession.Load<Item>(catalogueIrn);

                                        if (item != null && item.ArtworkTaxonomy.Irn == taxonomyIrn)
                                        {
                                            item.ArtworkTaxonomy = _taxonomyFactory.Make(row);
                                        }
                                    }
                                }

                                // Save any changes
                                catalogueDocumentSession.SaveChanges();
                                count += catalogueBatch.Count;
                            }
                        }

                        // Update linked species
                        count = 0;
                        var narratives = row.GetMaps("narrative");
                        while (true)
                        {
                            using (var narrativeDocumentSession = _documentStore.OpenSession())
                            {
                                if (ImportCanceled())
                                    return;

                                var narrativesBatch = narratives
                                    .Skip(count)
                                    .Take(Constants.DataBatchSize)
                                    .ToList();

                                if (narrativesBatch.Count == 0)
                                    break;

                                foreach (var narrative in narrativesBatch)
                                {
                                    var narrativeIrn = long.Parse(narrative.GetString("irn"));
                                    var sets = narrative.GetStrings("sets");

                                    if (sets.Any(x => x == Constants.ImuSpeciesQueryString))
                                    {
                                        var species = narrativeDocumentSession.Load<Species>(narrativeIrn);

                                        if (species != null && species.Taxonomy.Irn == taxonomyIrn)
                                        {
                                            species.Taxonomy = _taxonomyFactory.Make(row);

                                            // Relationships
                                            species.SpecimenIds = catalogues
                                                .Where(x => x != null && x.GetStrings("sets").Contains(Constants.ImuSpecimenQueryString))
                                                .Select(x => "specimens/" + x.GetString("irn"))
                                                .ToList();
                                        }
                                    }
                                }

                                // Save any changes
                                narrativeDocumentSession.SaveChanges();
                                count += narrativesBatch.Count;
                            }
                        }
                    }

                    importStatus.CurrentOffset += results.Count;

                    _log.Debug("{0} import progress... {1}/{2}", GetType().Name, importStatus.CurrentOffset, importStatus.CachedResult.Count);
                    documentSession.SaveChanges();

                    tx.Complete();
                }

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