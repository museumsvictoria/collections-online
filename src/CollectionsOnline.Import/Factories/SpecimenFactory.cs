using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using AutoMapper;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Factories;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Utilities;
using ImageResizer;
using IMu;

namespace CollectionsOnline.Import.Factories
{
    public class SpecimenFactory : IEmuAggregateRootFactory<Specimen>
    {
        private readonly ISlugFactory _slugFactory;
        private readonly IMediaHelper _mediaHelper;
        private readonly IPartiesNameFactory _partiesNameFactory;
        private readonly ITaxonomyFactory _taxonomyFactory;

        public SpecimenFactory(
            ISlugFactory slugFactory,
            IMediaHelper mediaHelper,
            IPartiesNameFactory partiesNameFactory,
            ITaxonomyFactory taxonomyFactory)
        {
            _slugFactory = slugFactory;
            _mediaHelper = mediaHelper;
            _partiesNameFactory = partiesNameFactory;
            _taxonomyFactory = taxonomyFactory;
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
                        "colevent=ColCollectionEventRef.(ExpExpeditionName,ColCollectionEventCode,ColCollectionMethod,ColDateVisitedTo,ColTimeVisitedTo,AquDepthToMet,AquDepthFromMet,site=ColSiteRef.(SitSiteCode,SitSiteNumber,EraEra,EraAge1,EraAge2,EraMvStage,EraMvGroup_tab,EraMvRockUnit_tab,EraMvMember_tab,EraLithology_tab,geo=[LocOcean_tab,LocContinent_tab,LocCountry_tab,LocProvinceStateTerritory_tab,LocDistrictCountyShire_tab,LocTownship_tab,LocNearestNamedPlace_tab],LocPreciseLocation,LocElevationASLFromMt,LocElevationASLToMt,latlong=[LatCentroidLongitudeDec_tab,LatCentroidLatitudeDec_tab,LatDatum_tab,determinedBy=LatDeterminedByRef_tab.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName),LatDetDate0,LatLatLongDetermination_tab,LatDetSource_tab]),collectors=ColParticipantRef_tab.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName))",
                        "SpeNoSpecimens",
                        "SpeSex_tab",
                        "SpeStageAge_tab",
                        "preparations=[StrSpecimenNature_tab,StrSpecimenForm_tab,StrFixativeTreatment_tab,StrStorageMedium_tab]",
                        "DarYearCollected",
                        "DarMonthCollected",
                        "DarDayCollected",
                        "site=SitSiteRef.(SitSiteCode,SitSiteNumber,EraEra,EraAge1,EraAge2,EraMvStage,EraMvGroup_tab,EraMvRockUnit_tab,EraMvMember_tab,EraLithology_tab,geo=[LocOcean_tab,LocContinent_tab,LocCountry_tab,LocProvinceStateTerritory_tab,LocDistrictCountyShire_tab,LocTownship_tab,LocNearestNamedPlace_tab],LocPreciseLocation,LocElevationASLFromMt,LocElevationASLToMt,latlong=[LatCentroidLongitudeDec_tab,LatCentroidLatitudeDec_tab,LatDatum_tab,determinedBy=LatDeterminedByRef_tab.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName),LatDetDate0,LatLatLongDetermination_tab,LatDetSource_tab])",
                        "identifications=[IdeTypeStatus_tab,IdeCurrentNameLocal_tab,identifiers=IdeIdentifiedByRef_nesttab.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName),IdeDateIdentified0,IdeAccuracyNotes_tab,IdeQualifier_tab,taxa=TaxTaxonomyRef_tab.(irn,ClaKingdom,ClaPhylum,ClaSubphylum,ClaSuperclass,ClaClass,ClaSubclass,ClaSuperorder,ClaOrder,ClaSuborder,ClaInfraorder,ClaSuperfamily,ClaFamily,ClaSubfamily,ClaGenus,ClaSubgenus,ClaSpecies,ClaSubspecies,AutAuthorString,ClaApplicableCode,comname=[ComName_tab,ComStatus_tab])]",
                        "media=MulMultiMediaRef_tab.(irn,MulTitle,MulMimeType,MulDescription,MulCreator_tab,MdaDataSets_tab,MdaElement_tab,MdaQualifier_tab,MdaFreeText_tab,ChaRepository_tab,DetAlternateText,AdmPublishWebNoPassword,AdmDateModified,AdmTimeModified)",
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
                        "related=ColRelatedRecordsRef_tab.(irn,MdaDataSets_tab)",
                        "attached=ColPhysicallyAttachedToRef.(irn,MdaDataSets_tab)",
                        "accession=AccAccessionLotRef.(AcqAcquisitionMethod,AcqDateReceived,AcqDateOwnership,AcqCreditLine,source=[name=AcqSourceRef_tab.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName),AcqSourceRole_tab])",
                        "RigText0",
                        "SpeNoSpecimens",
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
            var specimen = new Specimen();

            specimen.Id = "specimens/" + map.GetString("irn");

            specimen.IsHidden = string.Equals(map.GetString("AdmPublishWebNoPassword"), "no", StringComparison.OrdinalIgnoreCase);

            specimen.DateModified = DateTime.ParseExact(
                string.Format("{0} {1}", map.GetString("AdmDateModified"), map.GetString("AdmTimeModified")),
                "dd/MM/yyyy HH:mm",
                new CultureInfo("en-AU"));

            specimen.Category = map.GetString("ColCategory");
            specimen.ScientificGroup = map.GetString("ColScientificGroup");
            specimen.Discipline = map.GetString("ColDiscipline");
            specimen.RegistrationNumber = map["ColRegPart"] != null
                             ? string.Format("{0}{1}.{2}", map["ColRegPrefix"], map["ColRegNumber"], map["ColRegPart"])
                             : string.Format("{0}{1}", map["ColRegPrefix"], map["ColRegNumber"]);
            specimen.CollectionNames = map.GetStrings("ColCollectionName_tab") ?? new string[] { };
            specimen.Type = map.GetString("ColTypeOfItem");

            // Classifications
            if (map.GetString("ClaPrimaryClassification") != null && !map.GetString("ClaPrimaryClassification").Contains("to be classified", StringComparison.OrdinalIgnoreCase))
                specimen.PrimaryClassification = map.GetString("ClaPrimaryClassification").ToSentenceCase();
            if (map.GetString("ClaSecondaryClassification") != null && !map.GetString("ClaSecondaryClassification").Contains("to be classified", StringComparison.OrdinalIgnoreCase))
                specimen.SecondaryClassification = map.GetString("ClaSecondaryClassification").ToSentenceCase();
            if (map.GetString("ClaTertiaryClassification") != null && !map.GetString("ClaTertiaryClassification").Contains("to be classified", StringComparison.OrdinalIgnoreCase))
                specimen.TertiaryClassification = map.GetString("ClaTertiaryClassification").ToSentenceCase();

            specimen.ObjectName = map.GetString("ClaObjectName");
            specimen.ObjectSummary = map.GetString("ClaObjectSummary");
            specimen.IsdDescriptionOfContent = map.GetString("Con1Description");
            specimen.Significance = map.GetString("SubHistoryTechSignificance");

            // Tags
            if (map.GetStrings("SubSubjects_tab") != null)
                specimen.Tags = map.GetStrings("SubSubjects_tab").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => _slugFactory.MakeSlug(x)).ToList();

            // Collection plans
            specimen.CollectionPlans = map.GetStrings("SubThemes_tab").Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

            // Associations
            foreach (var associationMap in map.GetMaps("associations"))
            {
                var association = new Association();

                association.Type = associationMap.GetString("AssAssociationType_tab");
                association.Name = _partiesNameFactory.Make(associationMap.GetMap("party"));
                association.Date = associationMap.GetString("AssAssociationDate_tab");
                association.Comments = associationMap.GetString("AssAssociationComments0");

                var place = new[]
                {
                    associationMap.GetString("AssAssociationStreetAddress_tab"),
                    associationMap.GetString("AssAssociationLocality_tab"),
                    associationMap.GetString("AssAssociationRegion_tab").Remove(new[] { "greater", "district" }),
                    associationMap.GetString("AssAssociationState_tab"),
                    associationMap.GetString("AssAssociationCountry_tab")
                }.Distinct();

                association.Place = place.Concatenate(", ");
                association.PlaceKey = place.Concatenate("-").ToLower();

                specimen.Associations.Add(association);
            }

            // Related items/specimens
            foreach (var related in map.GetMaps("related").Where(x => x != null))
            {
                if (related.GetStrings("MdaDataSets_tab").Contains(Constants.ImuItemQueryString))
                    specimen.RelatedIds.Add("items/" + related.GetString("irn"));
                if (related.GetStrings("MdaDataSets_tab").Contains(Constants.ImuSpecimenQueryString))
                    specimen.RelatedIds.Add("specimens/" + related.GetString("irn"));
            }

            // Physically attached
            var attachedMap = map.GetMap("attached");
            if (attachedMap != null)
            {
                if (attachedMap.GetStrings("MdaDataSets_tab").Contains(Constants.ImuItemQueryString))
                    specimen.AttachedIds.Add("items/" + attachedMap.GetString("irn"));
                if (attachedMap.GetStrings("MdaDataSets_tab").Contains(Constants.ImuSpecimenQueryString))
                    specimen.AttachedIds.Add("specimens/" + attachedMap.GetString("irn"));
            }

            // Acquisition information
            var accessionMap = map.GetMap("accession");
            if (accessionMap != null)
            {
                var method = accessionMap.GetString("AcqAcquisitionMethod");

                if (!string.IsNullOrWhiteSpace(method))
                {
                    var sources = accessionMap.GetMaps("source")
                    .Where(x => string.IsNullOrWhiteSpace(x.GetString("AcqSourceRole_tab")) ||
                        (!x.GetString("AcqSourceRole_tab").Contains("confindential", StringComparison.OrdinalIgnoreCase) &&
                         !x.GetString("AcqSourceRole_tab").Contains("contact", StringComparison.OrdinalIgnoreCase) &&
                         !x.GetString("AcqSourceRole_tab").Contains("vendor", StringComparison.OrdinalIgnoreCase)))
                    .Select(x => _partiesNameFactory.Make(x.GetMap("name"))).ToList();

                    if (sources.Any())
                    {
                        if (!string.IsNullOrWhiteSpace(accessionMap.GetString("AcqDateReceived")))
                            sources.Add(accessionMap.GetString("AcqDateReceived"));
                        else if (!string.IsNullOrWhiteSpace(accessionMap.GetString("AcqDateOwnership")))
                            sources.Add(accessionMap.GetString("AcqDateOwnership"));

                        specimen.AcquisitionInformation = string.Format("{0} from {1}", method, sources.Concatenate(", "));
                    }
                    else
                    {
                        specimen.AcquisitionInformation = method;
                    }
                }

                var rights = map.GetStrings("RigText0").FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(accessionMap.GetString("AcqCreditLine")))
                    specimen.Acknowledgement = accessionMap.GetString("AcqCreditLine");
                else if (!string.IsNullOrWhiteSpace(rights))
                    specimen.Acknowledgement = rights;
            }

            // Specimen form
            specimen.SpecimenForm = map
                .GetMaps("preparations")
                .Where(x => x != null)
                .Select(x => x.GetString("StrSpecimenForm_tab"))
                .Concatenate(", ");

            // Discipline specific fields
            // Palaeontology
            specimen.PalaeontologyDateCollectedFrom = map.GetString("LocDateCollectedFrom");
            specimen.PalaeontologyDateCollectedTo = map.GetString("LocDateCollectedTo");
            
            // Mineralogy
            specimen.MineralogySpecies = map.GetString("MinSpecies");
            specimen.MineralogyVariety = map.GetString("MinVariety");
            specimen.MineralogyGroup = map.GetString("MinGroup");
            specimen.MineralogyClass = map.GetString("MinClass");
            specimen.MineralogyAssociatedMatrix = map.GetString("MinAssociatedMatrix");
            specimen.MineralogyIsType = map.GetString("MinType");
            specimen.MineralogyType = map.GetString("MinTypeType");
            
            // Meteorites
            specimen.MeteoritesName = map.GetString("MetName");
            specimen.MeteoritesClass = map.GetString("MetClass");
            specimen.MeteoritesGroup = map.GetString("MetGroup");
            specimen.MeteoritesType = map.GetString("MetType");
            specimen.MeteoritesMinerals = map.GetString("MetMainMineralsPresent");
            specimen.MeteoritesSpecimenWeight = map.GetString("MetSpecimenWeight");
            specimen.MeteoritesTotalWeight = map.GetString("MetTotalWeight");
            specimen.MeteoritesDateFell = map.GetString("MetDateSpecimenFell");
            specimen.MeteoritesDateFound = map.GetString("MetDateSpecimenFound");
            
            // Tektites
            specimen.TektitesName = map.GetString("TekName");
            specimen.TektitesClassification = map.GetString("TekClassification");
            specimen.TektitesShape = map.GetString("TekShape");
            specimen.TektitesLocalStrewnfield = map.GetString("TekLocalStrewnfield");
            specimen.TektitesGlobalStrewnfield = map.GetString("TekGlobalStrewnfield");

            // Petrology
            specimen.PetrologyRockClass = map.GetString("RocClass");
            specimen.PetrologyRockGroup = map.GetString("RocGroup");
            specimen.PetrologyRockName = map.GetString("RocRockName");
            specimen.PetrologyRockDescription = map.GetString("RocRockDescription");

            #region DwC Fields

            #region Record-level Terms

            //dcterms:type
            if (map.GetString("ColTypeOfItem") == "Specimen")
                specimen.DctermsType = "PhysicalObject";

            //dcterms:language
            specimen.DctermsLanguage = "en";

            //dcterms:rights
            specimen.DctermsRights = "Dataset licensed under Creative Commons Attribution (CC-BY) 3.0 Australian license. Use of data of individual specimen occurrences does not require attribution on a per record basis.";

            //dcterms:rightsHolder
            specimen.DctermsRightsHolder = "Museum Victoria";

            //institutionID
            specimen.InstitutionId = "NMV";

            //collectionID
            specimen.CollectionId = "urn:lsid:biocol.org:col:34978";

            //datasetID
            specimen.DatasetId = map.GetString("ColDiscipline");

            //institutionCode
            specimen.InstitutionCode = "NMV";

            //collectionCode
            specimen.CollectionCode = map.GetString("ColDiscipline");

            //ownerInstitutionCode
            specimen.OwnerInstitutionCode = "NMV";

            //datasetName
            var colevent = map.GetMap("colevent");
            if (colevent != null)
                specimen.DatasetName = colevent.GetString("ExpExpeditionName");

            //ownerInstitutionCode
            specimen.OwnerInstitutionCode = "NMV";

            //basisOfRecord
            if (map.GetString("ColTypeOfItem") == "Specimen")
                specimen.BasisOfRecord = "PreservedSpecimen";

            #endregion

            #region Occurrence

            //occurrenceID                                        
            specimen.OccurrenceId = (string.IsNullOrWhiteSpace(map.GetString("ColRegPart")))
                                      ? string.Format(
                                          "urn:lsid:ozcam.taxonomy.org.au:NMV:{0}:PreservedSpecimen:{1}{2}",
                                          map.GetString("ColDiscipline"), map.GetString("ColRegPrefix"),
                                          map.GetString("ColRegNumber"))
                                      : string.Format(
                                          "urn:lsid:ozcam.taxonomy.org.au:NMV:{0}:PreservedSpecimen:{1}{2}-{3}",
                                          map.GetString("ColDiscipline"), map.GetString("ColRegPrefix"),
                                          map.GetString("ColRegNumber"), map.GetString("ColRegPart"));

            //catalogNumber
            specimen.CatalogNumber = (string.IsNullOrWhiteSpace(map.GetString("ColRegPart")))
                                      ? string.Format("{0}{1}", map.GetString("ColRegPrefix"), map.GetString("ColRegNumber"))
                                      : string.Format("{0}{1}-{2}", map.GetString("ColRegPrefix"), map.GetString("ColRegNumber"), map.GetString("ColRegPart"));

            //recordedBy
            if (colevent != null && colevent.GetMaps("collectors") != null)
            {
                specimen.RecordedBy = colevent.GetMaps("collectors").Where(x => x != null).Select(x => _partiesNameFactory.Make(x)).Concatenate("; ");
            }

            //individualCount
            specimen.IndividualCount = map.GetString("SpeNoSpecimens");

            //sex
            specimen.Sex = map.GetStrings("SpeSex_tab").Concatenate("; ");

            //lifeStage
            specimen.LifeStage = map.GetStrings("SpeStageAge_tab").Concatenate("; ");

            //occurrenceStatus
            specimen.OccurrenceStatus = "present";

            //preparations
            foreach (var preparationMap in map.GetMaps("preparations"))
            {
                var preparation = new[]
                            {
                                new KeyValuePair<string, string>("specimenNature", preparationMap.GetString("StrSpecimenNature_tab")),
                                new KeyValuePair<string, string>("specimenForm", preparationMap.GetString("StrSpecimenForm_tab")),
                                new KeyValuePair<string, string>("fixativeTreatment", preparationMap.GetString("StrFixativeTreatment_tab")),
                                new KeyValuePair<string, string>("storageMedium", preparationMap.GetString("StrStorageMedium_tab"))
                            }
                    .Where(x => !string.IsNullOrWhiteSpace(x.Value))
                    .Select(x => string.Format("{0}={1}", x.Key, x.Value))
                    .Concatenate(",");

                if (specimen.Preparations != null)
                {
                    specimen.Preparations += ";" + preparation;
                }
                else
                {
                    specimen.Preparations = preparation;
                }
            }

            //associatedMedia
            foreach (var mediaMap in map.GetMaps("media").Where(x =>
                x != null &&
                string.Equals(x.GetString("AdmPublishWebNoPassword"), "yes", StringComparison.OrdinalIgnoreCase) &&
                x.GetString("MulMimeType") == "image" &&
                x.GetStrings("MdaDataSets_tab").Contains(Constants.ImuSpecimenQueryString)))
            {
                var irn = long.Parse(mediaMap.GetString("irn"));

                var url = PathFactory.MakeUrlPath(irn, FileFormatType.Jpg, "thumb");
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
                    specimen.Media.Add(new Media
                    {
                        Irn = irn,
                        DateModified =
                            DateTime.ParseExact(string.Format("{0} {1}", mediaMap.GetString("AdmDateModified"),
                                mediaMap.GetString("AdmTimeModified")),
                                "dd/MM/yyyy HH:mm", new CultureInfo("en-AU")),
                        Title = mediaMap.GetString("MulTitle"),
                        AlternateText = mediaMap.GetString("DetAlternateText"),
                        Type = mediaMap.GetString("MulMimeType"),
                        Url = url
                    });
                }
            }
            specimen.AssociatedMedia = specimen.Media.Select(x => x.Title).Concatenate("; ");

            #endregion

            #region Event

            //eventID
            if (colevent != null)
                specimen.EventId = colevent.GetString("ColCollectionEventCode");

            //samplingProtocol
            if (colevent != null)
                specimen.SamplingProtocol = colevent.GetString("ColCollectionMethod");

            //eventDate
            if (colevent != null && !string.IsNullOrWhiteSpace(colevent.GetString("ColDateVisitedTo")))
            {
                DateTime eventDate;
                if (DateTime.TryParseExact(colevent.GetString("ColDateVisitedTo"), "dd/MM/yyyy", new CultureInfo("en-AU"), DateTimeStyles.None, out eventDate))
                    specimen.EventDate = eventDate.ToString("s");
            }

            //eventTime
            if (colevent != null)
                specimen.EventTime = colevent.GetString("ColTimeVisitedTo");

            //year
            specimen.Year = map.GetString("DarYearCollected");

            //month
            specimen.Month = map.GetString("DarMonthCollected");

            //day
            specimen.Day = map.GetString("DarDayCollected");

            //verbatimEventDate
            if (colevent != null)
                specimen.VerbatimEventDate = colevent.GetString("ColDateVisitedTo");

            //fieldNumber
            if (colevent != null)
                specimen.FieldNumber = colevent.GetString("ColCollectionEventCode");

            #endregion

            #region Location

            var site = map.GetMap("site");

            if (site == null && colevent != null)
                site = colevent.GetMap("site");

            //locationID
            //locality
            //verbatimLocality
            //minimumElevationInMeters
            //maximumElevationInMeters
            if (site != null)
            {
                if (!string.IsNullOrWhiteSpace(site.GetString("SitSiteCode")) || !string.IsNullOrWhiteSpace(site.GetString("SitSiteNumber")))
                    specimen.LocationID = string.Format("{0}{1}", site.GetString("SitSiteCode"), site.GetString("SitSiteNumber"));

                specimen.Locality = site.GetString("LocPreciseLocation");
                specimen.VerbatimLocality = site.GetString("LocPreciseLocation");
                specimen.MinimumElevationInMeters = site.GetString("LocElevationASLFromMt");
                specimen.MaximumElevationInMeters = site.GetString("LocElevationASLToMt");

                var geo = site.GetMaps("geo").FirstOrDefault();

                //higherGeography
                //continent
                //waterBody
                //country
                //stateProvince
                //county
                //municipality
                if (geo != null)
                {
                    specimen.HigherGeography = new[]
                                {
                                    geo.GetString("LocOcean_tab"),
                                    geo.GetString("LocContinent_tab"),
                                    geo.GetString("LocCountry_tab"),
                                    geo.GetString("LocProvinceStateTerritory_tab")
                                }.Concatenate(", ");

                    specimen.Continent = geo.GetString("LocContinent_tab");
                    specimen.WaterBody = geo.GetString("LocOcean_tab");
                    specimen.Country = geo.GetString("LocCountry_tab");
                    specimen.StateProvince = geo.GetString("LocProvinceStateTerritory_tab");
                    specimen.County = geo.GetString("LocDistrictCountyShire_tab");
                    specimen.Municipality = geo.GetString("LocTownship_tab");
                    specimen.NearestNamedPlace = geo.GetString("LocNearestNamedPlace_tab");
                }

                var latlong = site.GetMaps("latlong").FirstOrDefault();

                //decimalLatitude
                //decimalLongitude
                //geodeticDatum
                //georeferencedBy
                //georeferencedDate
                //georeferenceProtocol
                //georeferenceSources
                if (latlong != null)
                {
                    specimen.DecimalLatitude = latlong.GetString("LatCentroidLatitudeDec_tab");
                    specimen.DecimalLongitude = latlong.GetString("LatCentroidLongitudeDec_tab");

                    specimen.GeodeticDatum = (string.IsNullOrWhiteSpace(latlong.GetString("LatDatum_tab"))) ? "WGS84" : latlong.GetString("LatDatum_tab");

                    specimen.GeoreferencedBy = _partiesNameFactory.Make(latlong.GetMap("determinedBy"));

                    DateTime georeferencedDate;
                    if (DateTime.TryParseExact(latlong.GetString("LatDetDate0"), "dd/MM/yyyy", new CultureInfo("en-AU"), DateTimeStyles.None, out georeferencedDate))
                        specimen.GeoreferencedDate = georeferencedDate.ToString("s");

                    specimen.GeoreferenceProtocol = latlong.GetString("LatLatLongDetermination_tab");
                    specimen.GeoreferenceSources = latlong.GetString("LatDetSource_tab");
                }

                // Geology site fields
                specimen.GeologyEra = site.GetString("EraEra");
                specimen.GeologyPeriod = site.GetString("EraAge1");
                specimen.GeologyEpoch = site.GetString("EraAge2");
                specimen.GeologyStage = site.GetString("EraMvStage");
                if (specimen.Discipline != "Tektites" && specimen.Discipline != "Meteorites")
                {
                    specimen.GeologyGroup = site.GetStrings("EraMvGroup_tab").Where(x => !string.IsNullOrWhiteSpace(x)).Concatenate(", ");
                    specimen.GeologyFormation = site.GetStrings("EraMvRockUnit_tab").Where(x => !string.IsNullOrWhiteSpace(x)).Concatenate(", ");
                    specimen.GeologyMember = site.GetStrings("EraMvMember_tab").Where(x => !string.IsNullOrWhiteSpace(x)).Concatenate(", ");
                    specimen.GeologyRockType = site.GetStrings("EraLithology_tab").Where(x => !string.IsNullOrWhiteSpace(x)).Concatenate(", ");
                }
            }

            //minimumDepthInMeters
            //maximumDepthInMeters
            if (colevent != null)
            {
                specimen.MinimumDepthInMeters = colevent.GetString("AquDepthFromMet");
                specimen.MaximumDepthInMeters = colevent.GetString("AquDepthToMet");
            }

            #endregion

            #region Identification

            var identification = map.GetMaps("identifications").FirstOrDefault(x => (x.GetString("IdeTypeStatus_tab") != null && Constants.TaxonomyTypeStatuses.Contains(x.GetString("IdeTypeStatus_tab").Trim().ToLower()))) ??
                                 map.GetMaps("identifications").FirstOrDefault(x => (x.GetString("IdeCurrentNameLocal_tab") != null && x.GetString("IdeCurrentNameLocal_tab").Trim().ToLower() == "yes"));

            if (identification != null)
            {
                //typeStatus
                specimen.TypeStatus = identification.GetString("IdeTypeStatus_tab");

                //identifiedBy
                if (identification.GetMaps("identifiers") != null)
                {
                    specimen.IdentifiedBy = identification.GetMaps("identifiers").Where(x => x != null).Select(x => _partiesNameFactory.Make(x)).Concatenate("; ");
                }

                //dateIdentified
                //identificationRemarks
                //identificationQualifier
                specimen.DateIdentified = identification.GetString("IdeDateIdentified0");
                specimen.IdentificationRemarks = identification.GetString("IdeAccuracyNotes_tab");
                specimen.IdentificationQualifier = identification.GetString("IdeQualifier_tab");

                specimen.Taxonomy = _taxonomyFactory.Make(identification.GetMap("taxa"));
            }

            #endregion

            #endregion

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
            var yearSpan = NaturalDateConverter.ConvertToYearSpan(specimen.Year);
            if (!string.IsNullOrWhiteSpace(yearSpan))
            {
                specimen.Year = yearSpan;
            }
            
            return specimen;
        }

        public void RegisterAutoMapperMap()
        {
            Mapper.CreateMap<Specimen, Specimen>()
                .ForMember(x => x.Id, options => options.Ignore());
        }
    }
}