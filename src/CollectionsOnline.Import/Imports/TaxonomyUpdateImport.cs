using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Models;
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

        public TaxonomyUpdateImport(
            IDocumentStore documentStore,
            Session session)
        {
            _documentStore = documentStore;
            _session = session;
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
                                    "names=[ComName_tab,ComStatus_tab]",
                                    "catalogue=<ecatalogue:TaxTaxonomyRef_tab>.(irn,sets=MdaDataSets_tab,identification=[IdeTypeStatus_tab,IdeCurrentNameLocal_tab,taxa=TaxTaxonomyRef_tab.(irn)])",
                                    "narrative=<enarratives:TaxTaxaRef_tab>.(irn,sets=DetPurpose_tab)"
                                };
            var types = new[] { "holotype", "lectotype", "neotype", "paralectotype", "paratype", "syntype", "type" };

            using (var documentSession = _documentStore.OpenSession())
            {
                // Check to see whether we need to run import, so grab the previous date run of any imports that utilize taxonomy.
                var previousDateRun = documentSession
                    .Load<Application>(Constants.ApplicationId)
                    .ImportStatuses.Where(x => x.ImportType.Contains("species", StringComparison.OrdinalIgnoreCase) || x.ImportType.Contains("specimen", StringComparison.OrdinalIgnoreCase))
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
                        while (true)
                        {
                            using (var specimenDocumentSession = _documentStore.OpenSession())
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
                                        var specimen = specimenDocumentSession.Load<Specimen>(catalogueIrn);

                                        if (specimen != null)
                                        {
                                            // Find the taxonomy record associated with this specimen and get the indentification tab so we know which fields to update on specimen record.
                                            var identification = catalogue
                                                .GetMaps("identification")
                                                .FirstOrDefault(x => x.GetMap("taxa") != null && x.GetMap("taxa").GetString("irn") == row.GetString("irn"));

                                            if (identification != null)
                                            {
                                                var currentName = identification.GetString("IdeCurrentNameLocal_tab");
                                                var typeStatus = identification.GetString("IdeTypeStatus_tab");

                                                if (types.Any(x => string.Equals(x, typeStatus, StringComparison.OrdinalIgnoreCase)) || string.Equals(currentName, "yes", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    specimen.ScientificName = new[]
                                                        {
                                                            row.GetString("ClaGenus"),
                                                            string.IsNullOrWhiteSpace(row.GetString("ClaSubgenus")) ? null : string.Format("({0})", row.GetString("ClaSubgenus")),
                                                            row.GetString("ClaSpecies"),
                                                            row.GetString("ClaSubspecies"),
                                                            row.GetString("AutAuthorString")
                                                        }.Concatenate(" ");

                                                    specimen.Kingdom = row.GetString("ClaKingdom");
                                                    specimen.Phylum = row.GetString("ClaPhylum");
                                                    specimen.Class = row.GetString("ClaClass");
                                                    specimen.Order = row.GetString("ClaOrder");
                                                    specimen.Family = row.GetString("ClaFamily");
                                                    specimen.Genus = row.GetString("ClaGenus");
                                                    specimen.Subgenus = row.GetString("ClaSubgenus");
                                                    specimen.SpecificEpithet = row.GetString("ClaSpecies");
                                                    specimen.InfraspecificEpithet = row.GetString("ClaSubspecies");
                                                    specimen.ScientificNameAuthorship = row.GetString("AutAuthorString");
                                                    specimen.NomenclaturalCode = row.GetString("ClaApplicableCode");

                                                    //higherClassification
                                                    var higherClassification = new Dictionary<string, string>
                                                        {
                                                            { "Kingdom", row.GetString("ClaKingdom") },
                                                            { "Phylum", row.GetString("ClaPhylum") },
                                                            { "Subphylum", row.GetString("ClaSubphylum") },
                                                            { "Superclass", row.GetString("ClaSuperclass") },
                                                            { "Class", row.GetString("ClaClass") },
                                                            { "Subclass", row.GetString("ClaSubclass") },
                                                            { "Superorder", row.GetString("ClaSuperorder") },
                                                            { "Order", row.GetString("ClaOrder") },
                                                            { "Suborder", row.GetString("ClaSuborder") },
                                                            { "Infraorder", row.GetString("ClaInfraorder") },
                                                            { "Superfamily", row.GetString("ClaSuperfamily") },
                                                            { "Family", row.GetString("ClaFamily") },
                                                            { "Subfamily", row.GetString("ClaSubfamily") },
                                                            { "Tribe", row.GetString("ClaTribe") },
                                                            { "Subtribe", row.GetString("ClaSubtribe") },
                                                            { "Genus", row.GetString("ClaGenus") },
                                                            { "Subgenus", row.GetString("ClaSubgenus") },
                                                            { "Species", row.GetString("ClaSpecies") },
                                                            { "Subspecies", row.GetString("ClaSubspecies") }
                                                        };

                                                    specimen.HigherClassification = higherClassification.Select(x => x.Value).Concatenate("; ");
                                                    specimen.TaxonRank = higherClassification.Where(x => !string.IsNullOrWhiteSpace(x.Value)).Select(x => x.Key).LastOrDefault();

                                                    //vernacularName
                                                    var vernacularName = row.GetMaps("names").FirstOrDefault(x => string.Equals(x.GetString("ComStatus_tab"), "preferred", StringComparison.OrdinalIgnoreCase));
                                                    if (vernacularName != null)
                                                        specimen.VernacularName = vernacularName.GetString("ComName_tab");
                                                }                                               
                                            }
                                        }
                                    }
                                }

                                // Save any changes
                                specimenDocumentSession.SaveChanges();
                                count += catalogueBatch.Count;
                            }
                        }

                        // Update linked species
                        count = 0;
                        var narratives = row.GetMaps("narrative");
                        while (true)
                        {
                            using (var speciesDocumentSession = _documentStore.OpenSession())
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
                                        var species = speciesDocumentSession.Load<Species>(narrativeIrn);

                                        if (species != null)
                                        {
                                            var names = row.GetMaps("names");
                                            var commonNames = new List<string>();
                                            var otherNames = new List<string>();
                                            foreach (var name in names)
                                            {
                                                var status = name.GetString("ComStatus_tab");
                                                var vernacularName = name.GetString("ComName_tab");

                                                if (string.Equals(status, "preferred", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    commonNames.Add(vernacularName);
                                                }
                                                else if (string.Equals(status, "other", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    otherNames.Add(vernacularName);
                                                }
                                            }
                                            species.CommonNames = commonNames;
                                            species.OtherNames = otherNames;

                                            species.Phylum = row.GetString("ClaPhylum");
                                            species.Subphylum = row.GetString("ClaSubphylum");
                                            species.Superclass = row.GetString("ClaSuperclass");
                                            species.Class = row.GetString("ClaClass");
                                            species.Subclass = row.GetString("ClaSubclass");
                                            species.Superorder = row.GetString("ClaSuperorder");
                                            species.Order = row.GetString("ClaOrder");
                                            species.Suborder = row.GetString("ClaSuborder");
                                            species.Infraorder = row.GetString("ClaInfraorder");
                                            species.Superfamily = row.GetString("ClaSuperfamily");
                                            species.Family = row.GetString("ClaFamily");
                                            species.Subfamily = row.GetString("ClaSubfamily");
                                            species.Genus = row.GetString("ClaGenus");
                                            species.Subgenus = row.GetString("ClaSubgenus");
                                            species.SpeciesName = row.GetString("ClaSpecies");
                                            species.Subspecies = row.GetString("ClaSubspecies");

                                            species.TaxonomyAuthor = row.GetString("AutAuthorString");
                                            species.HigherClassification = new[]
                                                {
                                                    species.Phylum,
                                                    species.Class,
                                                    species.Order,
                                                    species.Family
                                                }.Concatenate(" ");

                                            species.ScientificName = new[]
                                                {
                                                    row.GetString("ClaGenus"),
                                                    string.IsNullOrWhiteSpace(row.GetString("ClaSubgenus")) ? null : string.Format("({0})", row.GetString("ClaSubgenus")),
                                                    row.GetString("ClaSpecies"),
                                                    row.GetString("ClaSubspecies"),
                                                    row.GetString("AutAuthorString")
                                                }.Concatenate(" ");

                                            // Relationships
                                            species.SpecimenIds = catalogues
                                                .Where(x => x != null && x.GetStrings("sets").Contains(Constants.ImuSpecimenQueryString))
                                                .Select(x => "specimens/" + x.GetString("irn"))
                                                .ToList();
                                        }
                                    }
                                }

                                // Save any changes
                                speciesDocumentSession.SaveChanges();
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