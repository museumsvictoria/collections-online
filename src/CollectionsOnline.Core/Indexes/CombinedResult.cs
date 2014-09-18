namespace CollectionsOnline.Core.Indexes
{
    public class CombinedResult
    {
        /* Update fields */
        public long[] MediaIrns { get; set; }

        public long TaxonomyIrn { get; set; }

        /* Content/Order fields */
        public string Id { get; set; }

        public string Name { get; set; }

        public object[] Content { get; set; }

        public string Summary { get; set; }

        public string ThumbnailUri { get; set; }

        public int Quality { get; set; }

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

        public object[] ArticleTypes { get; set; }

        /* term fields */

        public object[] Tags { get; set; }

        public object[] Country { get; set; }

        public object[] CollectionNames { get; set; }

        public object[] CollectionPlans { get; set; }

        public string PrimaryClassification { get; set; }

        public string SecondaryClassification { get; set; }

        public string TertiaryClassification { get; set; }

        public object[] AssociationNames { get; set; }

        public string ItemTradeLiteraturePrimarySubject { get; set; }

        public string ItemTradeLiteraturePublicationDate { get; set; }

        public string ItemTradeLiteraturePrimaryRole { get; set; }

        public string ItemTradeLiteraturePrimaryName { get; set; }
    }
}