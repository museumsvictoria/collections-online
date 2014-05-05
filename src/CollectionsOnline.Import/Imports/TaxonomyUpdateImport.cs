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
                                    "others=[ClaOtherRank_tab,ClaOtherValue_tab]",
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
                        // Update specimens
                        var catalogs = row.GetMaps("catalogue");
                        using (var specimenDocumentSession = _documentStore.OpenSession())
                        {
                            foreach (var catalogue in catalogs)
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
                                                specimen.ScientificName = row.GetString("ClaScientificName");
                                                specimen.Kingdom = row.GetString("ClaKingdom");
                                                specimen.Phylum = row.GetString("ClaPhylum");
                                                specimen.Class = row.GetString("ClaClass");
                                                specimen.Order = row.GetString("ClaOrder");
                                                specimen.Family = row.GetString("ClaFamily");
                                                specimen.Genus = row.GetString("ClaGenus");
                                                specimen.Subgenus = row.GetString("ClaSubgenus");
                                                specimen.SpecificEpithet = row.GetString("ClaSpecies");
                                                specimen.InfraspecificEpithet = row.GetString("ClaSubspecies");
                                                specimen.TaxonRank = row.GetString("ClaRank");
                                                specimen.ScientificNameAuthorship = row.GetString("AutAuthorString");
                                                specimen.NomenclaturalCode = row.GetString("ClaApplicableCode");

                                                //higherClassification
                                                specimen.HigherClassification = new[]
                                                            {
                                                                row.GetString("ClaKingdom"), 
                                                                row.GetString("ClaPhylum"),
                                                                row.GetString("ClaSubphylum"),
                                                                row.GetString("ClaSuperclass"),
                                                                row.GetString("ClaClass"),
                                                                row.GetString("ClaSubclass"),
                                                                row.GetString("ClaSuperorder"),
                                                                row.GetString("ClaOrder"),
                                                                row.GetString("ClaSuborder"),
                                                                row.GetString("ClaInfraorder"),
                                                                row.GetString("ClaSuperfamily"),
                                                                row.GetString("ClaFamily"),
                                                                row.GetString("ClaSubfamily"),
                                                                row.GetString("ClaTribe"),
                                                                row.GetString("ClaSubtribe"),
                                                                row.GetString("ClaGenus"),
                                                                row.GetString("ClaSubgenus"),
                                                                row.GetString("ClaSpecies"),
                                                                row.GetString("ClaSubspecies")
                                                            }.Concatenate("; ");

                                                //vernacularName
                                                var vernacularName = row.GetMaps("names").FirstOrDefault(x => string.Equals(x.GetString("ComStatus_tab"), "preferred", StringComparison.OrdinalIgnoreCase));
                                                if (vernacularName != null)
                                                    specimen.VernacularName = vernacularName.GetString("ComName_tab");
                                            }

                                            if (string.Equals(currentName, "yes", StringComparison.OrdinalIgnoreCase))
                                            {
                                                specimen.AcceptedNameUsage = row.GetString("ClaScientificName");
                                            }

                                            if (types.Any(x => string.Equals(x, typeStatus, StringComparison.OrdinalIgnoreCase)))
                                            {
                                                specimen.OriginalNameUsage = row.GetString("ClaScientificName");
                                            }
                                        }
                                    }
                                }
                            }

                            specimenDocumentSession.SaveChanges();
                        }

                        // Update species
                        var narratives = row.GetMaps("narrative");
                        using (var speciesDocumentSession = _documentStore.OpenSession())
                        {
                            foreach (var narrative in narratives)
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

                                        var others = row.GetMaps("others");
                                        foreach (var other in others)
                                        {
                                            var rank = other.GetString("ClaOtherRank_tab");
                                            var value = other.GetString("ClaOtherValue_tab");

                                            if (string.Equals(rank, "mov", StringComparison.OrdinalIgnoreCase))
                                            {
                                                species.MoV = string.Format("MoV {0}", value);
                                            }
                                        }

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
                                                        species.Genus,
                                                        species.SpeciesName,
                                                        species.MoV,
                                                        species.TaxonomyAuthor
                                                    }.Concatenate(" ");

                                        // Relationships TODO: add filter to get only specimens added in specimen import
                                        species.SpecimenIds = new List<string>();
                                        foreach (var specimen in catalogs)
                                        {
                                            species.SpecimenIds.Add("specimens/" + specimen.GetString("irn"));
                                        }
                                    }
                                }
                            }

                            speciesDocumentSession.SaveChanges();
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