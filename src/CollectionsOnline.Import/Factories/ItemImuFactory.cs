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

namespace CollectionsOnline.Import.Factories
{
    public class ItemImuFactory : IImuFactory<Item>
    {
        private readonly ISlugFactory _slugFactory;
        private readonly IMediaHelper _mediaHelper;

        public ItemImuFactory(
            ISlugFactory slugFactory,
            IMediaHelper mediaHelper)
        {
            _slugFactory = slugFactory;
            _mediaHelper = mediaHelper;
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
                        "associations=[AssAssociationType_tab,name=AssAssociationNameRef_tab.(NamFullName),AssAssociationCountry_tab,AssAssociationState_tab,AssAssociationRegion_tab,AssAssociationLocality_tab,AssAssociationStreetAddress_tab,AssAssociationDate_tab,AssAssociationComments0]",
                        "SubThemes_tab",
                        "SubSubjects_tab",
                        "SubHistoryTechSignificance",
                        "DimModelScale",
                        "DimShape",
                        "dimensions=[DimConfiguration_tab,DimLengthUnit_tab,DimWeightUnit_tab,DimLength_tab,DimWidth_tab,DimDepth_tab,DimHeight_tab,DimCircumference_tab,DimWeight_tab,DimDimensionComments0]",
                        "SupReferences",
                        "bibliography=[summary=BibBibliographyRef_tab.(SummaryData),BibIssuedDate_tab,BibPages_tab]",
                        "Pro2ModelNameNumber_tab",
                        "Pro2BrandName_tab",
                        "related=ColRelatedRecordsRef_tab.(irn)",
                        "ArcContextNumber",
                        "ArcSiteName=ArcSiteNameRef.(SummaryData)",
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
                        "ArcManufacturerName=ArcManufacturerNameRef.(NamFullName)",
                        "ArcManufactureDate",
                        "ArcTechnique",
                        "ArcProvenance",
                        "NumDenomination",
                        "NumDateEra",
                        "NumSeries",
                        "NumMaterial",
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
                        "TLSPrimaryName=TLSPrimaryNameRef.(NamBranch,NamDepartment,NamOrganisation,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry)",
                        "media=MulMultiMediaRef_tab.(irn,MulTitle,MulMimeType,MulDescription,MulCreator_tab,MdaDataSets_tab,MdaElement_tab,MdaQualifier_tab,MdaFreeText_tab,ChaRepository_tab,rights=<erights:MulMultiMediaRef_tab>.(RigType,RigAcknowledgement),AdmPublishWebNoPassword,AdmDateModified,AdmTimeModified)",
                        "DesLocalName",
                        "locality=[ProSpecificLocality_tab,ProRegion_tab,ProStateProvince_tab,]",
                        "ProCountry",
                        "ProCulturalGroups_tab",
                        "DesObjectDescription",
                        "photographer=SouPhotographerRef.(NamFullName)",
                        "author=SouAuthorRef.(NamFullName)",
                        "illustrator=SouIllustratorRef.(NamFullName)",
                        "maker=SouMakerRef.(NamFullName)",
                        "SouDateProduced",
                        "SouDateProducedCirca",
                        "SouProducedEarliestDate",
                        "SouProducedLatestDate",
                        "collector=SouCollectorRef.(NamFullName)",
                        "SouCollectionDate",
                        "SouCollectionDateCirca",
                        "SouCollectionEarliestDate",
                        "SouCollectionLatestDate",
                        "DesCaption_tab",
                        "DesIndividualsIdentified",
                        "ManTitle",
                        "ManSheets",
                        "ManPages",
                        "letterto=ManLetterToRef.(NamFullName)",
                        "letterfrom=ManLetterFromRef.(NamFullName)",
                        "DesIndividualsMentioned_tab",
                        "DesLocalitiesMentioned_tab",
                        "DesStateProvinceMentioned_tab",
                        "DesRegionsMentioned_tab",
                        "DesCountryMentioned_tab",
                        "DesGroupNames_tab",
                        "DesGroupNamesMentioned_tab"
                    };
            }
        }

        public Terms Terms
        {
            get
            {
                var terms = new Terms();

                terms.Add("MdaDataSets_tab", "History & Technology Collections Online");                

                return terms;
            }
        }

        public Item MakeDocument(Map map)
        {
            var item = new Item();

            // Initialize collections that need to be initialized.
            item.Comments = new List<Comment>();

            item.Id = "items/" + map.GetString("irn");

            item.IsHidden = map.GetString("AdmPublishWebNoPassword") == "No";

            item.DateModified = DateTime.ParseExact(
                string.Format("{0} {1}", map.GetString("AdmDateModified"), map.GetString("AdmTimeModified")),
                "dd/MM/yyyy HH:mm",
                new CultureInfo("en-AU"));
            item.Category = map.GetString("ColCategory");
            item.Discipline = map.GetString("ColDiscipline");
            item.Type = map.GetString("ColTypeOfItem");
            item.RegistrationNumber = map["ColRegPart"] != null
                                         ? string.Format("{0}{1}.{2}", map["ColRegPrefix"], map["ColRegNumber"], map["ColRegPart"])
                                         : string.Format("{0}{1}", map["ColRegPrefix"], map["ColRegNumber"]);
            item.CollectionNames = map.GetStrings("ColCollectionName_tab");

            item.PrimaryClassification = map.GetString("ClaPrimaryClassification").ToSentenceCase();
            if (map.GetString("ClaSecondaryClassification") != null && !map.GetString("ClaSecondaryClassification").ToLower().Contains("to be classified"))
                item.SecondaryClassification = map.GetString("ClaSecondaryClassification").ToSentenceCase();
            if (map.GetString("ClaTertiaryClassification") != null && !map.GetString("ClaTertiaryClassification").ToLower().Contains("to be classified"))
                item.TertiaryClassification = map.GetString("ClaTertiaryClassification").ToSentenceCase();

            item.Name = map.GetString("ClaObjectName");
            item.ObjectSummary = map.GetString("ClaObjectSummary");
            item.Description = map.GetString("DesPhysicalDescription");
            item.Inscription = map.GetString("DesInscriptions");

            // Associations
            var associations = new List<Association>();
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
                    Notes = associationMap.GetString("AssAssociationComments0")
                };

                var nameMap = associationMap.GetMap("name");
                if (nameMap != null)
                {
                    association.Name = nameMap.GetString("NamFullName");
                }

                associations.Add(association);
            }
            item.Associations = associations;

            // Tags
            var tags = new List<string>();
            tags.AddRange(map.GetStrings("SubThemes_tab").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => _slugFactory.MakeSlug(x)));
            tags.AddRange(map.GetStrings("SubSubjects_tab").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => _slugFactory.MakeSlug(x)));
            item.Tags = tags;

            item.Significance = map.GetString("SubHistoryTechSignificance");
            item.ModelScale = map.GetString("DimModelScale");
            item.Shape = map.GetString("DimShape");

            // Dimensions
            var dimensions = new List<string>();
            foreach (var dimensionMap in map.GetMaps("dimensions"))
            {
                var dimension = new List<string>();

                var configuration = dimensionMap.GetString("DimConfiguration_tab");

                var lengthUnit = dimensionMap.GetString("DimLengthUnit_tab");
                var weightUnit = dimensionMap.GetString("DimWeightUnit_tab");

                if (!string.IsNullOrWhiteSpace(dimensionMap.GetString("DimLength_tab")))
                    dimension.Add(string.Format("{0} {1} (Length)", dimensionMap.GetString("DimLength_tab"), lengthUnit));

                if (!string.IsNullOrWhiteSpace(dimensionMap.GetString("DimWidth_tab")))
                    dimension.Add(string.Format("{0} {1} (Width)", dimensionMap.GetString("DimWidth_tab"), lengthUnit));

                if (!string.IsNullOrWhiteSpace(dimensionMap.GetString("DimDepth_tab")))
                    dimension.Add(string.Format("{0} {1} (Depth)", dimensionMap.GetString("DimDepth_tab"), lengthUnit));

                if (!string.IsNullOrWhiteSpace(dimensionMap.GetString("DimHeight_tab")))
                    dimension.Add(string.Format("{0} {1} (Height)", dimensionMap.GetString("DimHeight_tab"), lengthUnit));

                if (!string.IsNullOrWhiteSpace(dimensionMap.GetString("DimCircumference_tab")))
                    dimension.Add(string.Format("{0} {1} (Circumference)", dimensionMap.GetString("DimCircumference_tab"), lengthUnit));

                if (!string.IsNullOrWhiteSpace(dimensionMap.GetString("DimWeight_tab")))
                    dimension.Add(string.Format("{0} {1} (Weight)", dimensionMap.GetString("DimWeight_tab"), weightUnit));

                var notes = dimensionMap.GetString("DimDimensionComments0");

                dimensions.Add(string.Format("{0}: {1}. {2}", configuration, dimension.Concatenate(", "), notes));
            }
            item.Dimensions = dimensions;

            item.References = map.GetString("SupReferences");

            // Bibliographys
            var bibliographies = new List<string>();
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

                bibliographies.Add(bibliography.Concatenate(", "));
            }
            item.Bibliographies = bibliographies;

            // Brand/Model names
            item.ModelNames = map.GetStrings("Pro2ModelNameNumber_tab").Concatenate("; ");
            item.BrandNames = map.GetStrings("Pro2BrandName_tab").Concatenate("; ");

            // Related items
            item.RelatedItemIds = map.GetMaps("related").Where(x => x != null).Select(x => "items/" + x.GetString("irn")).ToList();

            // Archeology fields
            item.ArcheologyContextNumber = map.GetString("ArcContextNumber");
            if (map.GetMap("ArcSiteName") != null)
                item.ArcheologySite = map.GetMap("ArcSiteName").GetString("SummaryData");
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
            if (map.GetMap("ArcManufacturerName") != null)
                item.ArcheologySite = map.GetMap("ArcManufacturerName").GetString("NamFullName");
            item.ArcheologyManufactureDate = map.GetString("ArcManufactureDate");
            item.ArcheologyTechnique = map.GetString("ArcTechnique");
            item.ArcheologyProvenance = map.GetString("ArcProvenance");

            // Numismatics fields
            item.NumismaticsDenomination = map.GetString("NumDenomination");
            item.NumismaticsDateIssued = map.GetString("NumDateEra");
            item.NumismaticsSeries = map.GetString("NumSeries");
            item.NumismaticsMaterial = map.GetString("NumMaterial");
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

            var audioContents = new List<string>();
            foreach (var audioContentMap in map.GetMaps("audiocontent"))
            {
                var audioContent = new List<string>();

                audioContent.Add(audioContentMap.GetString("AudItemNumber_tab"));
                audioContent.Add(string.Format("{0} {1}", audioContentMap.GetString("AudSegmentPosition_tab"), audioContentMap.GetString("AudContentUnits_tab")).Trim());
                audioContent.Add(audioContentMap.GetString("AudSegmentContent_tab"));

                audioContents.Add(audioContent.Concatenate(", "));
            }
            item.AudioVisualContentSummary = audioContents.Concatenate(Environment.NewLine);

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

            var tradeLiteratureNameMap = map.GetMap("TLSPrimaryName");
            if (tradeLiteratureNameMap != null)
            {
                item.TradeLiteraturePrimaryName = new[]
                {
                    tradeLiteratureNameMap.GetString("NamBranch"),
                    tradeLiteratureNameMap.GetString("NamDepartment"),
                    tradeLiteratureNameMap.GetString("NamOrganisation"),
                    tradeLiteratureNameMap.GetString("AddPhysStreet"),
                    tradeLiteratureNameMap.GetString("AddPhysCity"),
                    tradeLiteratureNameMap.GetString("AddPhysState"),
                    tradeLiteratureNameMap.GetString("AddPhysCountry")
                }.Concatenate(", ");
            }

            // Media
            // TODO: Be more selective in what media we assign to item and how
            var media = new List<Media>();
            foreach (var mediaMap in map.GetMaps("media").Where(x => x.GetString("AdmPublishWebNoPassword") == "Yes" && x.GetString("MulMimeType") == "image"))
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
                    media.Add(new Media
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
            item.Media = media;

            // Indigenous Cultures
            item.IndigenousCulturesLocalName = map.GetString("DesLocalName");

            var localityMap = map.GetMaps("locality").FirstOrDefault();
            if (localityMap != null)
            {
                item.IndigenousCulturesLocality = new[]
                    {
                        localityMap.GetString("ProSpecificLocality_tab"),
                        localityMap.GetString("ProRegion_tab"),
                        localityMap.GetString("ProStateProvince_tab"),
                        map.GetString("ProCountry"),
                    }.Concatenate(", ");
            }

            item.IndigenousCulturesCulturalGroups = map.GetStrings("ProCulturalGroups_tab").Concatenate(", ");
            item.IndigenousCulturesDescription = map.GetString("DesObjectDescription");

            //            "photographer=SouPhotographerRef.(NamFullName)",
            //"author=SouAuthorRef.(NamFullName)",
            //"illustrator=SouIllustratorRef.(NamFullName)",
            //"maker=SouMakerRef.(NamFullName)",

            if (!string.IsNullOrWhiteSpace(map.GetString("SouDateProduced")))
            {
                item.IndigenousCulturesDateMade = map.GetString("SouDateProduced");
            }
            else if (!string.IsNullOrWhiteSpace(map.GetString("SouDateProducedCirca")))
            {
                item.IndigenousCulturesDateMade = map.GetString("SouDateProducedCirca");
            }
            else if (!string.IsNullOrWhiteSpace(map.GetString("SouProducedEarliestDate")) || !string.IsNullOrWhiteSpace(map.GetString("SouProducedLatestDate")))
            {
                item.IndigenousCulturesDateMade = new[]
                    {
                        map.GetString("SouProducedEarliestDate"),
                        map.GetString("SouProducedLatestDate")
                    }.Concatenate(" - ");
            }

            //"collector=SouCollectorRef.(NamFullName)",

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

            item.IndigenousCulturesCaption = map.GetStrings("DesCaption_tab").FirstOrDefault();
            item.IndigenousCulturesIndividualsIdentified = map.GetString("DesIndividualsIdentified");
            item.IndigenousCulturesTitle = map.GetString("ManTitle");
            item.IndigenousCulturesSheets = map.GetString("ManSheets");
            item.IndigenousCulturesPages = map.GetString("ManPages");

            //item.IndigenousCulturesLetterTo = map.GetString("ManLetterTo");
            //item.IndigenousCulturesLetterFrom = map.GetString("ManLetterFrom");


            item.IndigenousCulturesIndividualsMentioned = map.GetStrings("DesIndividualsMentioned_tab").Concatenate(", ");
            item.IndigenousCulturesLocalitiesMentioned = map.GetStrings("DesLocalitiesMentioned_tab").Concatenate(", ");
            item.IndigenousCulturesStateProvinceMentioned = map.GetStrings("DesStateProvinceMentioned_tab").Concatenate(", ");
            item.IndigenousCulturesRegionsMentioned = map.GetStrings("DesRegionsMentioned_tab").Concatenate(", ");
            item.IndigenousCulturesCountryMentioned = map.GetStrings("DesCountryMentioned_tab").Concatenate(", ");
            item.IndigenousCulturesGroupNames = map.GetStrings("DesGroupNames_tab").Concatenate(", ");
            item.IndigenousCulturesNamesMentioned = map.GetStrings("DesGroupNamesMentioned_tab").Concatenate(", ");

            // Build summary
            if (!string.IsNullOrWhiteSpace(item.ObjectSummary))
                item.Summary = item.ObjectSummary;
            else if (!string.IsNullOrWhiteSpace(item.Description))
                item.Summary = item.Description;
            else if (!string.IsNullOrWhiteSpace(item.AudioVisualContentSummary))
                item.Summary = item.AudioVisualContentSummary;

            // Build associated dates
            var associatedDates = new List<string>();
            string yearSpan;
            foreach (var association in associations)
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

            yearSpan = NaturalDateConverter.ConvertToYearSpan(item.IndigenousCulturesDateMade);
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