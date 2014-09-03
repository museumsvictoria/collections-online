﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Factories;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Utilities;
using IMu;
using Raven.Abstractions.Extensions;

namespace CollectionsOnline.Import.Factories
{
    public class SpecimenFactory : IEmuAggregateRootFactory<Specimen>
    {
        private readonly ISlugFactory _slugFactory;
        private readonly IPartiesNameFactory _partiesNameFactory;
        private readonly ITaxonomyFactory _taxonomyFactory;
        private readonly IMediaFactory _mediaFactory;
        private readonly IAssociationFactory _associationFactory;

        public SpecimenFactory(
            ISlugFactory slugFactory,
            IPartiesNameFactory partiesNameFactory,
            ITaxonomyFactory taxonomyFactory,
            IMediaFactory mediaFactory,
            IAssociationFactory associationFactory)
        {
            _slugFactory = slugFactory;
            _partiesNameFactory = partiesNameFactory;
            _taxonomyFactory = taxonomyFactory;
            _mediaFactory = mediaFactory;
            _associationFactory = associationFactory;
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
                        "identifications=[IdeTypeStatus_tab,IdeCurrentNameLocal_tab,identifiers=IdeIdentifiedByRef_nesttab.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName),IdeDateIdentified0,IdeQualifier_tab,IdeQualifierRank_tab,taxa=TaxTaxonomyRef_tab.(irn,ClaKingdom,ClaPhylum,ClaSubphylum,ClaSuperclass,ClaClass,ClaSubclass,ClaSuperorder,ClaOrder,ClaSuborder,ClaInfraorder,ClaSuperfamily,ClaFamily,ClaSubfamily,ClaGenus,ClaSubgenus,ClaSpecies,ClaSubspecies,AutAuthorString,ClaApplicableCode,comname=[ComName_tab,ComStatus_tab])]",
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
                        "parent=ColParentRecordRef.(irn,MdaDataSets_tab)",
                        "accession=AccAccessionLotRef.(AcqAcquisitionMethod,AcqDateReceived,AcqDateOwnership,AcqCreditLine,source=[name=AcqSourceRef_tab.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName),AcqSourceRole_tab])",
                        "RigText0",
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
            specimen.Associations = _associationFactory.Make(map.GetMaps("associations"));

            // Relationships
            // TODO: Add import to check for these relationships
            // Related items/specimens
            foreach (var related in map.GetMaps("related").Where(x => x != null && !string.IsNullOrWhiteSpace(x.GetString("irn"))))
            {
                if (related.GetStrings("MdaDataSets_tab").Contains(Constants.ImuItemQueryString))
                    specimen.RelatedIds.Add(string.Format("items/{0}", related.GetString("irn")));
                if (related.GetStrings("MdaDataSets_tab").Contains(Constants.ImuSpecimenQueryString))
                    specimen.RelatedIds.Add(string.Format("specimens/{0}", related.GetString("irn")));
            }
            // Physically attached
            var attachedMap = map.GetMap("attached");
            if (attachedMap != null)
            {
                if (attachedMap.GetStrings("MdaDataSets_tab").Contains(Constants.ImuItemQueryString))
                    specimen.RelatedIds.Add(string.Format("items/{0}", attachedMap.GetString("irn")));
                if (attachedMap.GetStrings("MdaDataSets_tab").Contains(Constants.ImuSpecimenQueryString))
                    specimen.RelatedIds.Add(string.Format("specimens/{0}", attachedMap.GetString("irn")));
            }
            // Parent record
            var parentMap = map.GetMap("parent");
            if (parentMap != null)
            {
                if (parentMap.GetStrings("MdaDataSets_tab").Contains(Constants.ImuItemQueryString))
                    specimen.RelatedIds.Add(string.Format("items/{0}", parentMap.GetString("irn")));
                if (parentMap.GetStrings("MdaDataSets_tab").Contains(Constants.ImuSpecimenQueryString))
                    specimen.RelatedIds.Add(string.Format("specimens/{0}", parentMap.GetString("irn")));
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

            // Number Of Specimens
            specimen.NumberOfSpecimens = map.GetString("SpeNoSpecimens");
            // Clutch Size
            specimen.ClutchSize = map.GetString("BirTotalClutchSize");
            // Sex
            specimen.Sex = map.GetStrings("SpeSex_tab").Concatenate(", ");
            // Stage Or Age
            specimen.StageOrAge = map.GetStrings("SpeStageAge_tab").Concatenate(", ");
            // Storages
            specimen.Storages.AddRange(
                map.GetMaps("storage")
                    .Select(x => new Storage
                    {
                        Nature = x.GetString("StrSpecimenNature_tab"),
                        Form = x.GetString("StrSpecimenForm_tab"),
                        FixativeTreatment = x.GetString("StrFixativeTreatment_tab"),
                        Medium = x.GetString("StrStorageMedium_tab")
                    })
                    .Where(x => x != null));

            // Taxonomy
            // TODO: make factory method as code duplicated in ItemFactory
            var identificationMap = map.GetMaps("identifications").FirstOrDefault(x => (x.GetString("IdeTypeStatus_tab") != null && Constants.TaxonomyTypeStatuses.Contains(x.GetString("IdeTypeStatus_tab").Trim().ToLower()))) ??
                                    map.GetMaps("identifications").FirstOrDefault(x => (x.GetString("IdeCurrentNameLocal_tab") != null && x.GetString("IdeCurrentNameLocal_tab").Trim().ToLower() == "yes"));            
            if (identificationMap != null)
            {
                // Type Status
                specimen.TypeStatus = identificationMap.GetString("IdeTypeStatus_tab");
                // Identified By
                if (identificationMap.GetMaps("identifiers") != null)
                {
                    specimen.IdentifiedBy = identificationMap.GetMaps("identifiers").Where(x => x != null).Select(x => _partiesNameFactory.Make(x)).Concatenate("; ");
                }
                // Date Identified
                specimen.DateIdentified = identificationMap.GetString("IdeDateIdentified0");
                
                // Identification Qualifier and Rank
                specimen.Qualifier = identificationMap.GetString("IdeQualifier_tab");
                if (string.Equals(identificationMap.GetString("IdeQualifierRank_tab"), "Genus", StringComparison.OrdinalIgnoreCase))
                    specimen.QualifierRank = QualifierRankType.Genus;
                else if (string.Equals(identificationMap.GetString("IdeQualifierRank_tab"), "species", StringComparison.OrdinalIgnoreCase))
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
                }
            }

            // Collection Event
            var collectionEventMap = map.GetMap("colevent");
            if (collectionEventMap != null)
            {
                specimen.ExpeditionName = collectionEventMap.GetString("ExpExpeditionName");
                specimen.CollectionEventCode = collectionEventMap.GetString("ColCollectionEventCode");
                specimen.SamplingMethod = collectionEventMap.GetString("ColCollectionMethod");

                DateTime dateVisitedFrom;
                if (DateTime.TryParseExact(collectionEventMap.GetString("ColDateVisitedFrom"), "dd/MM/yyyy", new CultureInfo("en-AU"), DateTimeStyles.None, out dateVisitedFrom))
                {
                    TimeSpan timeVisitedFrom;
                    if (TimeSpan.TryParseExact(collectionEventMap.GetString("ColTimeVisitedFrom"), @"hh\:mm", new CultureInfo("en-AU"), out timeVisitedFrom))
                    {
                        dateVisitedFrom += timeVisitedFrom;
                    }

                    specimen.DateVisitedFrom = dateVisitedFrom;
                }

                DateTime dateVisitedTo;
                if (DateTime.TryParseExact(collectionEventMap.GetString("ColDateVisitedTo"), "dd/MM/yyyy", new CultureInfo("en-AU"), DateTimeStyles.None, out dateVisitedTo))
                {
                    TimeSpan timeVisitedTo;
                    if (TimeSpan.TryParseExact(collectionEventMap.GetString("ColTimeVisitedTo"), @"hh\:mm", new CultureInfo("en-AU"), out timeVisitedTo))
                    {
                        dateVisitedTo += timeVisitedTo;
                    }

                    specimen.DateVisitedTo = dateVisitedTo;
                }

                specimen.DepthTo = collectionEventMap.GetString("AquDepthToMet");
                specimen.DepthFrom = collectionEventMap.GetString("AquDepthFromMet");

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
                    siteMap.GetString("SitSiteCode"),
                    siteMap.GetString("SitSiteNumber")
                }.Concatenate("");

                // Locality
                var geoMap = siteMap.GetMaps("geo").FirstOrDefault();
                if (geoMap != null)
                {
                    specimen.Ocean = geoMap.GetString("LocOcean_tab");
                    specimen.Continent = geoMap.GetString("LocContinent_tab");
                    specimen.Country = geoMap.GetString("LocCountry_tab");
                    specimen.State = geoMap.GetString("LocProvinceStateTerritory_tab");
                    specimen.District = geoMap.GetString("LocDistrictCountyShire_tab");
                    specimen.Town = geoMap.GetString("LocTownship_tab");
                    specimen.NearestNamedPlace = geoMap.GetString("LocNearestNamedPlace_tab");
                }

                specimen.PreciseLocation = siteMap.GetString("LocPreciseLocation");
                specimen.MinimumElevation = siteMap.GetString("LocElevationASLFromMt");
                specimen.MaximumElevation = siteMap.GetString("LocElevationASLToMt");

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

                    specimen.GeodeticDatum = (string.IsNullOrWhiteSpace(latlongMap.GetString("LatDatum_tab"))) ? "WGS84" : latlongMap.GetString("LatDatum_tab");
                    specimen.SiteRadius = latlongMap.GetString("LatRadiusNumeric_tab");
                    specimen.GeoreferenceBy = _partiesNameFactory.Make(latlongMap.GetMap("determinedBy"));

                    DateTime georeferenceDate;
                    if (DateTime.TryParseExact(latlongMap.GetString("LatDetDate0"), "dd/MM/yyyy", new CultureInfo("en-AU"), DateTimeStyles.None, out georeferenceDate))
                        specimen.GeoreferenceDate = georeferenceDate.ToString("s");

                    specimen.GeoreferenceProtocol = latlongMap.GetString("LatLatLongDetermination_tab");
                    specimen.GeoreferenceSource = latlongMap.GetString("LatDetSource_tab");
                }

                // Geology site fields
                if (!string.Equals(specimen.Discipline, "Tektites", StringComparison.OrdinalIgnoreCase) && !string.Equals(specimen.Discipline, "Meteorites", StringComparison.OrdinalIgnoreCase))
                {
                    specimen.GeologyEra = siteMap.GetString("EraEra");
                    specimen.GeologyPeriod = siteMap.GetString("EraAge1");
                    specimen.GeologyEpoch = siteMap.GetString("EraAge2");
                    specimen.GeologyStage = siteMap.GetString("EraMvStage");
                    specimen.GeologyGroup = siteMap.GetStrings("EraMvGroup_tab").Where(x => !string.IsNullOrWhiteSpace(x)).Concatenate(", ");
                    specimen.GeologyFormation = siteMap.GetStrings("EraMvRockUnit_tab").Where(x => !string.IsNullOrWhiteSpace(x)).Concatenate(", ");
                    specimen.GeologyMember = siteMap.GetStrings("EraMvMember_tab").Where(x => !string.IsNullOrWhiteSpace(x)).Concatenate(", ");
                    specimen.GeologyRockType = siteMap.GetStrings("EraLithology_tab").Where(x => !string.IsNullOrWhiteSpace(x)).Concatenate(", ");
                }
            }
                
            // Discipline specific fields
            // Palaeontology
            specimen.PalaeontologyDateCollectedFrom = map.GetString("LocDateCollectedFrom");
            specimen.PalaeontologyDateCollectedTo = map.GetString("LocDateCollectedTo");
            
            // Geology
            specimen.MineralogySpecies = map.GetString("MinSpecies");
            specimen.MineralogyVariety = map.GetString("MinVariety");
            specimen.MineralogyGroup = map.GetString("MinGroup");
            specimen.MineralogyClass = map.GetString("MinClass");
            specimen.MineralogyAssociatedMatrix = map.GetString("MinAssociatedMatrix");
            specimen.MineralogyType = map.GetString("MinType");
            specimen.MineralogyTypeOfType = map.GetString("MinTypeType");
            
            specimen.MeteoritesName = map.GetString("MetName");
            specimen.MeteoritesClass = map.GetString("MetClass");
            specimen.MeteoritesGroup = map.GetString("MetGroup");
            specimen.MeteoritesType = map.GetString("MetType");
            specimen.MeteoritesMinerals = map.GetString("MetMainMineralsPresent");
            specimen.MeteoritesSpecimenWeight = map.GetString("MetSpecimenWeight");
            specimen.MeteoritesTotalWeight = map.GetString("MetTotalWeight");
            specimen.MeteoritesDateFell = map.GetString("MetDateSpecimenFell");
            specimen.MeteoritesDateFound = map.GetString("MetDateSpecimenFound");
            
            specimen.TektitesName = map.GetString("TekName");
            specimen.TektitesClassification = map.GetString("TekClassification");
            specimen.TektitesShape = map.GetString("TekShape");
            specimen.TektitesLocalStrewnfield = map.GetString("TekLocalStrewnfield");
            specimen.TektitesGlobalStrewnfield = map.GetString("TekGlobalStrewnfield");

            specimen.PetrologyRockClass = map.GetString("RocClass");
            specimen.PetrologyRockGroup = map.GetString("RocGroup");
            specimen.PetrologyRockName = map.GetString("RocRockName");
            specimen.PetrologyRockDescription = map.GetString("RocRockDescription");

            // Media
            specimen.Media = _mediaFactory.Make(map.GetMaps("media"));

            var thumbnail = specimen.Media.FirstOrDefault(x => x is ImageMedia) as ImageMedia;
            if (thumbnail != null)
                specimen.ThumbnailUri = thumbnail.Thumbnail.Uri;

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
            
            return specimen;
        }

        public void RegisterAutoMapperMap()
        {
            Mapper.CreateMap<Specimen, Specimen>()
                .ForMember(x => x.Id, options => options.Ignore());
        }
    }
}