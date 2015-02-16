using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using AutoMapper;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Factories;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Core.Utilities;
using CollectionsOnline.Import.Extensions;
using IMu;
using NLog;
using Raven.Abstractions.Extensions;
using Raven.Client;

namespace CollectionsOnline.Import.Factories
{
    public class SpecimenFactory : IEmuAggregateRootFactory<Specimen>
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly ISlugFactory _slugFactory;
        private readonly IPartiesNameFactory _partiesNameFactory;
        private readonly ITaxonomyFactory _taxonomyFactory;
        private readonly IMediaFactory _mediaFactory;
        private readonly IAssociationFactory _associationFactory;
        private readonly IMuseumLocationFactory _museumLocationFactory;

        public SpecimenFactory(
            ISlugFactory slugFactory,
            IPartiesNameFactory partiesNameFactory,
            ITaxonomyFactory taxonomyFactory,
            IMediaFactory mediaFactory,
            IAssociationFactory associationFactory,
            IMuseumLocationFactory museumLocationFactory)
        {
            _slugFactory = slugFactory;
            _partiesNameFactory = partiesNameFactory;
            _taxonomyFactory = taxonomyFactory;
            _mediaFactory = mediaFactory;
            _associationFactory = associationFactory;
            _museumLocationFactory = museumLocationFactory;

            Mapper.CreateMap<Specimen, Specimen>()
                .ForMember(x => x.Id, options => options.Ignore());
        }

        public string ModuleName
        {
            get { return "ecatalogue"; }
        }

        public string[] Columns
        {
            get
            {
                return new[]
                    {
                        "irn",
                        "AdmPublishWebNoPassword",
                        "ColRegPrefix",
                        "ColRegNumber",
                        "ColRegPart",
                        "ColTypeOfItem",
                        "AdmDateModified",
                        "AdmTimeModified",
                        "colevent=ColCollectionEventRef.(ExpExpeditionName,ColCollectionEventCode,ColCollectionMethod,ColDateVisitedFrom,ColDateVisitedTo,ColTimeVisitedFrom,ColTimeVisitedTo,AquDepthToMet,AquDepthFromMet,site=ColSiteRef.(SitSiteCode,SitSiteNumber,EraEra,EraAge1,EraAge2,EraMvStage,EraMvGroup_tab,EraMvRockUnit_tab,EraMvMember_tab,EraLithology_tab,geo=[LocOcean_tab,LocContinent_tab,LocCountry_tab,LocProvinceStateTerritory_tab,LocDistrictCountyShire_tab,LocTownship_tab,LocNearestNamedPlace_tab],LocPreciseLocation,LocElevationASLFromMt,LocElevationASLToMt,latlong=[LatLongitudeDecimal_nesttab,LatLatitudeDecimal_nesttab,LatDatum_tab,LatRadiusNumeric_tab,determinedBy=LatDeterminedByRef_tab.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName),LatDetDate0,LatLatLongDetermination_tab,LatDetSource_tab]),collectors=ColParticipantRef_tab.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName))",
                        "SpeNoSpecimens",
                        "BirTotalClutchSize",
                        "SpeSex_tab",
                        "SpeStageAge_tab",
                        "storage=[StrSpecimenNature_tab,StrSpecimenForm_tab,StrFixativeTreatment_tab,StrStorageMedium_tab]",
                        "site=SitSiteRef.(SitSiteCode,SitSiteNumber,EraEra,EraAge1,EraAge2,EraMvStage,EraMvGroup_tab,EraMvRockUnit_tab,EraMvMember_tab,EraLithology_tab,geo=[LocOcean_tab,LocContinent_tab,LocCountry_tab,LocProvinceStateTerritory_tab,LocDistrictCountyShire_tab,LocTownship_tab,LocNearestNamedPlace_tab],LocPreciseLocation,LocElevationASLFromMt,LocElevationASLToMt,latlong=[LatLongitudeDecimal_nesttab,LatLatitudeDecimal_nesttab,LatDatum_tab,LatRadiusNumeric_tab,determinedBy=LatDeterminedByRef_tab.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName),LatDetDate0,LatLatLongDetermination_tab,LatDetSource_tab])",
                        "identifications=[IdeTypeStatus_tab,IdeCurrentNameLocal_tab,identifiers=IdeIdentifiedByRef_nesttab.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName),IdeDateIdentified0,IdeQualifier_tab,IdeQualifierRank_tab,taxa=TaxTaxonomyRef_tab.(irn,ClaKingdom,ClaPhylum,ClaSubphylum,ClaSuperclass,ClaClass,ClaSubclass,ClaSuperorder,ClaOrder,ClaSuborder,ClaInfraorder,ClaSuperfamily,ClaFamily,ClaSubfamily,ClaGenus,ClaSubgenus,ClaSpecies,ClaSubspecies,AutAuthorString,ClaApplicableCode,comname=[ComName_tab,ComStatus_tab],relatedspecies=<enarratives:TaxTaxaRef_tab>.(irn,DetPurpose_tab))]",
                        "media=MulMultiMediaRef_tab.(irn,MulTitle,MulMimeType,MdaDataSets_tab,metadata=[MdaElement_tab,MdaQualifier_tab,MdaFreeText_tab],DetAlternateText,RigCreator_tab,RigSource_tab,RigAcknowledgementCredit,RigCopyrightStatement,RigCopyrightStatus,RigLicence,RigLicenceDetails,AdmPublishWebNoPassword,AdmDateModified,AdmTimeModified)",
                        "ColCategory",
                        "ColScientificGroup",
                        "ColDiscipline",
                        "ColCollectionName_tab",
                        "ClaPrimaryClassification",
                        "ClaSecondaryClassification",
                        "ClaTertiaryClassification",
                        "ClaObjectName",
                        "ClaObjectSummary",
                        "Con1Description",
                        "SubHistoryTechSignificance",
                        "SubThemes_tab",
                        "SubSubjects_tab",
                        "associations=[AssAssociationType_tab,party=AssAssociationNameRef_tab.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName),AssAssociationCountry_tab,AssAssociationState_tab,AssAssociationRegion_tab,AssAssociationLocality_tab,AssAssociationStreetAddress_tab,AssAssociationDate_tab,AssAssociationComments0]",
                        "relateditemspecimens=ColRelatedRecordsRef_tab.(irn,MdaDataSets_tab)",
                        "attacheditemspecimens=ColPhysicallyAttachedToRef.(irn,MdaDataSets_tab)",
                        "parentitemspecimens=ColParentRecordRef.(irn,MdaDataSets_tab)",
                        "accession=AccAccessionLotRef.(AcqAcquisitionMethod,AcqDateReceived,AcqDateOwnership,AcqCreditLine,source=[name=AcqSourceRef_tab.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName),AcqSourceRole_tab],AdmPublishWebNoPassword)",
                        "RigText0",
                        "location=LocCurrentLocationRef.(LocLocationType,location=LocHolderLocationRef.(LocLocationType,location=LocHolderLocationRef.(LocLocationType,location=LocHolderLocationRef.(LocLocationType,location=LocHolderLocationRef.(LocLocationType,location=LocHolderLocationRef.(LocLocationType,LocLevel1,LocLevel2,LocLevel3,LocLevel4),LocLevel1,LocLevel2,LocLevel3,LocLevel4),LocLevel1,LocLevel2,LocLevel3,LocLevel4),LocLevel1,LocLevel2,LocLevel3,LocLevel4),LocLevel1,LocLevel2,LocLevel3,LocLevel4),LocLevel1,LocLevel2,LocLevel3,LocLevel4)",
                        "LocDateCollectedFrom",
                        "LocDateCollectedTo",
                        "LocSamplingMethod",
                        "MinSpecies",
                        "MinVariety",
                        "MinGroup",
                        "MinClass",
                        "MinAssociatedMatrix",
                        "MinType",
                        "MinTypeType",
                        "MetName",
                        "MetClass",
                        "MetGroup",
                        "MetType",
                        "MetMainMineralsPresent",
                        "MetSpecimenWeight",
                        "MetTotalWeight",
                        "MetDateSpecimenFell",
                        "MetDateSpecimenFound",
                        "TekName",
                        "TekClassification",
                        "TekShape",
                        "TekLocalStrewnfield",
                        "TekGlobalStrewnfield",
                        "RocClass",
                        "RocGroup",
                        "RocRockName",
                        "RocRockDescription",
                        "relatedarticlespecies=<enarratives:ObjObjectsRef_tab>.(irn,DetPurpose_tab)",
                        "relatedpartyarticles=AssAssociationNameRef_tab.(relatedarticles=<enarratives:ParPartiesRef_tab>.(irn,DetPurpose_tab))",
                        "relatedsitearticles=ArcSiteNameRef.(relatedarticles=<enarratives:SitSitesRef_tab>.(irn,DetPurpose_tab))",
                        "relatedcolleventarticles=ColCollectionEventRef.(relatedarticles=<enarratives:ColCollectionEventsRef_tab>.(irn,DetPurpose_tab))"
                    };
            }
        }

        public Terms Terms
        {
            get
            {
                var terms = new Terms();

                terms.Add("MdaDataSets_tab", Constants.ImuSpecimenQueryString);

                return terms;
            }
        }

        public Specimen MakeDocument(Map map)
        {
            var stopwatch = Stopwatch.StartNew();

            var specimen = new Specimen();

            specimen.Id = "specimens/" + map.GetEncodedString("irn");

            specimen.IsHidden = string.Equals(map.GetEncodedString("AdmPublishWebNoPassword"), "no", StringComparison.OrdinalIgnoreCase);

            specimen.DateModified = DateTime.ParseExact(
                string.Format("{0} {1}", map.GetEncodedString("AdmDateModified"), map.GetEncodedString("AdmTimeModified")),
                "dd/MM/yyyy HH:mm",
                new CultureInfo("en-AU"));

            specimen.Category = map.GetEncodedString("ColCategory");
            specimen.ScientificGroup = map.GetEncodedString("ColScientificGroup");
            specimen.Discipline = map.GetEncodedString("ColDiscipline");
            specimen.RegistrationNumber = map["ColRegPart"] != null
                             ? string.Format("{0}{1}.{2}", map["ColRegPrefix"], map["ColRegNumber"], map["ColRegPart"])
                             : string.Format("{0}{1}", map["ColRegPrefix"], map["ColRegNumber"]);
            specimen.CollectionNames = map.GetEncodedStrings("ColCollectionName_tab");
            specimen.Type = map.GetEncodedString("ColTypeOfItem");

            // Classifications
            if (map.GetEncodedString("ClaPrimaryClassification") != null && !map.GetEncodedString("ClaPrimaryClassification").Contains("to be classified", StringComparison.OrdinalIgnoreCase))
                specimen.PrimaryClassification = map.GetEncodedString("ClaPrimaryClassification").ToSentenceCase();
            if (map.GetEncodedString("ClaSecondaryClassification") != null && !map.GetEncodedString("ClaSecondaryClassification").Contains("to be classified", StringComparison.OrdinalIgnoreCase))
                specimen.SecondaryClassification = map.GetEncodedString("ClaSecondaryClassification").ToSentenceCase();
            if (map.GetEncodedString("ClaTertiaryClassification") != null && !map.GetEncodedString("ClaTertiaryClassification").Contains("to be classified", StringComparison.OrdinalIgnoreCase))
                specimen.TertiaryClassification = map.GetEncodedString("ClaTertiaryClassification").ToSentenceCase();

            specimen.ObjectName = map.GetEncodedString("ClaObjectName");
            specimen.ObjectSummary = map.GetEncodedString("ClaObjectSummary");
            specimen.IsdDescriptionOfContent = map.GetEncodedString("Con1Description");
            specimen.Significance = map.GetEncodedString("SubHistoryTechSignificance");

            // Tags
            specimen.Keywords.AddRange(map.GetEncodedStrings("SubSubjects_tab"));

            // Collection plans
            specimen.CollectionPlans = map.GetEncodedStrings("SubThemes_tab");

            // Associations
            specimen.Associations = _associationFactory.Make(map.GetMaps("associations"));

            // Acquisition information
            // TODO: make factory method as code duplicated in ItemFactory
            var accessionMap = map.GetMap("accession");
            if (accessionMap != null &&
                string.Equals(accessionMap.GetEncodedString("AdmPublishWebNoPassword"), "yes", StringComparison.OrdinalIgnoreCase))
            {
                var method = accessionMap.GetEncodedString("AcqAcquisitionMethod");

                if (!string.IsNullOrWhiteSpace(method))
                {
                    var sources = accessionMap.GetMaps("source")
                    .Where(x => string.IsNullOrWhiteSpace(x.GetEncodedString("AcqSourceRole_tab")) ||
                        (!x.GetEncodedString("AcqSourceRole_tab").Contains("confindential", StringComparison.OrdinalIgnoreCase) &&
                         !x.GetEncodedString("AcqSourceRole_tab").Contains("contact", StringComparison.OrdinalIgnoreCase) &&
                         !x.GetEncodedString("AcqSourceRole_tab").Contains("vendor", StringComparison.OrdinalIgnoreCase)))
                    .Select(x => _partiesNameFactory.Make(x.GetMap("name"))).ToList();

                    if (sources.Any())
                    {
                        if (!string.IsNullOrWhiteSpace(accessionMap.GetEncodedString("AcqDateReceived")))
                            sources.Add(accessionMap.GetEncodedString("AcqDateReceived"));
                        else if (!string.IsNullOrWhiteSpace(accessionMap.GetEncodedString("AcqDateOwnership")))
                            sources.Add(accessionMap.GetEncodedString("AcqDateOwnership"));

                        specimen.AcquisitionInformation = string.Format("{0} from {1}", method, sources.Concatenate(", "));
                    }
                    else
                    {
                        specimen.AcquisitionInformation = method;
                    }
                }

                var rights = map.GetEncodedStrings("RigText0").FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(accessionMap.GetEncodedString("AcqCreditLine")))
                    specimen.Acknowledgement = accessionMap.GetEncodedString("AcqCreditLine");
                else if (!string.IsNullOrWhiteSpace(rights))
                    specimen.Acknowledgement = rights;
            }

            // Object Location
            specimen.MuseumLocation = _museumLocationFactory.Make(map.GetMap("location"));

            // Number Of Specimens
            specimen.NumberOfSpecimens = map.GetEncodedString("SpeNoSpecimens");
            // Clutch Size
            specimen.ClutchSize = map.GetEncodedString("BirTotalClutchSize");
            // Sex
            specimen.Sex = map.GetEncodedStrings("SpeSex_tab").Concatenate(", ");
            // Stage Or Age
            specimen.StageOrAge = map.GetEncodedStrings("SpeStageAge_tab").Concatenate(", ");
            // Storages
            specimen.Storages.AddRange(
                map.GetMaps("storage")
                    .Select(x => new Storage
                    {
                        Nature = x.GetEncodedString("StrSpecimenNature_tab"),
                        Form = x.GetEncodedString("StrSpecimenForm_tab"),
                        FixativeTreatment = x.GetEncodedString("StrFixativeTreatment_tab"),
                        Medium = x.GetEncodedString("StrStorageMedium_tab")
                    })
                    .Where(x => x != null));

            // Taxonomy
            // TODO: make factory method as code duplicated in ItemFactory
            var identificationMap = map.GetMaps("identifications").FirstOrDefault(x => Constants.TaxonomyTypeStatuses.Contains(x.GetEncodedString("IdeTypeStatus_tab"))) ??
                                    map.GetMaps("identifications").FirstOrDefault(x => string.Equals(x.GetEncodedString("IdeCurrentNameLocal_tab"), "yes", StringComparison.OrdinalIgnoreCase));
            if (identificationMap != null)
            {
                // Type Status
                specimen.TypeStatus = identificationMap.GetEncodedString("IdeTypeStatus_tab");
                // Identified By
                if (identificationMap.GetMaps("identifiers") != null)
                {
                    specimen.IdentifiedBy = identificationMap.GetMaps("identifiers").Where(x => x != null).Select(x => _partiesNameFactory.Make(x)).Concatenate("; ");
                }
                // Date Identified
                specimen.DateIdentified = identificationMap.GetEncodedString("IdeDateIdentified0");
                
                // Identification Qualifier and Rank
                specimen.Qualifier = identificationMap.GetEncodedString("IdeQualifier_tab");
                if (string.Equals(identificationMap.GetEncodedString("IdeQualifierRank_tab"), "Genus", StringComparison.OrdinalIgnoreCase))
                    specimen.QualifierRank = QualifierRankType.Genus;
                else if (string.Equals(identificationMap.GetEncodedString("IdeQualifierRank_tab"), "species", StringComparison.OrdinalIgnoreCase))
                    specimen.QualifierRank = QualifierRankType.Species;

                // Taxonomy
                var taxonomyMap = identificationMap.GetMap("taxa");
                specimen.Taxonomy = _taxonomyFactory.Make(taxonomyMap);

                if (taxonomyMap != null)
                {
                    // Scientific Name
                    specimen.ScientificName = new[]
                    {
                        specimen.QualifierRank != QualifierRankType.Genus ? null : specimen.Qualifier,
                        specimen.Taxonomy.Genus,
                        string.IsNullOrWhiteSpace(specimen.Taxonomy.Subgenus)
                            ? null
                            : string.Format("({0})", specimen.Taxonomy.Subgenus),
                        specimen.QualifierRank != QualifierRankType.Species ? null : specimen.Qualifier,
                        specimen.Taxonomy.Species,
                        specimen.Taxonomy.Subspecies,
                        specimen.Taxonomy.Author
                    }.Concatenate(" ");

                    // Species profile Relationship
                    var relatedSpeciesMaps = taxonomyMap.GetMaps("relatedspecies");
                    specimen.RelatedSpeciesIds.AddRange(relatedSpeciesMaps
                        .Where(x => x != null && x.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuSpeciesQueryString))
                        .Select(x => string.Format("species/{0}", x.GetEncodedString("irn"))));
                }
            }

            // Collection Event
            var collectionEventMap = map.GetMap("colevent");
            if (collectionEventMap != null)
            {
                specimen.ExpeditionName = collectionEventMap.GetEncodedString("ExpExpeditionName");
                specimen.CollectionEventCode = collectionEventMap.GetEncodedString("ColCollectionEventCode");
                specimen.SamplingMethod = collectionEventMap.GetEncodedString("ColCollectionMethod");

                DateTime dateVisitedFrom;
                if (DateTime.TryParseExact(collectionEventMap.GetEncodedString("ColDateVisitedFrom"), "dd/MM/yyyy", new CultureInfo("en-AU"), DateTimeStyles.None, out dateVisitedFrom))
                {
                    TimeSpan timeVisitedFrom;
                    if (TimeSpan.TryParseExact(collectionEventMap.GetEncodedString("ColTimeVisitedFrom"), @"hh\:mm", new CultureInfo("en-AU"), out timeVisitedFrom))
                    {
                        dateVisitedFrom += timeVisitedFrom;
                    }

                    specimen.DateVisitedFrom = dateVisitedFrom;
                }

                DateTime dateVisitedTo;
                if (DateTime.TryParseExact(collectionEventMap.GetEncodedString("ColDateVisitedTo"), "dd/MM/yyyy", new CultureInfo("en-AU"), DateTimeStyles.None, out dateVisitedTo))
                {
                    TimeSpan timeVisitedTo;
                    if (TimeSpan.TryParseExact(collectionEventMap.GetEncodedString("ColTimeVisitedTo"), @"hh\:mm", new CultureInfo("en-AU"), out timeVisitedTo))
                    {
                        dateVisitedTo += timeVisitedTo;
                    }

                    specimen.DateVisitedTo = dateVisitedTo;
                }

                specimen.DepthTo = collectionEventMap.GetEncodedString("AquDepthToMet");
                specimen.DepthFrom = collectionEventMap.GetEncodedString("AquDepthFromMet");

                specimen.CollectedBy = collectionEventMap.GetMaps("collectors").Where(x => x != null).Select(x => _partiesNameFactory.Make(x)).Concatenate(", ");
            }

            // Sites
            var siteMap = map.GetMap("site");
            if (siteMap == null && collectionEventMap != null)
                siteMap = collectionEventMap.GetMap("site");

            if (siteMap != null)
            {
                // Site Code
                specimen.SiteCode = new[]
                {
                    siteMap.GetEncodedString("SitSiteCode"),
                    siteMap.GetEncodedString("SitSiteNumber")
                }.Concatenate("");

                // Locality
                var geoMap = siteMap.GetMaps("geo").FirstOrDefault();
                if (geoMap != null)
                {
                    specimen.Ocean = geoMap.GetEncodedString("LocOcean_tab");
                    specimen.Continent = geoMap.GetEncodedString("LocContinent_tab");
                    specimen.Country = geoMap.GetEncodedString("LocCountry_tab");
                    specimen.State = geoMap.GetEncodedString("LocProvinceStateTerritory_tab");
                    specimen.District = geoMap.GetEncodedString("LocDistrictCountyShire_tab");
                    specimen.Town = geoMap.GetEncodedString("LocTownship_tab");
                    specimen.NearestNamedPlace = geoMap.GetEncodedString("LocNearestNamedPlace_tab");
                }

                specimen.PreciseLocation = siteMap.GetEncodedString("LocPreciseLocation");
                specimen.MinimumElevation = siteMap.GetEncodedString("LocElevationASLFromMt");
                specimen.MaximumElevation = siteMap.GetEncodedString("LocElevationASLToMt");

                // Lat/Long
                var latlongMap = siteMap.GetMaps("latlong").FirstOrDefault();
                if (latlongMap != null)
                {
                    var decimalLatitude = (object[])latlongMap["LatLatitudeDecimal_nesttab"];
                    if (decimalLatitude != null)
                        specimen.Latitude = decimalLatitude.Where(x => x != null).FirstOrDefault() as string;

                    var decimalLongitude = ((object[])latlongMap["LatLongitudeDecimal_nesttab"]);
                    if (decimalLongitude != null)
                        specimen.Longitude = decimalLongitude.Where(x => x != null).FirstOrDefault() as string;

                    specimen.GeodeticDatum = (string.IsNullOrWhiteSpace(latlongMap.GetEncodedString("LatDatum_tab"))) ? "WGS84" : latlongMap.GetEncodedString("LatDatum_tab");
                    specimen.SiteRadius = latlongMap.GetEncodedString("LatRadiusNumeric_tab");
                    specimen.GeoreferenceBy = _partiesNameFactory.Make(latlongMap.GetMap("determinedBy"));

                    DateTime georeferenceDate;
                    if (DateTime.TryParseExact(latlongMap.GetEncodedString("LatDetDate0"), "dd/MM/yyyy", new CultureInfo("en-AU"), DateTimeStyles.None, out georeferenceDate))
                        specimen.GeoreferenceDate = georeferenceDate.ToString("s");

                    specimen.GeoreferenceProtocol = latlongMap.GetEncodedString("LatLatLongDetermination_tab");
                    specimen.GeoreferenceSource = latlongMap.GetEncodedString("LatDetSource_tab");
                }

                // Geology site fields
                if (!string.Equals(specimen.Discipline, "Tektites", StringComparison.OrdinalIgnoreCase) && !string.Equals(specimen.Discipline, "Meteorites", StringComparison.OrdinalIgnoreCase))
                {
                    specimen.GeologyEra = siteMap.GetEncodedString("EraEra");
                    specimen.GeologyPeriod = siteMap.GetEncodedString("EraAge1");
                    specimen.GeologyEpoch = siteMap.GetEncodedString("EraAge2");
                    specimen.GeologyStage = siteMap.GetEncodedString("EraMvStage");
                    specimen.GeologyGroup = siteMap.GetEncodedStrings("EraMvGroup_tab").Concatenate(", ");
                    specimen.GeologyFormation = siteMap.GetEncodedStrings("EraMvRockUnit_tab").Concatenate(", ");
                    specimen.GeologyMember = siteMap.GetEncodedStrings("EraMvMember_tab").Concatenate(", ");
                    specimen.GeologyRockType = siteMap.GetEncodedStrings("EraLithology_tab").Concatenate(", ");
                }
            }
                
            // Discipline specific fields
            // Palaeontology
            specimen.PalaeontologyDateCollectedFrom = map.GetEncodedString("LocDateCollectedFrom");
            specimen.PalaeontologyDateCollectedTo = map.GetEncodedString("LocDateCollectedTo");
            
            // Geology
            specimen.MineralogySpecies = map.GetEncodedString("MinSpecies");
            specimen.MineralogyVariety = map.GetEncodedString("MinVariety");
            specimen.MineralogyGroup = map.GetEncodedString("MinGroup");
            specimen.MineralogyClass = map.GetEncodedString("MinClass");
            specimen.MineralogyAssociatedMatrix = map.GetEncodedString("MinAssociatedMatrix");
            specimen.MineralogyType = map.GetEncodedString("MinType");
            specimen.MineralogyTypeOfType = map.GetEncodedString("MinTypeType");
            
            specimen.MeteoritesName = map.GetEncodedString("MetName");
            specimen.MeteoritesClass = map.GetEncodedString("MetClass");
            specimen.MeteoritesGroup = map.GetEncodedString("MetGroup");
            specimen.MeteoritesType = map.GetEncodedString("MetType");
            specimen.MeteoritesMinerals = map.GetEncodedString("MetMainMineralsPresent");
            specimen.MeteoritesSpecimenWeight = map.GetEncodedString("MetSpecimenWeight");
            specimen.MeteoritesTotalWeight = map.GetEncodedString("MetTotalWeight");
            specimen.MeteoritesDateFell = map.GetEncodedString("MetDateSpecimenFell");
            specimen.MeteoritesDateFound = map.GetEncodedString("MetDateSpecimenFound");
            
            specimen.TektitesName = map.GetEncodedString("TekName");
            specimen.TektitesClassification = map.GetEncodedString("TekClassification");
            specimen.TektitesShape = map.GetEncodedString("TekShape");
            specimen.TektitesLocalStrewnfield = map.GetEncodedString("TekLocalStrewnfield");
            specimen.TektitesGlobalStrewnfield = map.GetEncodedString("TekGlobalStrewnfield");

            specimen.PetrologyRockClass = map.GetEncodedString("RocClass");
            specimen.PetrologyRockGroup = map.GetEncodedString("RocGroup");
            specimen.PetrologyRockName = map.GetEncodedString("RocRockName");
            specimen.PetrologyRockDescription = map.GetEncodedString("RocRockDescription");

            // Media
            specimen.Media = _mediaFactory.Make(map.GetMaps("media"));

            var thumbnail = specimen.Media.FirstOrDefault(x => x is ImageMedia) as ImageMedia;
            if (thumbnail != null)
                specimen.ThumbnailUri = thumbnail.Thumbnail.Uri;

            // Relationships

            // Related items/specimens (directly related)
            foreach (var relatedItemSpecimen in map.GetMaps("relateditemspecimens").Where(x => x != null && !string.IsNullOrWhiteSpace(x.GetEncodedString("irn"))))
            {
                if (relatedItemSpecimen.GetEncodedStrings("MdaDataSets_tab").Contains(Constants.ImuItemQueryString))
                    specimen.RelatedItemIds.Add(string.Format("items/{0}", relatedItemSpecimen.GetEncodedString("irn")));
                if (relatedItemSpecimen.GetEncodedStrings("MdaDataSets_tab").Contains(Constants.ImuSpecimenQueryString))
                    specimen.RelatedSpecimenIds.Add(string.Format("specimens/{0}", relatedItemSpecimen.GetEncodedString("irn")));
            }
            // Physically attached
            var attachedItemSpecimenMap = map.GetMap("attacheditemspecimens");
            if (attachedItemSpecimenMap != null)
            {
                if (attachedItemSpecimenMap.GetEncodedStrings("MdaDataSets_tab").Contains(Constants.ImuItemQueryString))
                    specimen.RelatedItemIds.Add(string.Format("items/{0}", attachedItemSpecimenMap.GetEncodedString("irn")));
                if (attachedItemSpecimenMap.GetEncodedStrings("MdaDataSets_tab").Contains(Constants.ImuSpecimenQueryString))
                    specimen.RelatedSpecimenIds.Add(string.Format("specimens/{0}", attachedItemSpecimenMap.GetEncodedString("irn")));
            }
            // Parent record
            var parentItemSpecimenMap = map.GetMap("parentitemspecimens");
            if (parentItemSpecimenMap != null)
            {
                if (parentItemSpecimenMap.GetEncodedStrings("MdaDataSets_tab").Contains(Constants.ImuItemQueryString))
                    specimen.RelatedItemIds.Add(string.Format("items/{0}", parentItemSpecimenMap.GetEncodedString("irn")));
                if (parentItemSpecimenMap.GetEncodedStrings("MdaDataSets_tab").Contains(Constants.ImuSpecimenQueryString))
                    specimen.RelatedSpecimenIds.Add(string.Format("specimens/{0}", parentItemSpecimenMap.GetEncodedString("irn")));
            }

            // Related articles/species (direct attached)
            var relatedArticleSpeciesMap = map.GetMaps("relatedarticlespecies");
            if (relatedArticleSpeciesMap != null)
            {
                specimen.RelatedArticleIds.AddRangeUnique(relatedArticleSpeciesMap
                    .Where(x => x != null && x.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuArticleQueryString))
                    .Select(x => string.Format("articles/{0}", x.GetEncodedString("irn"))));

                specimen.RelatedSpeciesIds.AddRangeUnique(relatedArticleSpeciesMap
                    .Where(x => x != null && x.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuSpeciesQueryString))
                    .Select(x => string.Format("species/{0}", x.GetEncodedString("irn"))));
            }

            // Related articles (via party relationship)
            var relatedPartyArticlesMap = map.GetMaps("relatedpartyarticles");
            if (relatedPartyArticlesMap != null)
            {
                specimen.RelatedArticleIds.AddRangeUnique(relatedPartyArticlesMap
                        .Where(x => x != null)
                        .SelectMany(x => x.GetMaps("relatedarticles"))
                        .Where(x => x != null && x.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuArticleQueryString))
                        .Select(x => string.Format("articles/{0}", x.GetEncodedString("irn"))));
            }

            // Related articles (via sites relationship)
            var relatedSiteArticlesMap = map.GetMap("relatedsitearticles");
            if (relatedSiteArticlesMap != null)
            {
                specimen.RelatedArticleIds.AddRangeUnique(relatedSiteArticlesMap
                        .GetMaps("relatedarticles")
                        .Where(x => x != null && x.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuArticleQueryString))
                        .Select(x => string.Format("articles/{0}", x.GetEncodedString("irn"))));
            }

            // Related articles (via collection event relationship)
            var relatedCollectionEventArticlesMap = map.GetMap("relatedcolleventarticles");
            if (relatedCollectionEventArticlesMap != null)
            {
                specimen.RelatedArticleIds.AddRangeUnique(relatedCollectionEventArticlesMap
                        .GetMaps("relatedarticles")
                        .Where(x => x != null && x.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuArticleQueryString))
                        .Select(x => string.Format("articles/{0}", x.GetEncodedString("irn"))));
            }

            // Build summary
            if(specimen.Taxonomy != null)
                specimen.Summary = new[]
                    {
                        specimen.Taxonomy.CommonName,
                        new[] {
                            specimen.Taxonomy.Phylum,
                            specimen.Taxonomy.Class,
                            specimen.Taxonomy.Order,
                            specimen.Taxonomy.Family
                        }.Concatenate(" ")
                    }.Concatenate(Environment.NewLine);

            // Build Associated date
            if (specimen.DateVisitedFrom.HasValue)
            {
                var yearSpan = NaturalDateConverter.ConvertToYearSpan(specimen.DateVisitedFrom.Value.Year.ToString(CultureInfo.InvariantCulture));
                if(!string.IsNullOrWhiteSpace(yearSpan))
                    specimen.AssociatedDate = yearSpan;
            }

            // Display Title
            if (string.Equals(specimen.Discipline, "Tektites", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrWhiteSpace(specimen.TektitesName) && !string.IsNullOrWhiteSpace(specimen.TektitesClassification))
                    specimen.DisplayTitle = string.Format("{0} {1}", specimen.TektitesName, specimen.TektitesClassification);
                else if (!string.IsNullOrWhiteSpace(specimen.TektitesName))
                    specimen.DisplayTitle = specimen.TektitesName;
                else
                    specimen.DisplayTitle = "Tektite";
            }
            else if (string.Equals(specimen.Discipline, "Meteorites", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrWhiteSpace(specimen.MeteoritesName))
                    specimen.DisplayTitle = specimen.MeteoritesName;
                else
                    specimen.DisplayTitle = "Meteorite";
            }
            else if (string.Equals(specimen.Discipline, "Petrology", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrWhiteSpace(specimen.PetrologyRockName))
                    specimen.DisplayTitle = specimen.PetrologyRockName;
                else
                    specimen.DisplayTitle = "Petrology";
            }
            else if (!string.IsNullOrWhiteSpace(specimen.ScientificName))
                specimen.DisplayTitle = string.Format("<em>{0}</em>", specimen.ScientificName);
            else if (!string.IsNullOrWhiteSpace(specimen.ObjectName))
                specimen.DisplayTitle = specimen.ObjectName;
            else
                specimen.DisplayTitle = string.Format("Specimen {0}", specimen.RegistrationNumber);

            stopwatch.Stop();
            _log.Trace("Completed specimen creation for catalog record with irn {0}, elapsed time {1} ms", map.GetEncodedString("irn"), stopwatch.ElapsedMilliseconds);
            
            return specimen;
        }

        public void UpdateDocument(Specimen newDocument, Specimen existingDocument, IDocumentSession documentSession)
        {
            // Map over existing document
            Mapper.Map(newDocument, existingDocument);
        }
    }
}