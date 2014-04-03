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

        public string HasImages { get; set; }

        public string Discipline { get; set; }

        public string[] Dates { get; set; }

        public string ItemType { get; set; }

        public string SpeciesType { get; set; }

        public string SpeciesSubType { get; set; }

        public string[] SpeciesHabitats { get; set; }

        public string[] SpeciesDepths { get; set; }

        public string[] SpeciesWaterColumnLocations { get; set; }

        public string Phylum { get; set; }

        public string Class { get; set; }

        public string Order { get; set; }

        public string Family { get; set; }

        public string SpecimenScientificGroup { get; set; }

        public string SpecimenDiscipline { get; set; }

        public string[] StoryTypes { get; set; }

        /* term fields */
        public string Tag { get; set; }

        public string Country { get; set; }

        public string CollectionName { get; set; }

        public string CollectionPlan { get; set; }

        public string PrimaryClassification { get; set; }

        public string SecondaryClassification { get; set; }

        public string TertiaryClassification { get; set; }

        public string AssociationName { get; set; }

        public string ItemTradeLiteraturePrimarySubject { get; set; }

        public string ItemTradeLiteraturePublicationDate { get; set; }

        public string ItemTradeLiteraturePrimaryRole { get; set; }

        public string ItemTradeLiteraturePrimaryName { get; set; }
    }
}