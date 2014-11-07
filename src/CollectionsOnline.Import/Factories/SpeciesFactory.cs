using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using AutoMapper;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Extensions;
using IMu;
using NLog;
using Raven.Abstractions.Extensions;

namespace CollectionsOnline.Import.Factories
{
    public class SpeciesFactory : IEmuAggregateRootFactory<Species>
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
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
                    "relateditemspecimens=ObjObjectsRef_tab.(irn,MdaDataSets_tab)",
                    "authors=NarAuthorsRef_tab.(NamFullName,BioLabel,media=MulMultiMediaRef_tab.(irn,MulTitle,MulMimeType,MdaDataSets_tab,metadata=[MdaElement_tab,MdaQualifier_tab,MdaFreeText_tab],DetAlternateText,RigCreator_tab,RigSource_tab,RigAcknowledgementCredit,RigCopyrightStatement,RigCopyrightStatus,RigLicence,RigLicenceDetails,AdmPublishWebNoPassword,AdmDateModified,AdmTimeModified))",
                    "media=MulMultiMediaRef_tab.(irn,MulTitle,MulMimeType,MdaDataSets_tab,metadata=[MdaElement_tab,MdaQualifier_tab,MdaFreeText_tab],DetAlternateText,RigCreator_tab,RigSource_tab,RigAcknowledgementCredit,RigCopyrightStatement,RigCopyrightStatus,RigLicence,RigLicenceDetails,AdmPublishWebNoPassword,AdmDateModified,AdmTimeModified)",
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
            var stopwatch = Stopwatch.StartNew();

            var species = new Species();

            species.Id = "species/" + map.GetEncodedString("irn");

            species.IsHidden = string.Equals(map.GetEncodedString("AdmPublishWebNoPassword"), "no", StringComparison.OrdinalIgnoreCase);

            species.DateModified = DateTime.ParseExact(
                string.Format("{0} {1}", map.GetEncodedString("AdmDateModified"), map.GetEncodedString("AdmTimeModified")),
                "dd/MM/yyyy HH:mm",
                new CultureInfo("en-AU"));

            species.AnimalType = map.GetEncodedString("SpeTaxonGroup");
            species.AnimalSubType = map.GetEncodedString("SpeTaxonSubGroup");

            species.Colours.AddRange(map.GetEncodedStrings("SpeColour_tab").Distinct());

            species.MaximumSize = string.Format("{0} {1}", map.GetEncodedString("SpeMaximumSize"), map.GetEncodedString("SpeUnit")).Trim();

            species.Habitats = map.GetEncodedStrings("SpeHabitat_tab");
            species.WhereToLook = map.GetEncodedStrings("SpeWhereToLook_tab");
            species.WhenActive = map.GetEncodedStrings("SpeWhenActive_tab");
            species.NationalParks = map.GetEncodedStrings("SpeNationalParks_tab");

            species.Diet = map.GetEncodedString("SpeDiet");
            species.DietCategories = map.GetEncodedStrings("SpeDietCategories_tab");

            species.FastFact = map.GetEncodedString("SpeFastFact");
            species.Habitat = map.GetEncodedString("SpeHabitatNotes");
            species.Distribution = map.GetEncodedString("SpeDistribution");
            species.Biology = map.GetEncodedString("SpeBiology");
            species.GeneralDescription = map.GetEncodedString("SpeIdentifyingCharacters");
            species.BriefId = map.GetEncodedString("SpeBriefID");
            species.Hazards = map.GetEncodedString("SpeHazards");
            species.Endemicity = map.GetEncodedString("SpeEndemicity");
            species.Commercial = map.GetEncodedString("SpeCommercialSpecies");

            // Get Conservation Status
            foreach (var conservationMap in map.GetMaps("conservation"))
            {
                var authority = conservationMap.GetEncodedString("SpeConservationList_tab");
                var status = conservationMap.GetEncodedString("SpeStatus_tab");

                if(!string.IsNullOrWhiteSpace(authority) && !string.IsNullOrWhiteSpace(status))
                    species.ConservationStatuses.Add(string.Format("{0} {1}", authority, status));
            }

            species.ScientificDiagnosis = map.GetEncodedString("SpeScientificDiagnosis");

            // Animal specific fields (spider/butterflies) 
            species.Web = map.GetEncodedString("SpeWeb");
            species.Plants = map.GetEncodedStrings("SpePlant_tab");
            species.FlightStart = map.GetEncodedString("SpeFlightStart");
            species.FlightEnd = map.GetEncodedString("SpeFlightEnd");
            species.Depths = map.GetEncodedStrings("SpeDepth_tab");
            species.WaterColumnLocations = map.GetEncodedStrings("SpeWaterColumnLocation_tab");

            // Taxonomy
            var taxonomyMap = map.GetMaps("taxa").FirstOrDefault();
            species.Taxonomy = _taxonomyFactory.Make(taxonomyMap);

            // Relationships

            // Related items/specimens (directly related)
            foreach (var relatedItemSpecimen in map.GetMaps("relateditemspecimens").Where(x => x != null && !string.IsNullOrWhiteSpace(x.GetEncodedString("irn"))))
            {
                if (relatedItemSpecimen.GetEncodedStrings("MdaDataSets_tab").Contains(Constants.ImuItemQueryString))
                    species.RelatedItemIds.Add(string.Format("items/{0}", relatedItemSpecimen.GetEncodedString("irn")));
                if (relatedItemSpecimen.GetEncodedStrings("MdaDataSets_tab").Contains(Constants.ImuSpecimenQueryString))
                    species.RelatedSpecimenIds.Add(string.Format("specimens/{0}", relatedItemSpecimen.GetEncodedString("irn")));
            }

            // Authors
            species.Authors = map.GetMaps("authors")
                .Where(x => x != null)
                .Select(x => new Author
                {
                    Name = x.GetEncodedString("NamFullName"),
                    Biography = x.GetEncodedString("BioLabel"),
                    ProfileImage = _mediaFactory.Make(x.GetMaps("media").FirstOrDefault()) as ImageMedia
                }).ToList();

            // Media
            species.Media = _mediaFactory.Make(map.GetMaps("media"));

            var thumbnail = species.Media.FirstOrDefault(x => x is ImageMedia) as ImageMedia;
            if (thumbnail != null)
                species.ThumbnailUri = thumbnail.Thumbnail.Uri;

            // Build summary
            if (!string.IsNullOrWhiteSpace(species.GeneralDescription))
                species.Summary = species.GeneralDescription;
            else if (!string.IsNullOrWhiteSpace(species.Biology))
                species.Summary = species.Biology;

            stopwatch.Stop();
            _log.Trace("Completed species creation for narrative record with irn {0}, elapsed time {1} ms", map.GetEncodedString("irn"), stopwatch.ElapsedMilliseconds);
            
            return species;
        }

        public void RegisterAutoMapperMap()
        {
            Mapper.CreateMap<Species, Species>()
                .ForMember(x => x.Id, options => options.Ignore());
        }
    }
}