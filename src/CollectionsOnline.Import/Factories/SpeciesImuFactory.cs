using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using AutoMapper;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Utilities;
using ImageResizer;
using IMu;

namespace CollectionsOnline.Import.Factories
{
    public class SpeciesImuFactory : IImuFactory<Species>
    {
        private readonly IMediaHelper _mediaHelper;

        public SpeciesImuFactory(
            IMediaHelper mediaHelper)
        {
            _mediaHelper = mediaHelper;
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
                    "taxa=TaxTaxaRef_tab.(irn,names=[ComName_tab,ComStatus_tab],ClaPhylum,ClaSubphylum,ClaSuperclass,ClaClass,ClaSubclass,ClaSuperorder,ClaOrder,ClaSuborder,ClaInfraorder,ClaSuperfamily,ClaFamily,ClaSubfamily,ClaGenus,ClaSubgenus,ClaSpecies,ClaSubspecies,ClaScientificName,others=[ClaOtherRank_tab,ClaOtherValue_tab],AutAuthorString,specimens=<ecatalogue:TaxTaxonomyRef_tab>.(irn))",
                    "authors=NarAuthorsRef_tab.(NamFullName,BioLabel,media=MulMultiMediaRef_tab.(irn,AdmPublishWebNoPassword))",
                    "media=MulMultiMediaRef_tab.(irn,MulTitle,MulMimeType,MulDescription,MulCreator_tab,MdaDataSets_tab,MdaElement_tab,MdaQualifier_tab,MdaFreeText_tab,ChaRepository_tab,AdmPublishWebNoPassword,AdmDateModified,AdmTimeModified)"
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
            species.WhenActive = map.GetStrings("SpeWhereToLook_tab") ?? new string[] { };
            species.NationalParks = map.GetStrings("SpeNationalParks_tab") ?? new string[] { };

            species.Diet = map.GetString("SpeDiet");
            species.DietCategories = map.GetStrings("SpeDietCategories_tab") ?? new string[] { };

            species.FastFact = map.GetString("SpeFastFact");
            species.Habitat = map.GetString("SpeHabitatNotes");
            species.Distribution = map.GetString("SpeDistribution");
            species.Biology = map.GetString("SpeBiology");
            species.IdentifyingCharacters = map.GetString("SpeIdentifyingCharacters");
            species.BriefId = map.GetString("SpeBriefID");
            species.Hazards = map.GetString("SpeHazards");
            species.Endemicity = map.GetString("SpeEndemicity");
            species.Commercial = map.GetString("SpeCommercialSpecies");

            // Get Conservation Status
            species.ConservationStatuses = new List<string>();
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

            // Get Taxonomy
            species.CommonNames = new List<string>();
            species.OtherNames = new List<string>();
            species.SpecimenIds = new List<string>();

            var taxonomy = map.GetMaps("taxa").FirstOrDefault();
            if (taxonomy != null)
            {
                var names = taxonomy.GetMaps("names");
                foreach (var name in names)
                {
                    var status = name.GetString("ComStatus_tab");
                    var vernacularName = name.GetString("ComName_tab");

                    if (status != null && status.ToLower() == "preferred")
                    {
                        species.CommonNames.Add(vernacularName);
                    }
                    else if (status != null && status.ToLower() == "other")
                    {
                        species.OtherNames.Add(vernacularName);
                    }
                }

                species.Phylum = taxonomy.GetString("ClaPhylum");
                species.Subphylum = taxonomy.GetString("ClaSubphylum");
                species.Superclass = taxonomy.GetString("ClaSuperclass");
                species.Class = taxonomy.GetString("ClaClass");
                species.Subclass = taxonomy.GetString("ClaSubclass");
                species.Superorder = taxonomy.GetString("ClaSuperorder");
                species.Order = taxonomy.GetString("ClaOrder");
                species.Suborder = taxonomy.GetString("ClaSuborder");
                species.Infraorder = taxonomy.GetString("ClaInfraorder");
                species.Superfamily = taxonomy.GetString("ClaSuperfamily");
                species.Family = taxonomy.GetString("ClaFamily");
                species.Subfamily = taxonomy.GetString("ClaSubfamily");
                species.Genus = taxonomy.GetString("ClaGenus");
                species.Subgenus = taxonomy.GetString("ClaSubgenus");
                species.SpeciesName = taxonomy.GetString("ClaSpecies");
                species.Subspecies = taxonomy.GetString("ClaSubspecies");

                var others = taxonomy.GetMaps("others");
                foreach (var other in others)
                {
                    var rank = other.GetString("ClaOtherRank_tab");
                    var value = other.GetString("ClaOtherValue_tab");

                    if (rank.ToLower() == "mov")
                    {
                        species.MoV = string.Format("MoV {0}", value);
                    }
                }

                species.Author = taxonomy.GetString("AutAuthorString");
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
                foreach (var specimen in taxonomy.GetMaps("specimens"))
                {
                    species.SpecimenIds.Add("specimens/" + specimen.GetString("irn"));
                }
            }

            // Authors
            var authors = new List<Author>();
            authors.AddRange(map.GetMaps("authors").Select(x => new Author
            {
                Name = x.GetString("NamFullName"),
                Biography = x.GetString("BioLabel")
            }));
            species.Authors = authors;

            // Media
            // TODO: Be more selective in what media we assign to item and how
            species.Media = new List<Media>();
            foreach (var media in map.GetMaps("media").Where(x =>
                x != null &&
                string.Equals(x.GetString("AdmPublishWebNoPassword"), "yes", StringComparison.OrdinalIgnoreCase) &&
                x.GetString("MulMimeType") == "image" && 
                x.GetStrings("MdaDataSets_tab").Any(y => y == "App - Field Guide")))
            {
                var irn = long.Parse(media.GetString("irn"));

                var url = PathFactory.GetUrlPath(irn, FileFormatType.Jpg, "thumb");
                var thumbResizeSettings = new ResizeSettings
                {
                    Format = FileFormatType.Jpg.ToString(),
                    Height = 365,
                    Width = 365,
                    Mode = FitMode.Crop,
                    PaddingColor = Color.White,
                    Quality = 65
                };

                if (_mediaHelper.Save(irn, FileFormatType.Jpg, thumbResizeSettings, "thumb"))
                {
                    species.Media.Add(new Media
                    {
                        Irn = irn,
                        DateModified = DateTime.ParseExact(string.Format("{0} {1}", media.GetString("AdmDateModified"),
                            media.GetString("AdmTimeModified")),
                            "dd/MM/yyyy HH:mm", new CultureInfo("en-AU")),
                        Title = media.GetString("MulTitle"),
                        Type = media.GetString("MulMimeType"),
                        Url = url
                    });
                }
            }

            // Build summary
            if (!string.IsNullOrWhiteSpace(species.IdentifyingCharacters))
                species.Summary = species.IdentifyingCharacters;
            else if (!string.IsNullOrWhiteSpace(species.Biology))
                species.Summary = species.Biology;
            else if (!string.IsNullOrWhiteSpace(species.HigherClassification))
                species.Summary = species.HigherClassification;

            return species;
        }

        public void RegisterAutoMapperMap()
        {
            Mapper.CreateMap<Species, Species>()
                .ForMember(x => x.Id, options => options.Ignore());
        }
    }
}