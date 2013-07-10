using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CollectionsOnline.Core.DomainModels;
using CollectionsOnline.Core.Factories;
using CollectionsOnline.Core.Extensions;
using IMu;

namespace CollectionsOnline.Import.Helpers
{
    public class ItemImportHelper : IImportHelper<Item>
    {
        private readonly ISlugFactory _slugFactory;

        public ItemImportHelper(
            ISlugFactory slugFactory)
        {
            _slugFactory = slugFactory;
        }

        public string MakeModuleName()
        {
            return "ecatalogue";
        }

        public string[] MakeColumns()
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
                };
        }

        public Terms MakeTerms()
        {
            var terms = new Terms();

            terms.Add("MdaDataSets_tab", "History & Technology Collections Online");
            terms.Add("AdmPublishWebNoPassword", "Yes");

            return terms;
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
            item.CollectionNames = map.GetStrings("ColCollectionName_tab").ToArray();
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
                    bibliography.Add(bibliographyMap.GetString("BibPages_tab"));

                bibliographies.Add(bibliography.Concatenate(", "));
            }
            item.Bibliographies = bibliographies;

            return item;
        }
    }
}