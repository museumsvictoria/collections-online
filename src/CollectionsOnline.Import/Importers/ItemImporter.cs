using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CollectionsOnline.Core.Factories;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Models;
using IMu;

namespace CollectionsOnline.Import.Importers
{
    public class ItemImporter : IImporter<Item>
    {
        private readonly ISlugFactory _slugFactory;

        public ItemImporter(
            ISlugFactory slugFactory)
        {
            _slugFactory = slugFactory;
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
                        "media=MulMultiMediaRef_tab.(irn,MulTitle,MulMimeType,MdaDataSets_tab,MdaElement_tab,MdaQualifier_tab,MdaFreeText_tab,ChaRepository_tab,rights=<erights:MulMultiMediaRef_tab>.(RigType,RigAcknowledgement),AdmPublishWebNoPassword,AdmDateModified,AdmTimeModified)",
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
                        "SouDateProducedEarliestDate",
                        "SouDateProducedLatestDate",
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
                terms.Add("AdmPublishWebNoPassword", "Yes");

                return terms;
            }
        }
        
        public Item MakeDocument(Map map)
        {
            var item = new Item(map.GetString("irn"));

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
            item.PrimaryClassification = map.GetString("ClaPrimaryClassification");
            item.SecondaryClassification = map.GetString("ClaSecondaryClassification");
            item.TertiaryClassification = map.GetString("ClaTertiaryClassification");
            item.Name = map.GetString("ClaObjectName");
            item.Summary = map.GetString("ClaObjectSummary");
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

                if(!string.IsNullOrWhiteSpace(dimensionMap.GetString("DimLength_tab")))
                    dimension.Add(string.Format("{0} {1} (Length)", dimensionMap.GetString("DimLength_tab"), lengthUnit));

                if(!string.IsNullOrWhiteSpace(dimensionMap.GetString("DimWidth_tab")))
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

            // Media
            var media = new List<Media>();
            foreach (var mediaMap in map.GetMaps("media").Where(x => x.GetString("AdmPublishWebNoPassword") == "Yes"))
            {
                media.Add(new Media
                    {
                        DateModified =
                            DateTime.ParseExact(
                                string.Format("{0} {1}", mediaMap.GetString("AdmDateModified"),
                                              mediaMap.GetString("AdmTimeModified")), "dd/MM/yyyy HH:mm",
                                new CultureInfo("en-AU")),
                        Title = mediaMap.GetString("MulTitle"),
                        Type = mediaMap.GetString("MulMimeType")
                    });
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
            else if (!string.IsNullOrWhiteSpace(map.GetString("SouDateProducedEarliestDate")) || !string.IsNullOrWhiteSpace(map.GetString("SouDateProducedLatestDate")))
            {
                item.IndigenousCulturesDateMade = new[]
                    {
                        map.GetString("SouDateProducedEarliestDate"),
                        map.GetString("SouDateProducedLatestDate")
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
            else if (!string.IsNullOrWhiteSpace(map.GetString("SouCollectionDateEarliestDate")) || !string.IsNullOrWhiteSpace(map.GetString("SouCollectionDateLatestDate")))
            {
                item.IndigenousCulturesDateCollected = new[]
                    {
                        map.GetString("SouCollectionDateEarliestDate"),
                        map.GetString("SouCollectionDateLatestDate")
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

            return item;
        }
    }
}