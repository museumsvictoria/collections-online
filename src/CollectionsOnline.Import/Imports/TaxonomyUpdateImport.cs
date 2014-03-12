using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Factories;
using CollectionsOnline.Import.Utilities;
using ImageResizer;
using IMu;
using NLog;
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

        public void Run(DateTime? dateLastRun)
        {
            // Taxonomy update only happens if import has been run before
            if (dateLastRun.HasValue)
            {
                _log.Debug("Beginning Taxonomy update import");

                var module = new Module("etaxonomy", _session);

                var terms = new Terms();
                terms.Add("AdmDateModified", dateLastRun.Value.ToString("MMM dd yyyy"), ">=");

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

                var hits = module.FindTerms(terms);
                _log.Debug("Finished Search. {0} Hits", hits);

                var count = 0;

                while (true)
                {
                    if (Program.ImportCanceled)
                    {
                        _log.Debug("Canceling Data import");
                        return;
                    }

                    var results = module.Fetch("start", count, Constants.DataBatchSize, columns);

                    if (results.Count == 0)
                        break;

                    foreach (var row in results.Rows)
                    {
                        // Update specimens
                        var catalogues = row.GetMaps("catalogue");
                        using (var documentSession = _documentStore.OpenSession())
                        {
                            foreach (var catalogue in catalogues)
                            {
                                var catalogueIrn = long.Parse(catalogue.GetString("irn"));
                                var sets = catalogue.GetStrings("sets");

                                // Check to see whether it is a specimen record.
                                if (sets.Any(x => x == Constants.ImuSpecimenQueryString))
                                {
                                    var specimen = documentSession.Load<Specimen>(catalogueIrn);

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
                                                }.Concatenate(";");

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

                            documentSession.SaveChanges();
                        }

                        // Update species
                        var narratives = row.GetMaps("narrative");
                        using (var documentSession = _documentStore.OpenSession())
                        {
                            foreach (var narrative in narratives)
                            {
                                var narrativeIrn = long.Parse(narrative.GetString("irn"));
                                var sets = narrative.GetStrings("sets");

                                if (sets.Any(x => x == Constants.ImuSpeciesQueryString))
                                {
                                    var species = documentSession.Load<Species>(narrativeIrn);

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

                                        species.Author = row.GetString("AutAuthorString");
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
                                            species.Author
                                        }.Concatenate(" ");

                                        // Relationships TODO: add filter to get only specimens added in specimen import
                                        species.SpecimenIds = new List<string>();
                                        foreach (var specimen in catalogues)
                                        {
                                            species.SpecimenIds.Add("specimens/" + specimen.GetString("irn"));
                                        }
                                    }
                                }
                            }

                            documentSession.SaveChanges();
                        }
                    }

                    count += results.Count;
                    _log.Debug("import progress... {0}/{1}", count, hits);
                }
            }
        }
    }
}