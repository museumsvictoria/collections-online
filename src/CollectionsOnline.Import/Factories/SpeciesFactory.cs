using System;
using System.Globalization;
using System.Linq;
using AutoMapper;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Models;
using IMu;

namespace CollectionsOnline.Import.Factories
{
    public class SpeciesFactory : IEmuAggregateRootFactory<Species>
    {
        private readonly ITaxonomyFactory _taxonomyFactory;
        private readonly IMediaFactory _mediaFactory;

        public SpeciesFactory(
            ITaxonomyFactory taxonomyFactory,
            IMediaFactory mediaFactory)
        {
            _taxonomyFactory = taxonomyFactory;
            _mediaFactory = mediaFactory;
        }

        public string ModuleName
        {
            get { return "enarratives"; }
        }

        public string[] Columns
        {
            get
            {
                return new[]
                {
                    "irn",
                    "AdmPublishWebNoPassword",
                    "AdmDateModified",
                    "AdmTimeModified",
                    "SpeTaxonGroup",
                    "SpeTaxonSubGroup",
                    "SpeColour_tab",
                    "SpeMaximumSize",
                    "SpeUnit",
                    "SpeHabitat_tab",
                    "SpeWhereToLook_tab",
                    "SpeWhenActive_tab",
                    "SpeNationalParks_tab",
                    "SpeDiet",
                    "SpeDietCategories_tab",
                    "SpeFastFact",
                    "SpeHabitatNotes",
                    "SpeDistribution",
                    "SpeBiology",
                    "SpeIdentifyingCharacters",
                    "SpeBriefID",
                    "SpeHazards",
                    "SpeEndemicity",
                    "SpeCommercialSpecies",
                    "conservation=[SpeConservationList_tab,SpeStatus_tab]",
                    "SpeScientificDiagnosis",
                    "SpeWeb",
                    "SpePlant_tab",
                    "SpeFlightStart",
                    "SpeFlightEnd",
                    "SpeDepth_tab",
                    "SpeWaterColumnLocation_tab",
                    "taxa=TaxTaxaRef_tab.(irn,ClaKingdom,ClaPhylum,ClaSubphylum,ClaSuperclass,ClaClass,ClaSubclass,ClaSuperorder,ClaOrder,ClaSuborder,ClaInfraorder,ClaSuperfamily,ClaFamily,ClaSubfamily,ClaGenus,ClaSubgenus,ClaSpecies,ClaSubspecies,AutAuthorString,ClaApplicableCode,comname=[ComName_tab,ComStatus_tab])",
                    "specimens=TaxTaxaRef_tab.(specimens=<ecatalogue:TaxTaxonomyRef_tab>.(irn,sets=MdaDataSets_tab))",
                    "authors=NarAuthorsRef_tab.(NamFullName,BioLabel,media=MulMultiMediaRef_tab.(irn,MulTitle,MulMimeType,MulDescription,MulCreator_tab,MdaDataSets_tab,MdaElement_tab,MdaQualifier_tab,MdaFreeText_tab,ChaRepository_tab,DetAlternateText,AdmPublishWebNoPassword,AdmDateModified,AdmTimeModified))",
                    "media=MulMultiMediaRef_tab.(irn,MulTitle,MulMimeType,MulDescription,MulCreator_tab,MdaDataSets_tab,MdaElement_tab,MdaQualifier_tab,MdaFreeText_tab,ChaRepository_tab,DetAlternateText,AdmPublishWebNoPassword,AdmDateModified,AdmTimeModified)"
                };
            }
        }

        public Terms Terms
        {
            get
            {
                var terms = new Terms();

                terms.Add("DetPurpose_tab", Constants.ImuSpeciesQueryString);

                return terms;
            }
        }

        public Species MakeDocument(Map map)
        {
            var species = new Species();

            species.Id = "species/" + map.GetString("irn");

            species.IsHidden = string.Equals(map.GetString("AdmPublishWebNoPassword"), "no", StringComparison.OrdinalIgnoreCase);

            species.DateModified = DateTime.ParseExact(
                string.Format("{0} {1}", map.GetString("AdmDateModified"), map.GetString("AdmTimeModified")),
                "dd/MM/yyyy HH:mm",
                new CultureInfo("en-AU"));

            species.AnimalType = map.GetString("SpeTaxonGroup");
            species.AnimalSubType = map.GetString("SpeTaxonSubGroup");

            species.Colours = map.GetStrings("SpeColour_tab") ?? new string[] { };
            species.MaximumSize = string.Format("{0} {1}", map.GetString("SpeMaximumSize"), map.GetString("SpeUnit")).Trim();

            species.Habitats = map.GetStrings("SpeHabitat_tab") ?? new string[] { };
            species.WhereToLook = map.GetStrings("SpeWhereToLook_tab") ?? new string[] { };
            species.WhenActive = map.GetStrings("SpeWhenActive_tab") ?? new string[] { };
            species.NationalParks = map.GetStrings("SpeNationalParks_tab") ?? new string[] { };

            species.Diet = map.GetString("SpeDiet");
            species.DietCategories = map.GetStrings("SpeDietCategories_tab") ?? new string[] { };

            species.FastFact = map.GetString("SpeFastFact");
            species.Habitat = map.GetString("SpeHabitatNotes");
            species.Distribution = map.GetString("SpeDistribution");
            species.Biology = map.GetString("SpeBiology");
            species.GeneralDescription = map.GetString("SpeIdentifyingCharacters");
            species.BriefId = map.GetString("SpeBriefID");
            species.Hazards = map.GetString("SpeHazards");
            species.Endemicity = map.GetString("SpeEndemicity");
            species.Commercial = map.GetString("SpeCommercialSpecies");

            // Get Conservation Status
            foreach (var conservationMap in map.GetMaps("conservation"))
            {
                var authority = conservationMap.GetString("SpeConservationList_tab");
                var status = conservationMap.GetString("SpeStatus_tab");

                species.ConservationStatuses.Add(string.Format("{0} {1}", authority, status));
            }

            species.ScientificDiagnosis = map.GetString("SpeScientificDiagnosis");

            // Animal specific fields (spider/butterflies) 
            species.Web = map.GetString("SpeWeb");
            species.Plants = map.GetStrings("SpePlant_tab") ?? new string[] { };
            species.FlightStart = map.GetString("SpeFlightStart");
            species.FlightEnd = map.GetString("SpeFlightEnd");
            species.Depths = map.GetStrings("SpeDepth_tab") ?? new string[] { };
            species.WaterColumnLocations = map.GetStrings("SpeWaterColumnLocation_tab") ?? new string[] { };

            // Taxonomy
            var taxonomyMap = map.GetMaps("taxa").FirstOrDefault();
            species.Taxonomy = _taxonomyFactory.Make(taxonomyMap);

            if (taxonomyMap != null)
            {
                // Scientific Name
                species.ScientificName = new[]
                {
                    species.Taxonomy.Genus,
                    string.IsNullOrWhiteSpace(species.Taxonomy.Subgenus)
                        ? null
                        : string.Format("({0})", species.Taxonomy.Subgenus),
                    species.Taxonomy.Species,
                    species.Taxonomy.Subspecies,
                    species.Taxonomy.Author
                }.Concatenate(" ");
            }

            // Relationships (specimen occurrences for species)
            // TODO: Add import to check for these relationships
            var specimensMap = map
                .GetMaps("specimens")
                .FirstOrDefault();

            if (specimensMap != null)
                species.SpecimenIds = specimensMap
                    .GetMaps("specimens")
                    .Where(x => x != null && x.GetStrings("sets").Contains(Constants.ImuSpecimenQueryString))
                    .Select(x => "specimens/" + x.GetString("irn"))
                    .ToList();

            // Authors
            species.Authors = map.GetMaps("authors")
                .Where(x => x != null)
                .Select(x => new Author
                {
                    Name = x.GetString("NamFullName"),
                    Biography = x.GetString("BioLabel"),
                    Media = _mediaFactory.Make(x.GetMaps("media").FirstOrDefault())
                }).ToList();

            // Media
            species.Media = _mediaFactory.Make(map.GetMaps("media"));

            // Build summary
            if (!string.IsNullOrWhiteSpace(species.GeneralDescription))
                species.Summary = species.GeneralDescription;
            else if (!string.IsNullOrWhiteSpace(species.Biology))
                species.Summary = species.Biology;
            
            return species;
        }

        public void RegisterAutoMapperMap()
        {
            Mapper.CreateMap<Species, Species>()
                .ForMember(x => x.Id, options => options.Ignore());
        }
    }
}