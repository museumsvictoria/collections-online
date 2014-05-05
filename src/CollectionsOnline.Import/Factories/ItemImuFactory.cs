using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using AutoMapper;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Factories;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Utilities;
using ImageResizer;
using IMu;
using Raven.Abstractions.Extensions;

namespace CollectionsOnline.Import.Factories
{
    public class ItemImuFactory : IImuFactory<Item>
    {
        private readonly ISlugFactory _slugFactory;
        private readonly IMediaHelper _mediaHelper;
        private readonly IPartiesNameFactory _partiesNameFactory;

        public ItemImuFactory(
            ISlugFactory slugFactory,
            IMediaHelper mediaHelper,
            IPartiesNameFactory partiesNameFactory)
        {
            _slugFactory = slugFactory;
            _mediaHelper = mediaHelper;
            _partiesNameFactory = partiesNameFactory;
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
                        "related=ColRelatedRecordsRef_tab.(irn,MdaDataSets_tab)",
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
                        "media=MulMultiMediaRef_tab.(irn,MulTitle,MulMimeType,MulDescription,MulCreator_tab,MdaDataSets_tab,MdaElement_tab,MdaQualifier_tab,MdaFreeText_tab,ChaRepository_tab,AdmPublishWebNoPassword,AdmDateModified,AdmTimeModified)",
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
                        "location=LocCurrentLocationRef.(LocLocationType,location2=LocHolderLocationRef.(LocLocationType,location3=LocHolderLocationRef.(LocLocationType,LocLevel1),LocLevel1),LocLevel1)"
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
            foreach (var associationMap in map.GetMaps("associations"))
            {
                var association = new Association
                {
                    Type = associationMap.GetString("AssAssociationType_tab"),
                    Country = associationMap.GetString("AssAssociationCountry_tab"),
                    State = associationMap.GetString("AssAssociationState_tab"),
                    Region = associationMap.GetString("AssAssociationRegion_tab"),
                    Locality = associationMap.GetString("AssAssociationLocality_tab"),
                    Street = associationMap.GetString("AssAssociationStreetAddress_tab"),
                    Date = associationMap.GetString("AssAssociationDate_tab"),
                    Comments = associationMap.GetString("AssAssociationComments0")
                };

                association.Name = _partiesNameFactory.MakePartiesName(associationMap.GetMap("party"));

                item.Associations.Add(association);
            }

            // Tags
            if (map.GetStrings("SubSubjects_tab") != null)
                item.Tags = map.GetStrings("SubSubjects_tab").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => _slugFactory.MakeSlug(x)).ToList();

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
            item.ModelNames = map.GetStrings("Pro2ModelNameNumber_tab").Concatenate("; ");

            // Brand names
            item.BrandNames = map.GetMaps("brand")
                .Where(x => x != null && !string.IsNullOrWhiteSpace(x.GetString("Pro2BrandName_tab")))
                .Select(
                    x => !string.IsNullOrWhiteSpace(x.GetString("Pro2ProductType_tab")) 
                        ? string.Format("{0} ({1})", x.GetString("Pro2BrandName_tab"), x.GetString("Pro2ProductType_tab"))
                        : x.GetString("Pro2BrandName_tab"))
                .Concatenate("; ");

            // Related items/specimens
            foreach (var related in map.GetMaps("related").Where(x => x != null && !string.IsNullOrWhiteSpace(x.GetString("irn"))))
            {
                if(related.GetStrings("MdaDataSets_tab").Any(x => string.Equals(x, Constants.ImuItemQueryString, StringComparison.OrdinalIgnoreCase)))
                    item.RelatedIds.Add("items/" + related.GetString("irn"));
                if (related.GetStrings("MdaDataSets_tab").Any(x => string.Equals(x, Constants.ImuSpecimenQueryString, StringComparison.OrdinalIgnoreCase)))
                    item.RelatedIds.Add("specimens/" + related.GetString("irn"));
            }

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
            item.ArcheologyManufactureName = _partiesNameFactory.MakePartiesName(map.GetMap("arcmanname"));
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
            item.TradeLiteraturePublicationTypes = map.GetStrings("TLDPublicationTypes_tab").Concatenate("; ");            
            item.TradeLiteraturePrimaryRole = map.GetString("TLSPrimaryRole");
            item.TradeLiteraturePrimaryName = _partiesNameFactory.MakePartiesName(map.GetMap("tlparty"));

            // Media
            // TODO: add in MdaDataSets_tab check for "Website - Collections Online" when added to data.
            foreach (var mediaMap in map.GetMaps("media").Where(x => 
                x != null &&
                string.Equals(x.GetString("AdmPublishWebNoPassword"), "yes", StringComparison.OrdinalIgnoreCase) &&
                x.GetStrings("MdaDataSets_tab").Contains(Constants.ImuMultimediaQueryString) &&
                x.GetString("MulMimeType") == "image"))
            {
                var irn = long.Parse(mediaMap.GetString("irn"));

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
                    item.Media.Add(new Media
                    {
                        Irn = irn,
                        DateModified =
                            DateTime.ParseExact(
                                string.Format("{0} {1}", mediaMap.GetString("AdmDateModified"),
                                    mediaMap.GetString("AdmTimeModified")), "dd/MM/yyyy HH:mm",
                                new CultureInfo("en-AU")),
                        Title = mediaMap.GetString("MulTitle"),
                        Type = mediaMap.GetString("MulMimeType"),
                        Url = url
                    });
                }
            }

            // Indigenous Cultures Fields
            var iclocalityMap = map.GetMaps("iclocality").FirstOrDefault();
            if (iclocalityMap != null)
            {
                item.IndigenousCulturesLocality = new[]
                    {
                        iclocalityMap.GetString("ProSpecificLocality_tab"),
                        iclocalityMap.GetString("ProRegion_tab"),
                        iclocalityMap.GetString("ProStateProvince_tab"),
                        map.GetString("ProCountry")
                    }.Concatenate(", ");
            }

            item.IndigenousCulturesCulturalGroups = map.GetStrings("ProCulturalGroups_tab").Concatenate(", ");
            item.IndigenousCulturesMedium = map.GetStrings("DesObjectMedium_tab").Concatenate(", ");
            item.IndigenousCulturesDescription = map.GetString("DesObjectDescription");

            if (map.GetStrings("DesSubjects_tab") != null)
                item.Tags.AddRange(map.GetStrings("DesSubjects_tab").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => _slugFactory.MakeSlug(x)));

            item.IndigenousCulturesPhotographer = _partiesNameFactory.MakePartiesName(map.GetMap("icphotographer"));
            item.IndigenousCulturesAuthor = _partiesNameFactory.MakePartiesName(map.GetMap("icauthor"));
            item.IndigenousCulturesIllustrator = _partiesNameFactory.MakePartiesName(map.GetMap("icillustrator"));
            item.IndigenousCulturesMaker = _partiesNameFactory.MakePartiesName(map.GetMap("icmaker"));

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

            item.IndigenousCulturesCollector = _partiesNameFactory.MakePartiesName(map.GetMap("iccollector"));

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
            item.IndigenousCulturesLetterTo = _partiesNameFactory.MakePartiesName(map.GetMap("icletterto"));
            item.IndigenousCulturesLetterFrom = _partiesNameFactory.MakePartiesName(map.GetMap("icletterfrom"));
            
            if (string.Equals(map.GetString("ColCategory"), "Indigenous Collections", StringComparison.OrdinalIgnoreCase))
            {
                item.ObjectName = new[]
                    {
                        item.IndigenousCulturesMedium,
                        item.IndigenousCulturesCulturalGroups,
                        item.IndigenousCulturesLocality,
                        item.IndigenousCulturesDate
                    }.Concatenate(", ");
            }

            // Artwork fields
            item.ArtworkTechnique = map.GetString("ArtTechnique");
            item.ArtworkSupport = map.GetString("ArtSupport");
            item.ArtworkPlateNumber = map.GetString("ArtPlateNumber");
            item.ArtworkDrawingNumber = map.GetString("ArtDrawingNumber");
            item.ArtworkState = map.GetString("ArtState");
            item.ArtworkPublisher = _partiesNameFactory.MakePartiesName(map.GetMap("artpublisher"));
            item.ArtworkPrimaryInscriptions = map.GetString("ArtPrimaryInscriptions");
            item.ArtworkSecondaryInscriptions = map.GetString("ArtSecondaryInscriptions");
            item.ArtworkTertiaryInscriptions = map.GetString("ArtTertiaryInscriptions");

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
                    .Select(x => _partiesNameFactory.MakePartiesName(x.GetMap("name"))).ToList();

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
            var locationMap = map.GetMap("location");

            // Build summary
            if (!string.IsNullOrWhiteSpace(item.ObjectSummary))
                item.Summary = item.ObjectSummary;
            else if (!string.IsNullOrWhiteSpace(item.Description))
                item.Summary = item.Description;

            // Build associated dates
            var associatedDates = new List<string>();
            string yearSpan;
            foreach (var association in item.Associations)
            {
                yearSpan = NaturalDateConverter.ConvertToYearSpan(association.Date);
                if (!string.IsNullOrWhiteSpace(yearSpan))
                {
                    associatedDates.Add(yearSpan);
                }
            }

            yearSpan = NaturalDateConverter.ConvertToYearSpan(item.ArcheologyManufactureDate);
            if (!string.IsNullOrWhiteSpace(yearSpan))
            {
                associatedDates.Add(yearSpan);
            }

            yearSpan = NaturalDateConverter.ConvertToYearSpan(item.IndigenousCulturesDate);
            if (!string.IsNullOrWhiteSpace(yearSpan))
            {
                associatedDates.Add(yearSpan);
            }

            yearSpan = NaturalDateConverter.ConvertToYearSpan(item.IndigenousCulturesDateCollected);
            if (!string.IsNullOrWhiteSpace(yearSpan))
            {
                associatedDates.Add(yearSpan);
            }

            yearSpan = NaturalDateConverter.ConvertToYearSpan(item.NumismaticsDateIssued);
            if (!string.IsNullOrWhiteSpace(yearSpan))
            {
                associatedDates.Add(yearSpan);
            }

            yearSpan = NaturalDateConverter.ConvertToYearSpan(item.PhilatelyDateIssued);
            if (!string.IsNullOrWhiteSpace(yearSpan))
            {
                associatedDates.Add(yearSpan);
            }

            yearSpan = NaturalDateConverter.ConvertToYearSpan(item.TradeLiteraturePublicationDate);
            if (!string.IsNullOrWhiteSpace(yearSpan))
            {
                associatedDates.Add(yearSpan);
            }

            item.AssociatedDates = associatedDates.Distinct().ToList();

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