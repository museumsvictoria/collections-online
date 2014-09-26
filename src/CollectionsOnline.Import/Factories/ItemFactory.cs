using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using AutoMapper;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Factories;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Core.Utilities;
using IMu;
using NLog;
using Raven.Abstractions.Extensions;

namespace CollectionsOnline.Import.Factories
{
    public class ItemFactory : IEmuAggregateRootFactory<Item>
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly ISlugFactory _slugFactory;
        private readonly IPartiesNameFactory _partiesNameFactory;
        private readonly IMuseumLocationFactory _museumLocationFactory;
        private readonly ITaxonomyFactory _taxonomyFactory;
        private readonly IMediaFactory _mediaFactory;
        private readonly IAssociationFactory _associationFactory;

        public ItemFactory(
            ISlugFactory slugFactory,
            IPartiesNameFactory partiesNameFactory,
            IMuseumLocationFactory museumLocationFactory,
            ITaxonomyFactory taxonomyFactory,
            IMediaFactory mediaFactory,
            IAssociationFactory associationFactory)
        {
            _slugFactory = slugFactory;
            _partiesNameFactory = partiesNameFactory;
            _museumLocationFactory = museumLocationFactory;
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
                        "AdmDateModified",
                        "AdmTimeModified",
                        "ColCategory",
                        "ColDiscipline",
                        "ColTypeOfItem",
                        "ColRegPrefix",
                        "ColRegNumber",
                        "ColRegPart",
                        "ColCollectionName_tab",
                        "ClaPrimaryClassification",
                        "ClaSecondaryClassification",
                        "ClaTertiaryClassification",
                        "ClaObjectName",
                        "ClaObjectSummary",
                        "DesPhysicalDescription",
                        "DesInscriptions",
                        "associations=[AssAssociationType_tab,party=AssAssociationNameRef_tab.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName),AssAssociationCountry_tab,AssAssociationState_tab,AssAssociationRegion_tab,AssAssociationLocality_tab,AssAssociationStreetAddress_tab,AssAssociationDate_tab,AssAssociationComments0]",
                        "SubThemes_tab",
                        "SubSubjects_tab",
                        "SubHistoryTechSignificance",
                        "DimModelScale",
                        "DimShape",
                        "dimensions=[DimConfiguration_tab,DimLengthUnit_tab,DimWeightUnit_tab,DimLength_tab,DimWidth_tab,DimDepth_tab,DimHeight_tab,DimCircumference_tab,DimWeight_tab,DimDimensionComments0]",
                        "SupReferences",
                        "bibliography=[summary=BibBibliographyRef_tab.(SummaryData),BibIssuedDate_tab,BibPages_tab]",
                        "Pro2ModelNameNumber_tab",
                        "brand=[Pro2BrandName_tab,Pro2ProductType_tab]",
                        "relateditemspecimens=ColRelatedRecordsRef_tab.(irn,MdaDataSets_tab)",
                        "ArcContextNumber",
                        "arcsitename=ArcSiteNameRef.(SummaryData)",
                        "ArcDescription",
                        "ArcDistinguishingMarks",
                        "ArcActivity",
                        "ArcSpecificActivity",
                        "ArcDecoration",
                        "ArcPattern",
                        "ArcColour",
                        "ArcMoulding",
                        "ArcPlacement",
                        "ArcForm",
                        "ArcShape",
                        "arcmanname=ArcManufacturerNameRef.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName)",
                        "ArcManufactureDate",
                        "ArcTechnique",
                        "ArcProvenance",
                        "NumDenomination",
                        "NumDateEra",
                        "NumSeries",
                        "NumMaterial",
                        "NumAxis",
                        "NumEdgeDescription",
                        "NumObverseDescription",
                        "NumReverseDescription",
                        "PhiColour",
                        "PhiDenomination",
                        "PhiImprint",
                        "PhiIssue",
                        "PhiIssueDate",
                        "PhiItemForm",
                        "PhiOverprint",
                        "PhiGibbonsNo",
                        "GenMedium",
                        "GenFormat",
                        "GenColour",
                        "GenLanguage",
                        "Con1Description",
                        "Con3PeopleDepicted_tab",
                        "AudRecordingType",
                        "AudTotalLengthOfRecording",
                        "AudUnits",
                        "AudAudibilityRating",
                        "AudComments",
                        "audiocontent=[AudItemNumber_tab,AudSegmentPosition_tab,AudContentUnits_tab,AudSegmentContent_tab]",
                        "TLDNumberOfPages",
                        "TLDPageSizeFormat",
                        "TLSCoverTitle",
                        "TLSPrimarySubject",
                        "TLSPublicationDate",
                        "TLDIllustraionTypes_tab",
                        "TLDPrintingTypes_tab",
                        "TLDPublicationTypes_tab",
                        "TLSPrimaryRole",
                        "tlparty=TLSPrimaryNameRef.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName)",
                        "media=MulMultiMediaRef_tab.(irn,MulTitle,MulMimeType,MulDescription,MulCreator_tab,MdaDataSets_tab,MdaElement_tab,MdaQualifier_tab,MdaFreeText_tab,ChaRepository_tab,DetAlternateText,AdmPublishWebNoPassword,AdmDateModified,AdmTimeModified)",
                        "iclocality=[ProStateProvince_tab,ProRegion_tab,ProSpecificLocality_tab]",
                        "ProCountry",
                        "ProCulturalGroups_tab",
                        "DesObjectMedium_tab",
                        "DesObjectDescription",
                        "DesSubjects_tab",
                        "icphotographer=SouPhotographerRef.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName)",
                        "icauthor=SouAuthorRef.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName)",
                        "icillustrator=SouIllustratorRef.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName)",
                        "icmaker=SouMakerRef.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName)",
                        "SouDateProduced",
                        "SouDateProducedCirca",
                        "SouProducedEarliestDate",
                        "SouProducedLatestDate",
                        "iccollector=SouCollectorRef.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName)",
                        "SouCollectionDate",
                        "SouCollectionDateCirca",
                        "SouCollectionEarliestDate",
                        "SouCollectionLatestDate",
                        "DesIndividualsIdentified",
                        "ManTitle",
                        "ManSheets",
                        "ManPages",
                        "icletterto=ManLetterToRef.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName)",
                        "icletterfrom=ManLetterFromRef.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName)",
                        "ArtMedium",
                        "ArtTechnique",
                        "ArtSupport",
                        "ArtPlateNumber",
                        "ArtDrawingNumber",
                        "ArtState",
                        "artpublisher=ArtPublisherRef.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName)",
                        "ArtPrimaryInscriptions",
                        "ArtSecondaryInscriptions",
                        "ArtTertiaryInscriptions",
                        "accession=AccAccessionLotRef.(AcqAcquisitionMethod,AcqDateReceived,AcqDateOwnership,AcqCreditLine,source=[name=AcqSourceRef_tab.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName),AcqSourceRole_tab])",
                        "RigText0",
                        "location=LocCurrentLocationRef.(LocLocationType,location=LocHolderLocationRef.(LocLocationType,location=LocHolderLocationRef.(LocLocationType,location=LocHolderLocationRef.(LocLocationType,location=LocHolderLocationRef.(LocLocationType,location=LocHolderLocationRef.(LocLocationType,LocLevel1,LocLevel2,LocLevel3,LocLevel4),LocLevel1,LocLevel2,LocLevel3,LocLevel4),LocLevel1,LocLevel2,LocLevel3,LocLevel4),LocLevel1,LocLevel2,LocLevel3,LocLevel4),LocLevel1,LocLevel2,LocLevel3,LocLevel4),LocLevel1,LocLevel2,LocLevel3,LocLevel4)",
                        "identifications=[IdeTypeStatus_tab,IdeCurrentNameLocal_tab,identifiers=IdeIdentifiedByRef_nesttab.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName),IdeDateIdentified0,IdeAccuracyNotes_tab,IdeQualifier_tab,IdeQualifierRank_tab,taxa=TaxTaxonomyRef_tab.(irn,ClaKingdom,ClaPhylum,ClaSubphylum,ClaSuperclass,ClaClass,ClaSubclass,ClaSuperorder,ClaOrder,ClaSuborder,ClaInfraorder,ClaSuperfamily,ClaFamily,ClaSubfamily,ClaGenus,ClaSubgenus,ClaSpecies,ClaSubspecies,AutAuthorString,ClaApplicableCode,comname=[ComName_tab,ComStatus_tab])]",
                        "relatedarticles=<enarratives:ObjObjectsRef_tab>.(irn,DetPurpose_tab)",
                        "relatedpartyarticles=AssAssociationNameRef_tab.(relatedarticles=<enarratives:ParPartiesRef_tab>.(irn,DetPurpose_tab))",
                        "relatedsitearticles=ArcSiteNameRef.(relatedarticles=<enarratives:SitSitesRef_tab>.(irn,DetPurpose_tab))"
                    };
            }
        }

        public Terms Terms
        {
            get
            {
                var terms = new Terms();

                terms.Add("MdaDataSets_tab", Constants.ImuItemQueryString);
                
                return terms;
            }
        }

        public Item MakeDocument(Map map)
        {
            var stopwatch = Stopwatch.StartNew();

            var item = new Item();

            item.Id = "items/" + map.GetString("irn");

            item.IsHidden = string.Equals(map.GetString("AdmPublishWebNoPassword"), "no", StringComparison.OrdinalIgnoreCase);

            item.DateModified = DateTime.ParseExact(
                string.Format("{0} {1}", map.GetString("AdmDateModified"), map.GetString("AdmTimeModified")),
                "dd/MM/yyyy HH:mm",
                new CultureInfo("en-AU"));
            item.Category = map.GetString("ColCategory");
            item.Discipline = map.GetString("ColDiscipline");
            item.Type = map.GetString("ColTypeOfItem");
            item.RegistrationNumber = !string.IsNullOrWhiteSpace(map.GetString("ColRegPart"))
                                         ? string.Format("{0}{1}.{2}", map.GetString("ColRegPrefix"), map.GetString("ColRegNumber"), map.GetString("ColRegPart"))
                                         : string.Format("{0}{1}", map.GetString("ColRegPrefix"), map.GetString("ColRegNumber"));
            
            // Collection names
            item.CollectionNames = map.GetStrings("ColCollectionName_tab").Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

            // Collection plans
            item.CollectionPlans = map.GetStrings("SubThemes_tab").Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

            // Classifications
            if (map.GetString("ClaPrimaryClassification") != null && !map.GetString("ClaPrimaryClassification").Contains("to be classified", StringComparison.OrdinalIgnoreCase))
                item.PrimaryClassification = map.GetString("ClaPrimaryClassification").ToSentenceCase();
            if (map.GetString("ClaSecondaryClassification") != null && !map.GetString("ClaSecondaryClassification").Contains("to be classified", StringComparison.OrdinalIgnoreCase))
                item.SecondaryClassification = map.GetString("ClaSecondaryClassification").ToSentenceCase();
            if (map.GetString("ClaTertiaryClassification") != null && !map.GetString("ClaTertiaryClassification").Contains("to be classified", StringComparison.OrdinalIgnoreCase))
                item.TertiaryClassification = map.GetString("ClaTertiaryClassification").ToSentenceCase();

            item.ObjectName = map.GetString("ClaObjectName");
            item.ObjectSummary = map.GetString("ClaObjectSummary");
            item.Description = map.GetString("DesPhysicalDescription");
            item.Inscription = map.GetString("DesInscriptions");

            // Associations
            item.Associations = _associationFactory.Make(map.GetMaps("associations"));

            // Tags
            if (map.GetStrings("SubSubjects_tab") != null)
                item.Keywords = map.GetStrings("SubSubjects_tab").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => _slugFactory.MakeSlug(x)).ToList();

            item.Significance = map.GetString("SubHistoryTechSignificance");
            item.ModelScale = map.GetString("DimModelScale");
            item.Shape = map.GetString("DimShape");

            // Dimensions
            foreach (var dimensionMap in map.GetMaps("dimensions"))
            {
                var dimensions = new List<string>();

                var lengthUnit = dimensionMap.GetString("DimLengthUnit_tab");
                var weightUnit = dimensionMap.GetString("DimWeightUnit_tab");

                if (!string.IsNullOrWhiteSpace(dimensionMap.GetString("DimLength_tab")))
                    dimensions.Add(string.Format("{0} {1} (Length)", dimensionMap.GetString("DimLength_tab"), lengthUnit));

                if (!string.IsNullOrWhiteSpace(dimensionMap.GetString("DimWidth_tab")))
                    dimensions.Add(string.Format("{0} {1} (Width)", dimensionMap.GetString("DimWidth_tab"), lengthUnit));

                if (!string.IsNullOrWhiteSpace(dimensionMap.GetString("DimDepth_tab")))
                    dimensions.Add(string.Format("{0} {1} (Depth)", dimensionMap.GetString("DimDepth_tab"), lengthUnit));

                if (!string.IsNullOrWhiteSpace(dimensionMap.GetString("DimHeight_tab")))
                    dimensions.Add(string.Format("{0} {1} (Height)", dimensionMap.GetString("DimHeight_tab"), lengthUnit));

                if (!string.IsNullOrWhiteSpace(dimensionMap.GetString("DimCircumference_tab")))
                    dimensions.Add(string.Format("{0} {1} (Circumference)", dimensionMap.GetString("DimCircumference_tab"), lengthUnit));

                if (!string.IsNullOrWhiteSpace(dimensionMap.GetString("DimWeight_tab")))
                    dimensions.Add(string.Format("{0} {1} (Weight)", dimensionMap.GetString("DimWeight_tab"), weightUnit));

                item.Dimensions.Add(new Dimension
                {
                    Configuration = dimensionMap.GetString("DimConfiguration_tab"),
                    Dimensions = dimensions.Concatenate(", "),
                    Comments = dimensionMap.GetString("DimDimensionComments0")
                });
            }
            
            item.References = map.GetString("SupReferences");

            // Bibliographies
            foreach (var bibliographyMap in map.GetMaps("bibliography"))
            {
                var bibliography = new List<string>();

                var summaryMap = bibliographyMap.GetMap("summary");
                if (summaryMap != null && !string.IsNullOrWhiteSpace(summaryMap.GetString("SummaryData")))
                    bibliography.Add(summaryMap.GetString("SummaryData"));

                if (!string.IsNullOrWhiteSpace(bibliographyMap.GetString("BibIssuedDate_tab")))
                    bibliography.Add(bibliographyMap.GetString("BibIssuedDate_tab"));

                if (!string.IsNullOrWhiteSpace(bibliographyMap.GetString("BibPages_tab")))
                    bibliography.Add(string.Format("{0} Pages", bibliographyMap.GetString("BibPages_tab")));

                item.Bibliographies.Add(bibliography.Concatenate(", "));
            }

            // Model names
            item.ModelNames = map.GetStrings("Pro2ModelNameNumber_tab").Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

            // Brand names
            item.BrandNames = map.GetMaps("brand")
                .Where(x => x != null && !string.IsNullOrWhiteSpace(x.GetString("Pro2BrandName_tab")))
                .Select(
                    x => !string.IsNullOrWhiteSpace(x.GetString("Pro2ProductType_tab")) 
                        ? string.Format("{0} ({1})", x.GetString("Pro2BrandName_tab"), x.GetString("Pro2ProductType_tab"))
                        : x.GetString("Pro2BrandName_tab")).ToList();

            // Archeology fields
            item.ArcheologyContextNumber = map.GetString("ArcContextNumber");
            if (map.GetMap("arcsitename") != null)
                item.ArcheologySite = map.GetMap("arcsitename").GetString("SummaryData");
            item.ArcheologyDescription = map.GetString("ArcDescription");
            item.ArcheologyDistinguishingMarks = map.GetString("ArcDistinguishingMarks");
            item.ArcheologyActivity = map.GetString("ArcActivity");
            item.ArcheologySpecificActivity = map.GetString("ArcSpecificActivity");
            item.ArcheologyDecoration = map.GetString("ArcDecoration");
            item.ArcheologyPattern = map.GetString("ArcPattern");
            item.ArcheologyColour = map.GetString("ArcColour");
            item.ArcheologyMoulding = map.GetString("ArcMoulding");
            item.ArcheologyPlacement = map.GetString("ArcPlacement");
            item.ArcheologyForm = map.GetString("ArcForm");
            item.ArcheologyShape = map.GetString("ArcShape");
            item.ArcheologyManufactureName = _partiesNameFactory.Make(map.GetMap("arcmanname"));
            item.ArcheologyManufactureDate = map.GetString("ArcManufactureDate");
            item.ArcheologyTechnique = map.GetString("ArcTechnique");
            item.ArcheologyProvenance = map.GetString("ArcProvenance");

            // Numismatics fields
            item.NumismaticsDenomination = map.GetString("NumDenomination");
            item.NumismaticsDateIssued = map.GetString("NumDateEra");
            item.NumismaticsSeries = map.GetString("NumSeries");
            item.NumismaticsMaterial = map.GetString("NumMaterial");
            item.NumismaticsAxis = map.GetString("NumAxis");
            item.NumismaticsEdgeDescription = map.GetString("NumEdgeDescription");
            item.NumismaticsObverseDescription = map.GetString("NumObverseDescription");
            item.NumismaticsReverseDescription = map.GetString("NumReverseDescription");

            // Philately Fields
            item.PhilatelyColour = map.GetString("PhiColour");
            item.PhilatelyDenomination = map.GetString("PhiDenomination");
            item.PhilatelyImprint = map.GetString("PhiImprint");
            item.PhilatelyIssue = map.GetString("PhiIssue");
            item.PhilatelyDateIssued = map.GetString("PhiIssueDate");
            item.PhilatelyForm = map.GetString("PhiItemForm");
            item.PhilatelyOverprint = map.GetString("PhiOverprint");
            item.PhilatelyGibbonsNumber = map.GetString("PhiGibbonsNo");

            // ISD Fields
            item.IsdFormat = new[]
                {
                    map.GetString("GenMedium"), 
                    map.GetString("GenFormat"), 
                    map.GetString("GenColour")
                }.Concatenate(", ");
            item.IsdLanguage = map.GetString("GenLanguage");
            item.IsdDescriptionOfContent = map.GetString("Con1Description");
            item.IsdPeopleDepicted = map.GetStrings("Con3PeopleDepicted_tab").Concatenate("; ");

            // Audiovisual Fields
            item.AudioVisualRecordingDetails = new[]
                {
                    map.GetString("AudRecordingType"),
                    string.Format("{0} {1}", map.GetString("AudTotalLengthOfRecording"), map.GetString("AudUnits")).Trim(),
                    map.GetString("AudAudibilityRating"),
                    map.GetString("AudComments")
                }.Concatenate(", ");

            foreach (var audioContentMap in map.GetMaps("audiocontent"))
            {
                var audioContent = new List<string>();

                audioContent.Add(audioContentMap.GetString("AudItemNumber_tab"));
                audioContent.Add(string.Format("{0} {1}", audioContentMap.GetString("AudSegmentPosition_tab"), audioContentMap.GetString("AudContentUnits_tab")).Trim());
                audioContent.Add(audioContentMap.GetString("AudSegmentContent_tab"));

                item.AudioVisualContentSummaries.Add(audioContent.Concatenate(", "));
            }

            // Trade Literature Fields
            item.TradeLiteratureNumberofPages = map.GetString("TLDNumberOfPages");
            item.TradeLiteraturePageSizeFormat = map.GetString("TLDPageSizeFormat");
            item.TradeLiteratureCoverTitle = map.GetString("TLSCoverTitle");
            item.TradeLiteraturePrimarySubject = map.GetString("TLSPrimarySubject");
            item.TradeLiteraturePublicationDate = map.GetString("TLSPublicationDate");
            item.TradeLiteratureIllustrationTypes = map.GetStrings("TLDIllustraionTypes_tab").Concatenate("; ");
            item.TradeLiteraturePrintingTypes = map.GetStrings("TLDPrintingTypes_tab").Concatenate("; ");
            item.TradeLiteraturePublicationTypes = map.GetStrings("TLDPublicationTypes_tab").Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            item.TradeLiteraturePrimaryRole = map.GetString("TLSPrimaryRole");
            item.TradeLiteraturePrimaryName = _partiesNameFactory.Make(map.GetMap("tlparty"));

            // Media
            var mediaStopwatch = Stopwatch.StartNew();
            item.Media = _mediaFactory.Make(map.GetMaps("media"));
            mediaStopwatch.Stop();

            var thumbnail = item.Media.FirstOrDefault(x => x is ImageMedia) as ImageMedia;
            if (thumbnail != null)
                item.ThumbnailUri = thumbnail.Thumbnail.Uri;
            
            // Indigenous Cultures Fields
            var iclocalityMap = map.GetMaps("iclocality").FirstOrDefault();
            if (iclocalityMap != null)
            {
                item.IndigenousCulturesLocality = iclocalityMap.GetString("ProSpecificLocality_tab");
                item.IndigenousCulturesRegion = iclocalityMap.GetString("ProRegion_tab");
                item.IndigenousCulturesState = iclocalityMap.GetString("ProStateProvince_tab");
                item.IndigenousCulturesCountry = map.GetString("ProCountry");
            }

            item.IndigenousCulturesCulturalGroups = map.GetStrings("ProCulturalGroups_tab").Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            item.IndigenousCulturesMedium = map.GetStrings("DesObjectMedium_tab").Concatenate(", ");
            item.IndigenousCulturesDescription = map.GetString("DesObjectDescription");

            if (map.GetStrings("DesSubjects_tab") != null)
                item.Keywords.AddRange(map.GetStrings("DesSubjects_tab").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => _slugFactory.MakeSlug(x)));

            item.IndigenousCulturesPhotographer = _partiesNameFactory.Make(map.GetMap("icphotographer"));
            item.IndigenousCulturesAuthor = _partiesNameFactory.Make(map.GetMap("icauthor"));
            item.IndigenousCulturesIllustrator = _partiesNameFactory.Make(map.GetMap("icillustrator"));
            item.IndigenousCulturesMaker = _partiesNameFactory.Make(map.GetMap("icmaker"));

            if (!string.IsNullOrWhiteSpace(map.GetString("SouDateProduced")))
            {
                item.IndigenousCulturesDate = map.GetString("SouDateProduced");
            }
            else if (!string.IsNullOrWhiteSpace(map.GetString("SouDateProducedCirca")))
            {
                item.IndigenousCulturesDate = map.GetString("SouDateProducedCirca");
            }
            else if (!string.IsNullOrWhiteSpace(map.GetString("SouProducedEarliestDate")) || !string.IsNullOrWhiteSpace(map.GetString("SouProducedLatestDate")))
            {
                item.IndigenousCulturesDate = new[]
                    {
                        map.GetString("SouProducedEarliestDate"),
                        map.GetString("SouProducedLatestDate")
                    }.Concatenate(" - ");
            }

            item.IndigenousCulturesCollector = _partiesNameFactory.Make(map.GetMap("iccollector"));

            if (!string.IsNullOrWhiteSpace(map.GetString("SouCollectionDate")))
            {
                item.IndigenousCulturesDateCollected = map.GetString("SouCollectionDate");
            }
            else if (!string.IsNullOrWhiteSpace(map.GetString("SouCollectionDateCirca")))
            {
                item.IndigenousCulturesDateCollected = map.GetString("SouCollectionDateCirca");
            }
            else if (!string.IsNullOrWhiteSpace(map.GetString("SouCollectionEarliestDate")) || !string.IsNullOrWhiteSpace(map.GetString("SouCollectionLatestDate")))
            {
                item.IndigenousCulturesDateCollected = new[]
                    {
                        map.GetString("SouCollectionEarliestDate"),
                        map.GetString("SouCollectionLatestDate")
                    }.Concatenate(" - ");
            }

            item.IndigenousCulturesIndividualsIdentified = map.GetString("DesIndividualsIdentified");

            item.IndigenousCulturesTitle = map.GetString("ManTitle");
            item.IndigenousCulturesSheets = map.GetString("ManSheets");
            item.IndigenousCulturesPages = map.GetString("ManPages");
            item.IndigenousCulturesLetterTo = _partiesNameFactory.Make(map.GetMap("icletterto"));
            item.IndigenousCulturesLetterFrom = _partiesNameFactory.Make(map.GetMap("icletterfrom"));
            
            if (string.Equals(map.GetString("ColCategory"), "Indigenous Collections", StringComparison.OrdinalIgnoreCase))
            {
                item.ObjectName = new[]
                    {
                        item.IndigenousCulturesMedium,
                        item.IndigenousCulturesCulturalGroups.Concatenate(", "),
                        new []
                        {
                            item.IndigenousCulturesLocality,
                            item.IndigenousCulturesRegion,
                            item.IndigenousCulturesState,
                            item.IndigenousCulturesCountry
                        }.Concatenate(", "),
                        item.IndigenousCulturesDate
                    }.Concatenate(", ");
            }

            // Artwork fields
            item.ArtworkMedium = map.GetString("ArtMedium");
            item.ArtworkTechnique = map.GetString("ArtTechnique");
            item.ArtworkSupport = map.GetString("ArtSupport");
            item.ArtworkPlateNumber = map.GetString("ArtPlateNumber");
            item.ArtworkDrawingNumber = map.GetString("ArtDrawingNumber");
            item.ArtworkState = map.GetString("ArtState");
            item.ArtworkPublisher = _partiesNameFactory.Make(map.GetMap("artpublisher"));
            item.ArtworkPrimaryInscriptions = map.GetString("ArtPrimaryInscriptions");
            item.ArtworkSecondaryInscriptions = map.GetString("ArtSecondaryInscriptions");
            item.ArtworkTertiaryInscriptions = map.GetString("ArtTertiaryInscriptions");

            // Taxonomy
            // TODO: make factory method as code duplicated in SpecimenFactory
            var identificationMap = map.GetMaps("identifications").FirstOrDefault(x => (x.GetString("IdeTypeStatus_tab") != null && Constants.TaxonomyTypeStatuses.Contains(x.GetString("IdeTypeStatus_tab").Trim().ToLower()))) ??
                                 map.GetMaps("identifications").FirstOrDefault(x => (x.GetString("IdeCurrentNameLocal_tab") != null && x.GetString("IdeCurrentNameLocal_tab").Trim().ToLower() == "yes"));
            if (identificationMap != null)
            {
                // Type Status
                item.TypeStatus = identificationMap.GetString("IdeTypeStatus_tab");
                // Identified By
                if (identificationMap.GetMaps("identifiers") != null)
                {
                    item.IdentifiedBy = identificationMap.GetMaps("identifiers").Where(x => x != null).Select(x => _partiesNameFactory.Make(x)).Concatenate("; ");
                }
                // Date Identified
                item.DateIdentified = identificationMap.GetString("IdeDateIdentified0");

                // Identification Qualifier and Rank
                item.Qualifier = identificationMap.GetString("IdeQualifier_tab");
                if (string.Equals(identificationMap.GetString("IdeQualifierRank_tab"), "Genus", StringComparison.OrdinalIgnoreCase))
                    item.QualifierRank = QualifierRankType.Genus;
                else if (string.Equals(identificationMap.GetString("IdeQualifierRank_tab"), "species", StringComparison.OrdinalIgnoreCase))
                    item.QualifierRank = QualifierRankType.Species;

                // Taxonomy
                var taxonomyMap = identificationMap.GetMap("taxa");
                item.Taxonomy = _taxonomyFactory.Make(taxonomyMap);

                if (taxonomyMap != null)
                {
                    // Scientific Name
                    item.ScientificName = new[]
                    {
                        item.QualifierRank != QualifierRankType.Genus ? null : item.Qualifier,
                        taxonomyMap.GetString("ClaGenus"),
                        string.IsNullOrWhiteSpace(taxonomyMap.GetString("ClaSubgenus"))
                            ? null
                            : string.Format("({0})", taxonomyMap.GetString("ClaSubgenus")),
                        item.QualifierRank != QualifierRankType.Species ? null : item.Qualifier,
                        taxonomyMap.GetString("ClaSpecies"),
                        taxonomyMap.GetString("ClaSubspecies"),
                        taxonomyMap.GetString("AutAuthorString")
                    }.Concatenate(" ");
                }
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

                        item.AcquisitionInformation = string.Format("{0} from {1}", method, sources.Concatenate(", "));
                    }
                    else
                    {
                        item.AcquisitionInformation = method;
                    }
                }

                var rights = map.GetStrings("RigText0").FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(accessionMap.GetString("AcqCreditLine")))
                    item.Acknowledgement = accessionMap.GetString("AcqCreditLine");
                else if (!string.IsNullOrWhiteSpace(rights))
                    item.Acknowledgement = rights;
            }

            // Object Location
            item.MuseumLocation = _museumLocationFactory.Make(map.GetMap("location"));

            // Relationships

            // Related items/specimens (directly related)
            foreach (var relatedItemSpecimen in map.GetMaps("relateditemspecimens").Where(x => x != null && !string.IsNullOrWhiteSpace(x.GetString("irn"))))
            {
                if (relatedItemSpecimen.GetStrings("MdaDataSets_tab").Contains(Constants.ImuItemQueryString))
                    item.RelatedItemIds.Add(string.Format("items/{0}", relatedItemSpecimen.GetString("irn")));
                if (relatedItemSpecimen.GetStrings("MdaDataSets_tab").Contains(Constants.ImuSpecimenQueryString))
                    item.RelatedSpecimenIds.Add(string.Format("specimens/{0}", relatedItemSpecimen.GetString("irn")));
            }

            // Related articles (direct attached)
            var relatedArticlesMap = map.GetMaps("relatedarticles");
            if (relatedArticlesMap != null)
            {
                item.RelatedArticleIds.AddRangeUnique(relatedArticlesMap
                    .Where(x => x != null && x.GetStrings("DetPurpose_tab").Contains(Constants.ImuArticleQueryString))
                    .Select(x => string.Format("articles/{0}", x.GetString("irn"))));
            }

            // Related articles (via party relationship)
            var relatedPartyArticlesMap = map.GetMaps("relatedpartyarticles");
            if (relatedPartyArticlesMap != null)
            {
                item.RelatedArticleIds.AddRangeUnique(relatedPartyArticlesMap
                        .Where(x => x != null)
                        .SelectMany(x => x.GetMaps("relatedarticles"))
                        .Where(x => x != null && x.GetStrings("DetPurpose_tab").Contains(Constants.ImuArticleQueryString))
                        .Select(x => string.Format("articles/{0}", x.GetString("irn"))));
            }

            // Related articles (via sites relationship)
            var relatedSiteArticlesMap = map.GetMap("relatedsitearticles");
            if (relatedSiteArticlesMap != null)
            {
                item.RelatedArticleIds.AddRangeUnique(relatedSiteArticlesMap
                        .GetMaps("relatedarticles")
                        .Where(x => x != null && x.GetStrings("DetPurpose_tab").Contains(Constants.ImuArticleQueryString))
                        .Select(x => string.Format("articles/{0}", x.GetString("irn"))));
            }

            // Build summary
            if (!string.IsNullOrWhiteSpace(item.ObjectSummary))
                item.Summary = item.ObjectSummary;
            else if (!string.IsNullOrWhiteSpace(item.Description))
                item.Summary = item.Description;
            
            stopwatch.Stop();
            _log.Trace("Completed item creation for Catalog record with irn {0}, elapsed time {1} ms, media creation took {2} ms ({3} media)", map.GetString("irn"), stopwatch.ElapsedMilliseconds, mediaStopwatch.ElapsedMilliseconds, item.Media.Count);

            return item;
        }

        public void RegisterAutoMapperMap()
        {
            Mapper.CreateMap<Item, Item>()
                .ForMember(x => x.Id, options => options.Ignore())
                .ForMember(x => x.Comments, options => options.Ignore());
        }
    }
}