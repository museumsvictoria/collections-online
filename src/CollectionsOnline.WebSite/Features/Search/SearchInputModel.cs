namespace CollectionsOnline.WebSite.Features.Search
{
    public class SearchInputModel
    {
        public int Offset { get; set; }

        public int Limit { get; set; }

        public string Query { get; set; }

        /* facet fields */
        public string Type { get; set; }

        public string Category { get; set; }

        public string ItemType { get; set; }

        public string SpeciesType { get; set; }

        public string SpeciesSubType { get; set; }

        public string[] SpeciesHabitats { get; set; }

        public string[] SpeciesDepths { get; set; }

        public string[] SpeciesWaterColumnLocations { get; set; }

        public string SpeciesPhylum { get; set; }

        public string SpeciesClass { get; set; }

        public string SpeciesOrder { get; set; }

        public string SpeciesFamily { get; set; }

        public string SpecimenScientificGroup { get; set; }

        public string SpecimenDiscipline { get; set; }

        public string[] StoryTypes { get; set; }

        /* term fields */
        public string Tag { get; set; }

        public string Country { get; set; }

        public string ItemCollectionName { get; set; }

        public string ItemPrimaryClassification { get; set; }

        public string ItemSecondaryClassification { get; set; }

        public string ItemTertiaryClassification { get; set; }

        public string ItemAssociationName { get; set; }

        public string ItemTradeLiteraturePrimarySubject { get; set; }

        public string ItemTradeLiteraturePublicationDate { get; set; }

        public string ItemTradeLiteraturePrimaryRole { get; set; }

        public string ItemTradeLiteraturePrimaryName { get; set; }
    }
}