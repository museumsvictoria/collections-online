using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Models;
using IMu;
using Raven.Client;

namespace CollectionsOnline.Import.Importers
{
    public class SpeciesImport : Import<Species>
    {
        public SpeciesImport(
            IDocumentStore documentStore,
            Session session) : base(documentStore, session)
        {
        }

        public override string ModuleName
        {
            get { return "enarratives"; }
        }

        public override string[] Columns
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
                        "media=MulMultiMediaRef_tab.(irn,MulTitle,MulMimeType,MulDescription,MdaDataSets_tab,MdaElement_tab,MdaQualifier_tab,MdaFreeText_tab,ChaRepository_tab,rights=<erights:MulMultiMediaRef_tab>.(RigType,RigAcknowledgement),AdmPublishWebNoPassword,AdmDateModified,AdmTimeModified)",
                    };
            }
        }

        public override Terms Terms
        {
            get
            {
                var terms = new Terms();

                terms.Add("DetPurpose_tab", "Website - Species profile");                

                return terms;
            }
        }

        public override Species MakeDocument(Map map)
        {
            var species = new Species();

            species.Id = "species/" + map.GetString("irn");

            species.IsHidden = map.GetString("AdmPublishWebNoPassword") == "No";

            species.DateModified = DateTime.ParseExact(
                string.Format("{0} {1}", map.GetString("AdmDateModified"), map.GetString("AdmTimeModified")),
                "dd/MM/yyyy HH:mm",
                new CultureInfo("en-AU"));

            species.AnimalType = map.GetString("SpeTaxonGroup");
            species.AnimalSubType = map.GetString("SpeTaxonSubGroup");

            species.Colours = map.GetStrings("SpeColour_tab");
            species.MaximumSize = string.Format("{0} {1}", map.GetString("SpeMaximumSize"), map.GetString("SpeUnit"));

            species.Habitats = map.GetStrings("SpeHabitat_tab");
            species.WhereToLook = map.GetStrings("SpeWhereToLook_tab");
            species.WhenActive = map.GetStrings("SpeWhereToLook_tab");
            species.NationalParks = map.GetStrings("SpeNationalParks_tab");

            species.Diet = map.GetString("SpeDiet");
            species.DietCategories = map.GetStrings("SpeDietCategories_tab");

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
            var conservationStatuses = new List<string>();
            foreach (var conservationMap in map.GetMaps("conservation"))
            {
                var authority = conservationMap.GetString("SpeConservationList_tab");
                var status = conservationMap.GetString("SpeStatus_tab");

                conservationStatuses.Add(string.Format("{0} {1}", authority, status));
            }
            species.ConservationStatuses = conservationStatuses;

            species.ScientificDiagnosis = map.GetString("SpeScientificDiagnosis");

            // Animal specific fields (spider/butterflies) 
            species.Web = map.GetString("SpeWeb");
            species.Plants = map.GetStrings("SpePlant_tab");
            species.FlightStart = map.GetString("SpeFlightStart");
            species.FlightEnd = map.GetString("SpeFlightEnd");
            species.Depths = map.GetStrings("SpeDepth_tab");
            species.WaterColumnLocations = map.GetStrings("SpeWaterColumnLocation_tab");

            // Get Taxonomy
            var taxaMap = map.GetMaps("taxa").FirstOrDefault();
            if (taxaMap != null)
            {
                var namesMap = taxaMap.GetMaps("names");
                var commonNames = new List<string>();
                var otherNames = new List<string>();
                foreach (var nameMap in namesMap)
                {
                    var status = nameMap.GetString("ComStatus_tab");
                    var name = nameMap.GetString("ComName_tab");

                    if (status != null && status.ToLower() == "preferred")
                    {
                        commonNames.Add(name);
                    }
                    else if (status != null && status.ToLower() == "other")
                    {
                        otherNames.Add(name);
                    }
                }
                species.CommonNames = commonNames;
                species.OtherNames = otherNames;

                species.Phylum = taxaMap.GetString("ClaPhylum");
                species.Subphylum = taxaMap.GetString("ClaSubphylum");
                species.Superclass = taxaMap.GetString("ClaSuperclass");
                species.Class = taxaMap.GetString("ClaClass");
                species.Subclass = taxaMap.GetString("ClaSubclass");
                species.Superorder = taxaMap.GetString("ClaSuperorder");
                species.Order = taxaMap.GetString("ClaOrder");
                species.Suborder = taxaMap.GetString("ClaSuborder");
                species.Infraorder = taxaMap.GetString("ClaInfraorder");
                species.Superfamily = taxaMap.GetString("ClaSuperfamily");
                species.Family = taxaMap.GetString("ClaFamily");
                species.Subfamily = taxaMap.GetString("ClaSubfamily");
                species.Genus = taxaMap.GetString("ClaGenus");
                species.Subgenus = taxaMap.GetString("ClaSubgenus");
                species.SpeciesName = taxaMap.GetString("ClaSpecies");
                species.Subspecies = taxaMap.GetString("ClaSubspecies");

                var othersMap = taxaMap.GetMaps("others");
                foreach (var otherMap in othersMap)
                {
                    var rank = otherMap.GetString("ClaOtherRank_tab");
                    var value = otherMap.GetString("ClaOtherValue_tab");

                    if (rank.ToLower() == "mov")
                    {
                        species.MoV = string.Format("MoV {0}", value);
                    }
                }

                species.Author = taxaMap.GetString("AutAuthorString");
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

                // Relationships
                species.SpecimenIds = taxaMap.GetMaps("specimens").Where(x => x != null).Select(x => "specimens/" + x.GetString("irn")).ToList();
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
            var media = new List<Media>();
            foreach (var mediaMap in map.GetMaps("media").Where(x => x.GetString("AdmPublishWebNoPassword") == "Yes"))
            {
                media.Add(new Media
                {
                    DateModified = DateTime.ParseExact(string.Format("{0} {1}", mediaMap.GetString("AdmDateModified"),
                                                        mediaMap.GetString("AdmTimeModified")),
                                                       "dd/MM/yyyy HH:mm", new CultureInfo("en-AU")),
                    Title = mediaMap.GetString("MulTitle"),
                    Type = mediaMap.GetString("MulMimeType")
                });
            }
            species.Media = media;

            return species;
        }
    }
}