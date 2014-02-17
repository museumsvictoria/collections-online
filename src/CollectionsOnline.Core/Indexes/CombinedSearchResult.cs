namespace CollectionsOnline.Core.Indexes
{
    public class CombinedSearchResult
    {
        /* Content fields */
        public string Id { get; set; }

        public string Name { get; set; }

        public object[] Content { get; set; }

        public string Summary { get; set; }

        public string ThumbUrl { get; set; }
        
        /* facet fields */
        
        public string Type { get; set; }

        public string Category { get; set; }

        public string HasImages { get; set; }

        public string Discipline { get; set; }

        public object[] Dates { get; set; }

        public string ItemType { get; set; }

        public string SpeciesType { get; set; }

        public string SpeciesSubType { get; set; }

        public object[] SpeciesHabitats { get; set; }

        public object[] SpeciesDepths { get; set; }

        public object[] SpeciesWaterColumnLocations { get; set; }

        public string Phylum { get; set; }

        public string Class { get; set; }

        public string SpecimenScientificGroup { get; set; }

        public string SpecimenDiscipline { get; set; }

        public object[] StoryTypes { get; set; }
        
        /* term fields */

        public object[] Tags { get; set; }

        public object[] Country { get; set; }

        public object[] ItemCollectionNames { get; set; }

        public string ItemPrimaryClassification { get; set; }

        public string ItemSecondaryClassification { get; set; }

        public string ItemTertiaryClassification { get; set; }

        public object[] ItemAssociationNames { get; set; }

        public string ItemTradeLiteraturePrimarySubject { get; set; }

        public string ItemTradeLiteraturePublicationDate { get; set; }

        public string ItemTradeLiteraturePrimaryRole { get; set; }

        public string ItemTradeLiteraturePrimaryName { get; set; }
    }
}
