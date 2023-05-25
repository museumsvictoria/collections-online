using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Extensions;
using IMu;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Serilog;

namespace CollectionsOnline.Import.Factories
{
    public class ItemFactory : IEmuAggregateRootFactory<Item>
    {
        private readonly IPartiesNameFactory _partiesNameFactory;
        private readonly IMuseumLocationFactory _museumLocationFactory;
        private readonly ITaxonomyFactory _taxonomyFactory;
        private readonly IMediaFactory _mediaFactory;
        private readonly IAssociationFactory _associationFactory;
        private readonly ISummaryFactory _summaryFactory;
        private readonly IDisplayTitleFactory _displayTitleFactory;

        public ItemFactory(
            IPartiesNameFactory partiesNameFactory,
            IMuseumLocationFactory museumLocationFactory,
            ITaxonomyFactory taxonomyFactory,
            IMediaFactory mediaFactory,
            IAssociationFactory associationFactory,
            ISummaryFactory summaryFactory,
            IDisplayTitleFactory displayTitleFactory)
        {
            _partiesNameFactory = partiesNameFactory;
            _museumLocationFactory = museumLocationFactory;
            _taxonomyFactory = taxonomyFactory;
            _mediaFactory = mediaFactory;
            _associationFactory = associationFactory;
            _summaryFactory = summaryFactory;
            _displayTitleFactory = displayTitleFactory;
        }

        public string ModuleName => "ecatalogue";

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
                        "dimensions=[DimConfiguration_tab,DimLengthUnit_tab,DimWeightUnit_tab,DimLength_tab,DimWidth_tab,DimDepth_tab,DimHeight_tab,DimCircumference_tab,DimOutsideDiameter_tab,DimWeight_tab,DimDimensionComments0]",
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
                        "media=MulMultiMediaRef_tab.(irn,MulTitle,MulIdentifier,MulMimeType,MdaDataSets_tab,metadata=[MdaElement_tab,MdaQualifier_tab,MdaFreeText_tab],DetAlternateText,RigCreator_tab,RigSource_tab,RigAcknowledgementCredit,RigCopyrightStatement,RigCopyrightStatus,RigLicence,RigLicenceDetails,ChaRepository_tab,ChaMd5Sum,AdmPublishWebNoPassword,AdmDateModified,AdmTimeModified)",
                        "fplocality=[ProStateProvince_tab,ProRegion_tab,ProSpecificLocality_tab]",
                        "ProCountry",
                        "ProCulturalGroups_tab",
                        "DesObjectMedium_tab",                        
                        "DesObjectDescription",
                        "DesLocalName",
                        "DesSubjects_tab",
                        "fpphotographer=SouPhotographerRef.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName)",
                        "fpauthor=SouAuthorRef.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName)",
                        "fpillustrator=SouIllustratorRef.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName)",
                        "fpmaker=SouMakerRef.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName)",
                        "SouDateProduced",
                        "SouDateProducedCirca",
                        "SouProducedEarliestDate",
                        "SouProducedLatestDate",
                        "fpcollector=SouCollectorRef.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName)",
                        "SouCollectionDate",
                        "SouCollectionDateCirca",
                        "SouCollectionEarliestDate",
                        "SouCollectionLatestDate",
                        "DesIndividualsIdentified",
                        "ManTitle",
                        "ManSheets",
                        "ManPages",
                        "fpletterto=ManLetterToRef.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName)",
                        "fpletterfrom=ManLetterFromRef.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName)",
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
                        "accession=AccAccessionLotRef.(AcqAcquisitionMethod,AcqDateReceived,AcqDateOwnership,AcqCreditLine,source=[name=AcqSourceRef_tab.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName),AcqSourceRole_tab],AdmPublishWebNoPassword)",
                        "RigText0",
                        "location=LocCurrentLocationRef.(LocLocationType,location=LocHolderLocationRef.(LocLocationType,location=LocHolderLocationRef.(LocLocationType,location=LocHolderLocationRef.(LocLocationType,location=LocHolderLocationRef.(LocLocationType,location=LocHolderLocationRef.(LocLocationType,LocLevel1,LocLevel2,LocLevel3,LocLevel4),LocLevel1,LocLevel2,LocLevel3,LocLevel4),LocLevel1,LocLevel2,LocLevel3,LocLevel4),LocLevel1,LocLevel2,LocLevel3,LocLevel4),LocLevel1,LocLevel2,LocLevel3,LocLevel4),LocLevel1,LocLevel2,LocLevel3,LocLevel4)",
                        "identifications=[IdeTypeStatus_tab,IdeCurrentNameLocal_tab,identifiers=IdeIdentifiedByRef_nesttab.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName),IdeDateIdentified0,IdeAccuracyNotes_tab,IdeQualifier_tab,IdeQualifierRank_tab,taxa=TaxTaxonomyRef_tab.(irn,ClaKingdom,ClaPhylum,ClaSubphylum,ClaSuperclass,ClaClass,ClaSubclass,ClaSuperorder,ClaOrder,ClaSuborder,ClaInfraorder,ClaSuperfamily,ClaFamily,ClaSubfamily,ClaGenus,ClaSubgenus,ClaSpecies,ClaSubspecies,AutAuthorString,ClaApplicableCode,comname=[ComName_tab,ComStatus_tab],relatedspecies=<enarratives:TaxTaxaRef_tab>.(irn,DetPurpose_tab))]",
                        "relatedarticlespecies=<enarratives:ObjObjectsRef_tab>.(irn,DetPurpose_tab)",
                        "relatedpartyarticles=AssAssociationNameRef_tab.(relatedarticles=<enarratives:ParPartiesRef_tab>.(irn,DetPurpose_tab))",
                        "relatedsitearticles=ArcSiteNameRef.(relatedarticles=<enarratives:SitSitesRef_tab>.(irn,DetPurpose_tab))",
                        "ColParentType",
                        "parts=<ecatalogue:ColParentRecordRef>.(objstatus=<eexhibitobjects:StaObjectRef>.(irn,StaStatus,event=StaEventRef.(DatCommencementDate,DatCompletionDate,EveEventNumber,EveEventTitle,venname=VenVenueNameRef_tab.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName)),location=StaCurrentLocationRef.(LocLocationType,location=LocHolderLocationRef.(LocLocationType,location=LocHolderLocationRef.(LocLocationType,location=LocHolderLocationRef.(LocLocationType,location=LocHolderLocationRef.(LocLocationType,location=LocHolderLocationRef.(LocLocationType,LocLevel1,LocLevel2,LocLevel3,LocLevel4),LocLevel1,LocLevel2,LocLevel3,LocLevel4),LocLevel1,LocLevel2,LocLevel3,LocLevel4),LocLevel1,LocLevel2,LocLevel3,LocLevel4),LocLevel1,LocLevel2,LocLevel3,LocLevel4),LocLevel1,LocLevel2,LocLevel3,LocLevel4)))",
                        "objstatus=<eexhibitobjects:StaObjectRef>.(StaStatus,event=StaEventRef.(DatCommencementDate,DatCompletionDate,EveEventNumber,EveEventTitle,venname=VenVenueNameRef_tab.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName)),location=StaCurrentLocationRef.(LocLocationType,location=LocHolderLocationRef.(LocLocationType,location=LocHolderLocationRef.(LocLocationType,location=LocHolderLocationRef.(LocLocationType,location=LocHolderLocationRef.(LocLocationType,location=LocHolderLocationRef.(LocLocationType,LocLevel1,LocLevel2,LocLevel3,LocLevel4),LocLevel1,LocLevel2,LocLevel3,LocLevel4),LocLevel1,LocLevel2,LocLevel3,LocLevel4),LocLevel1,LocLevel2,LocLevel3,LocLevel4),LocLevel1,LocLevel2,LocLevel3,LocLevel4),LocLevel1,LocLevel2,LocLevel3,LocLevel4))",
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

            item.Id = "items/" + map.GetEncodedString("irn");

            item.IsHidden = string.Equals(map.GetEncodedString("AdmPublishWebNoPassword"), "no", StringComparison.OrdinalIgnoreCase);

            item.DateModified = DateTime.ParseExact(
                $"{map.GetEncodedString("AdmDateModified")} {map.GetEncodedString("AdmTimeModified")}",
                "dd/MM/yyyy HH:mm",
                new CultureInfo("en-AU")).ToUniversalTime();

            item.Category = map.GetEncodedString("ColCategory");
            item.Discipline = map.GetEncodedString("ColDiscipline");
            item.Type = map.GetEncodedString("ColTypeOfItem");
            item.RegistrationNumber = !string.IsNullOrWhiteSpace(map.GetEncodedString("ColRegPart"))
                                         ? $"{map.GetEncodedString("ColRegPrefix")} {map.GetEncodedString("ColRegNumber")}.{map.GetEncodedString("ColRegPart")}"
                                         : $"{map.GetEncodedString("ColRegPrefix")} {map.GetEncodedString("ColRegNumber")}";
            
            // Collection names
            item.CollectionNames = map.GetEncodedStrings("ColCollectionName_tab");

            // Collection areas (remove problematic characters used for multi-select facets)
            item.CollectingAreas = map.GetEncodedStrings("SubThemes_tab")
                .Select(x => x.CleanForMultiFacets())
                .ToList();

            // Classifications
            if (map.GetEncodedString("ClaPrimaryClassification") != null && !map.GetEncodedString("ClaPrimaryClassification").Contains("to be classified", StringComparison.OrdinalIgnoreCase))
                item.Classifications.Add(map.GetEncodedString("ClaPrimaryClassification").ToSentenceCase());
            if (map.GetEncodedString("ClaSecondaryClassification") != null && !map.GetEncodedString("ClaSecondaryClassification").Contains("to be classified", StringComparison.OrdinalIgnoreCase))
                item.Classifications.Add(map.GetEncodedString("ClaSecondaryClassification").ToSentenceCase());
            if (map.GetEncodedString("ClaTertiaryClassification") != null && !map.GetEncodedString("ClaTertiaryClassification").Contains("to be classified", StringComparison.OrdinalIgnoreCase))
                 item.Classifications.Add(map.GetEncodedString("ClaTertiaryClassification").ToSentenceCase());

            item.ObjectName = map.GetEncodedString("ClaObjectName");
            item.ObjectSummary = map.GetEncodedString("ClaObjectSummary");
            item.PhysicalDescription = map.GetEncodedString("DesPhysicalDescription");
            item.Inscription = map.GetEncodedString("DesInscriptions");

            // Associations
            item.Associations = _associationFactory.Make(map.GetMaps("associations"));

            // Tags
            item.Keywords.AddRange(map.GetEncodedStrings("SubSubjects_tab"));

            item.Significance = map.GetEncodedString("SubHistoryTechSignificance");
            item.ModelScale = map.GetEncodedString("DimModelScale");
            item.Shape = map.GetEncodedString("DimShape");

            // Dimensions
            foreach (var dimensionMap in map.GetMaps("dimensions"))
            {
                var dimensions = new List<string>();

                var lengthUnit = dimensionMap.GetEncodedString("DimLengthUnit_tab");
                var weightUnit = dimensionMap.GetEncodedString("DimWeightUnit_tab");

                if (!string.IsNullOrWhiteSpace(dimensionMap.GetEncodedString("DimLength_tab")))
                    dimensions.Add($"{dimensionMap.GetEncodedString("DimLength_tab")} {lengthUnit} (Length)");

                if (!string.IsNullOrWhiteSpace(dimensionMap.GetEncodedString("DimWidth_tab")))
                    dimensions.Add($"{dimensionMap.GetEncodedString("DimWidth_tab")} {lengthUnit} (Width)");

                if (!string.IsNullOrWhiteSpace(dimensionMap.GetEncodedString("DimDepth_tab")))
                    dimensions.Add($"{dimensionMap.GetEncodedString("DimDepth_tab")} {lengthUnit} (Depth)");

                if (!string.IsNullOrWhiteSpace(dimensionMap.GetEncodedString("DimHeight_tab")))
                    dimensions.Add($"{dimensionMap.GetEncodedString("DimHeight_tab")} {lengthUnit} (Height)");

                if (!string.IsNullOrWhiteSpace(dimensionMap.GetEncodedString("DimCircumference_tab")))
                    dimensions.Add($"{dimensionMap.GetEncodedString("DimCircumference_tab")} {lengthUnit} (Circumference)");

                if (!string.IsNullOrWhiteSpace(dimensionMap.GetEncodedString("DimOutsideDiameter_tab")))
                    dimensions.Add($"{dimensionMap.GetEncodedString("DimOutsideDiameter_tab")} {lengthUnit} (Outside Diameter)");

                if (!string.IsNullOrWhiteSpace(dimensionMap.GetEncodedString("DimWeight_tab")))
                    dimensions.Add($"{dimensionMap.GetEncodedString("DimWeight_tab")} {weightUnit} (Weight)");

                item.Dimensions.Add(new Dimension
                {
                    Configuration = dimensionMap.GetEncodedString("DimConfiguration_tab"),
                    Dimensions = dimensions.Concatenate(", "),
                    Comments = dimensionMap.GetEncodedString("DimDimensionComments0")
                });
            }
            
            item.References = map.GetEncodedString("SupReferences");

            // Bibliographies
            foreach (var bibliographyMap in map.GetMaps("bibliography"))
            {
                var bibliography = new List<string>();

                var summaryMap = bibliographyMap.GetMap("summary");
                if (summaryMap != null && !string.IsNullOrWhiteSpace(summaryMap.GetEncodedString("SummaryData")))
                    bibliography.Add(summaryMap.GetEncodedString("SummaryData"));

                if (!string.IsNullOrWhiteSpace(bibliographyMap.GetEncodedString("BibIssuedDate_tab")))
                    bibliography.Add(bibliographyMap.GetEncodedString("BibIssuedDate_tab"));

                if (!string.IsNullOrWhiteSpace(bibliographyMap.GetEncodedString("BibPages_tab")))
                    bibliography.Add($"{bibliographyMap.GetEncodedString("BibPages_tab")} Pages");

                item.Bibliographies.Add(bibliography.Concatenate(", "));
            }

            // Model names
            item.ModelNames = map.GetEncodedStrings("Pro2ModelNameNumber_tab");

            // Brand names
            item.Brands.AddRange(
                map.GetMaps("brand")
                    .Select(x => new Brand
                    {
                        Name = x.GetEncodedString("Pro2BrandName_tab"),
                        ProductType = x.GetEncodedString("Pro2ProductType_tab")
                    })
                    .Where(x => x != null));

            // Archeology fields
            item.ArcheologyContextNumber = map.GetEncodedString("ArcContextNumber");
            if (map.GetMap("arcsitename") != null)
                item.ArcheologySite = map.GetMap("arcsitename").GetEncodedString("SummaryData");
            item.ArcheologyDescription = map.GetEncodedString("ArcDescription");
            item.ArcheologyDistinguishingMarks = map.GetEncodedString("ArcDistinguishingMarks");
            item.ArcheologyActivity = map.GetEncodedString("ArcActivity");
            item.ArcheologySpecificActivity = map.GetEncodedString("ArcSpecificActivity");
            item.ArcheologyDecoration = map.GetEncodedString("ArcDecoration");
            item.ArcheologyPattern = map.GetEncodedString("ArcPattern");
            item.ArcheologyColour = map.GetEncodedString("ArcColour");
            item.ArcheologyMoulding = map.GetEncodedString("ArcMoulding");
            item.ArcheologyPlacement = map.GetEncodedString("ArcPlacement");
            item.ArcheologyForm = map.GetEncodedString("ArcForm");
            item.ArcheologyShape = map.GetEncodedString("ArcShape");
            item.ArcheologyManufactureName = _partiesNameFactory.Make(map.GetMap("arcmanname"));
            item.ArcheologyManufactureDate = map.GetEncodedString("ArcManufactureDate");
            item.ArcheologyTechnique = map.GetEncodedString("ArcTechnique");
            item.ArcheologyProvenance = map.GetEncodedString("ArcProvenance");

            // Numismatics fields
            item.NumismaticsDenomination = map.GetEncodedString("NumDenomination");
            item.NumismaticsDateIssued = map.GetEncodedString("NumDateEra");
            item.NumismaticsSeries = map.GetEncodedString("NumSeries");
            item.NumismaticsMaterial = map.GetEncodedString("NumMaterial");
            item.NumismaticsAxis = map.GetEncodedString("NumAxis");
            item.NumismaticsEdgeDescription = map.GetEncodedString("NumEdgeDescription");
            item.NumismaticsObverseDescription = map.GetEncodedString("NumObverseDescription");
            item.NumismaticsReverseDescription = map.GetEncodedString("NumReverseDescription");

            // Philately Fields
            item.PhilatelyColour = map.GetEncodedString("PhiColour");
            item.PhilatelyDenomination = map.GetEncodedString("PhiDenomination");
            item.PhilatelyImprint = map.GetEncodedString("PhiImprint");
            item.PhilatelyIssue = map.GetEncodedString("PhiIssue");
            item.PhilatelyDateIssued = map.GetEncodedString("PhiIssueDate");
            item.PhilatelyForm = map.GetEncodedString("PhiItemForm");
            item.PhilatelyOverprint = map.GetEncodedString("PhiOverprint");
            item.PhilatelyGibbonsNumber = map.GetEncodedString("PhiGibbonsNo");

            // ISD Fields
            item.IsdFormat = new[]
                {
                    map.GetEncodedString("GenMedium"), 
                    map.GetEncodedString("GenFormat"), 
                    map.GetEncodedString("GenColour")
                }.Concatenate(", ");
            item.IsdLanguage = map.GetEncodedString("GenLanguage");
            item.IsdDescriptionOfContent = map.GetEncodedString("Con1Description");
            item.IsdPeopleDepicted = map.GetEncodedStrings("Con3PeopleDepicted_tab").Concatenate("; ");

            // Audiovisual Fields
            item.AudioVisualRecordingDetails = new[]
                {
                    map.GetEncodedString("AudRecordingType"),
                    $"{map.GetEncodedString("AudTotalLengthOfRecording")} {map.GetEncodedString("AudUnits")}".Trim(),
                    map.GetEncodedString("AudAudibilityRating"),
                    map.GetEncodedString("AudComments")
                }.Concatenate(", ");

            foreach (var audioContentMap in map.GetMaps("audiocontent"))
            {
                var audioContent = new List<string>();

                audioContent.Add(audioContentMap.GetEncodedString("AudItemNumber_tab"));
                audioContent.Add(
                    $"{audioContentMap.GetEncodedString("AudSegmentPosition_tab")} {audioContentMap.GetEncodedString("AudContentUnits_tab")}"
                        .Trim());
                audioContent.Add(audioContentMap.GetEncodedString("AudSegmentContent_tab"));

                item.AudioVisualContentSummaries.Add(audioContent.Concatenate(", "));
            }

            // Trade Literature Fields
            item.TradeLiteratureNumberofPages = map.GetEncodedString("TLDNumberOfPages");
            item.TradeLiteraturePageSizeFormat = map.GetEncodedString("TLDPageSizeFormat");
            item.TradeLiteratureCoverTitle = map.GetEncodedString("TLSCoverTitle");
            item.TradeLiteraturePrimarySubject = map.GetEncodedString("TLSPrimarySubject");
            item.TradeLiteraturePublicationDate = map.GetEncodedString("TLSPublicationDate");
            item.TradeLiteratureIllustrationTypes = map.GetEncodedStrings("TLDIllustraionTypes_tab").Concatenate("; ");
            item.TradeLiteraturePrintingTypes = map.GetEncodedStrings("TLDPrintingTypes_tab").Concatenate("; ");
            item.TradeLiteraturePublicationTypes = map.GetEncodedStrings("TLDPublicationTypes_tab").ToList();
            item.TradeLiteraturePrimaryRole = map.GetEncodedString("TLSPrimaryRole");
            item.TradeLiteraturePrimaryName = _partiesNameFactory.Make(map.GetMap("tlparty"));

            // Media
            item.Media = _mediaFactory.Make(map.GetMaps("media"));

            // Assign thumbnail
            var media = item.Media.OfType<IHasThumbnail>().FirstOrDefault();
            if (media != null)
                item.ThumbnailUri = media.Thumbnail.Uri;
            
            // First Peoples Fields
            var fpLocalityMap = map.GetMaps("fplocality").FirstOrDefault();
            if (fpLocalityMap != null)
            {
                item.FirstPeoplesLocalities = new[]
                {
                    fpLocalityMap.GetEncodedString("ProSpecificLocality_tab"),
                    fpLocalityMap.GetEncodedString("ProRegion_tab"),
                    fpLocalityMap.GetEncodedString("ProStateProvince_tab"),
                    map.GetEncodedString("ProCountry")
                }.Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
            }

            item.FirstPeoplesCulturalGroups = map.GetEncodedStrings("ProCulturalGroups_tab");
            item.FirstPeoplesMedium = map.GetEncodedStrings("DesObjectMedium_tab").Concatenate(", ");
            item.FirstPeoplesDescription = map.GetEncodedString("DesObjectDescription");
            item.FirstPeoplesLocalName = map.GetEncodedString("DesLocalName");

            item.Keywords.AddRange(map.GetEncodedStrings("DesSubjects_tab"));

            item.FirstPeoplesPhotographer = _partiesNameFactory.Make(map.GetMap("fpphotographer"));
            item.FirstPeoplesAuthor = _partiesNameFactory.Make(map.GetMap("fpauthor"));
            item.FirstPeoplesIllustrator = _partiesNameFactory.Make(map.GetMap("fpillustrator"));
            item.FirstPeoplesMaker = _partiesNameFactory.Make(map.GetMap("fpmaker"));

            if (!string.IsNullOrWhiteSpace(map.GetEncodedString("SouDateProduced")))
            {
                item.FirstPeoplesDate = map.GetEncodedString("SouDateProduced");
            }
            else if (!string.IsNullOrWhiteSpace(map.GetEncodedString("SouDateProducedCirca")))
            {
                item.FirstPeoplesDate = map.GetEncodedString("SouDateProducedCirca");
            }
            else if (!string.IsNullOrWhiteSpace(map.GetEncodedString("SouProducedEarliestDate")) || !string.IsNullOrWhiteSpace(map.GetEncodedString("SouProducedLatestDate")))
            {
                item.FirstPeoplesDate = new[]
                    {
                        map.GetEncodedString("SouProducedEarliestDate"),
                        map.GetEncodedString("SouProducedLatestDate")
                    }.Concatenate(" - ");
            }

            item.FirstPeoplesCollector = _partiesNameFactory.Make(map.GetMap("fpcollector"));

            if (!string.IsNullOrWhiteSpace(map.GetEncodedString("SouCollectionDate")))
            {
                item.FirstPeoplesDateCollected = map.GetEncodedString("SouCollectionDate");
            }
            else if (!string.IsNullOrWhiteSpace(map.GetEncodedString("SouCollectionDateCirca")))
            {
                item.FirstPeoplesDateCollected = map.GetEncodedString("SouCollectionDateCirca");
            }
            else if (!string.IsNullOrWhiteSpace(map.GetEncodedString("SouCollectionEarliestDate")) || !string.IsNullOrWhiteSpace(map.GetEncodedString("SouCollectionLatestDate")))
            {
                item.FirstPeoplesDateCollected = new[]
                    {
                        map.GetEncodedString("SouCollectionEarliestDate"),
                        map.GetEncodedString("SouCollectionLatestDate")
                    }.Concatenate(" - ");
            }

            item.FirstPeoplesIndividualsIdentified = map.GetEncodedString("DesIndividualsIdentified");

            item.FirstPeoplesTitle = map.GetEncodedString("ManTitle");
            item.FirstPeoplesSheets = map.GetEncodedString("ManSheets");
            item.FirstPeoplesPages = map.GetEncodedString("ManPages");
            item.FirstPeoplesLetterTo = _partiesNameFactory.Make(map.GetMap("fpletterto"));
            item.FirstPeoplesLetterFrom = _partiesNameFactory.Make(map.GetMap("fpletterfrom"));           

            // Artwork fields
            item.ArtworkMedium = map.GetEncodedString("ArtMedium");
            item.ArtworkTechnique = map.GetEncodedString("ArtTechnique");
            item.ArtworkSupport = map.GetEncodedString("ArtSupport");
            item.ArtworkPlateNumber = map.GetEncodedString("ArtPlateNumber");
            item.ArtworkDrawingNumber = map.GetEncodedString("ArtDrawingNumber");
            item.ArtworkState = map.GetEncodedString("ArtState");
            item.ArtworkPublisher = _partiesNameFactory.Make(map.GetMap("artpublisher"));
            item.ArtworkPrimaryInscriptions = map.GetEncodedString("ArtPrimaryInscriptions");
            item.ArtworkSecondaryInscriptions = map.GetEncodedString("ArtSecondaryInscriptions");
            item.ArtworkTertiaryInscriptions = map.GetEncodedString("ArtTertiaryInscriptions");

            // Taxonomy
            // TODO: make factory method as code duplicated in SpecimenFactory
            var identificationMap = map.GetMaps("identifications").FirstOrDefault(x => Constants.TaxonomyTypeStatuses.Contains(x.GetEncodedString("IdeTypeStatus_tab"), StringComparison.OrdinalIgnoreCase)) ??
                                    map.GetMaps("identifications").FirstOrDefault(x => string.Equals(x.GetEncodedString("IdeCurrentNameLocal_tab"), "yes", StringComparison.OrdinalIgnoreCase)) ??
                                    map.GetMaps("identifications").FirstOrDefault();
            if (identificationMap != null)
            {
                // Type Status
                item.TypeStatus = identificationMap.GetEncodedString("IdeTypeStatus_tab");
                // Identified By
                if (identificationMap.GetMaps("identifiers") != null)
                {
                    item.IdentifiedBy = identificationMap.GetMaps("identifiers").Where(x => x != null).Select(x => _partiesNameFactory.Make(x)).Concatenate("; ");
                }
                // Date Identified
                item.DateIdentified = identificationMap.GetEncodedString("IdeDateIdentified0");

                // Identification Qualifier and Rank
                item.Qualifier = identificationMap.GetEncodedString("IdeQualifier_tab");
                if (string.Equals(identificationMap.GetEncodedString("IdeQualifierRank_tab"), "Genus", StringComparison.OrdinalIgnoreCase))
                    item.QualifierRank = QualifierRankType.Genus;
                else if (string.Equals(identificationMap.GetEncodedString("IdeQualifierRank_tab"), "species", StringComparison.OrdinalIgnoreCase))
                    item.QualifierRank = QualifierRankType.Species;

                // Taxonomy
                var taxonomyMap = identificationMap.GetMap("taxa");
                item.Taxonomy = _taxonomyFactory.Make(taxonomyMap);

                if (taxonomyMap != null)
                {
                    // Species profile Relationship
                    var relatedSpeciesMaps = taxonomyMap.GetMaps("relatedspecies");
                    item.RelatedSpeciesIds.AddRange(relatedSpeciesMaps
                        .Where(x => x != null && x.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuSpeciesQueryString))
                        .Select(x => $"species/{x.GetEncodedString("irn")}"));
                }
            }

            // Acquisition information
            // TODO: make factory method as code duplicated in SpecimenFactory
            var accessionMap = map.GetMap("accession");
            if (accessionMap != null &&
                string.Equals(accessionMap.GetEncodedString("AdmPublishWebNoPassword"), "yes", StringComparison.OrdinalIgnoreCase))
            {
                var method = accessionMap.GetEncodedString("AcqAcquisitionMethod");

                if (!string.IsNullOrWhiteSpace(method))
                {
                    var sources = accessionMap.GetMaps("source")
                    .Where(x => string.IsNullOrWhiteSpace(x.GetEncodedString("AcqSourceRole_tab")) ||
                        !x.GetEncodedString("AcqSourceRole_tab").Contains("confidential", StringComparison.OrdinalIgnoreCase) &&
                        !x.GetEncodedString("AcqSourceRole_tab").Contains("contact", StringComparison.OrdinalIgnoreCase) &&
                        !x.GetEncodedString("AcqSourceRole_tab").Contains("vendor", StringComparison.OrdinalIgnoreCase))
                    .Select(x => _partiesNameFactory.Make(x.GetMap("name"))).ToList();

                    if (sources.Any())
                    {
                        if (!string.IsNullOrWhiteSpace(accessionMap.GetEncodedString("AcqDateReceived")))
                            sources.Add(accessionMap.GetEncodedString("AcqDateReceived"));
                        else if (!string.IsNullOrWhiteSpace(accessionMap.GetEncodedString("AcqDateOwnership")))
                            sources.Add(accessionMap.GetEncodedString("AcqDateOwnership"));

                        item.AcquisitionInformation = $"{method} from {sources.Concatenate(", ")}";
                    }
                    else
                    {
                        item.AcquisitionInformation = method;
                    }
                }

                var rights = map.GetEncodedStrings("RigText0").FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(accessionMap.GetEncodedString("AcqCreditLine")))
                    item.Acknowledgement = accessionMap.GetEncodedString("AcqCreditLine");
                else if (!string.IsNullOrWhiteSpace(rights))
                    item.Acknowledgement = rights;
            }

            // Museum Location
            item.MuseumLocation = _museumLocationFactory.Make(map.GetEncodedString("ColParentType"),
                map.GetMaps("objstatus"), map.GetMaps("parts"));

            // Licence
            item.Licence = Constants.Licences[LicenceType.CcBy];

            // Relationships

            // Related items/specimens (directly related)
            foreach (var relatedItemSpecimen in map.GetMaps("relateditemspecimens").Where(x => x != null && !string.IsNullOrWhiteSpace(x.GetEncodedString("irn"))))
            {
                if (relatedItemSpecimen.GetEncodedStrings("MdaDataSets_tab").Contains(Constants.ImuItemQueryString))
                    item.RelatedItemIds.Add($"items/{relatedItemSpecimen.GetEncodedString("irn")}");
                if (relatedItemSpecimen.GetEncodedStrings("MdaDataSets_tab").Contains(Constants.ImuSpecimenQueryString))
                    item.RelatedSpecimenIds.Add($"specimens/{relatedItemSpecimen.GetEncodedString("irn")}");
            }

            // Related articles/species (direct attached)
            var relatedArticleSpeciesMap = map.GetMaps("relatedarticlespecies");
            if (relatedArticleSpeciesMap != null)
            {
                item.RelatedArticleIds.AddRangeUnique(relatedArticleSpeciesMap
                    .Where(x => x != null && x.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuArticleQueryString))
                    .Select(x => $"articles/{x.GetEncodedString("irn")}"));

                item.RelatedSpeciesIds.AddRangeUnique(relatedArticleSpeciesMap
                    .Where(x => x != null && x.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuSpeciesQueryString))
                    .Select(x => $"species/{x.GetEncodedString("irn")}"));
            }

            // Related articles (via party relationship)
            var relatedPartyArticlesMap = map.GetMaps("relatedpartyarticles");
            if (relatedPartyArticlesMap != null)
            {
                item.RelatedArticleIds.AddRangeUnique(relatedPartyArticlesMap
                        .Where(x => x != null)
                        .SelectMany(x => x.GetMaps("relatedarticles"))
                        .Where(x => x != null && x.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuArticleQueryString))
                        .Select(x => $"articles/{x.GetEncodedString("irn")}"));
            }

            // Related articles (via sites relationship)
            var relatedSiteArticlesMap = map.GetMap("relatedsitearticles");
            if (relatedSiteArticlesMap != null)
            {
                item.RelatedArticleIds.AddRangeUnique(relatedSiteArticlesMap
                        .GetMaps("relatedarticles")
                        .Where(x => x != null && x.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuArticleQueryString))
                        .Select(x => $"articles/{x.GetEncodedString("irn")}"));
            }

            // Build summary
            item.Summary = _summaryFactory.Make(item);

            // Display Title
            item.DisplayTitle = _displayTitleFactory.Make(item);

            Log.Logger.Debug("Completed {Id} creation with {MediaCount} media", item.Id, item.Media.Count);

            return item;
        }

        public void UpdateDocument(Item newDocument, Item existingDocument, IList<string> missingDocumentIds,
            IDocumentSession documentSession)
        {
            // Map over existing document
            Mapper.Map(newDocument, existingDocument);
        }
    }
}