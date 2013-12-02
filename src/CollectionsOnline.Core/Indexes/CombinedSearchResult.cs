namespace CollectionsOnline.Core.Indexes
{
    public class CombinedSearchResult
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public object[] Content { get; set; }

        public string Category { get; set; }

        public object[] Tags { get; set; }

        public string ItemType { get; set; }

        public object[] ItemCollectionNames { get; set; }

        public string ItemPrimaryClassification { get; set; }

        public string ItemSecondaryClassification { get; set; }

        public string ItemTertiaryClassification { get; set; }

        public object[] ItemAssociationNames { get; set; }

        public object[] StoryTypes { get; set; }

        public string SpeciesType { get; set; }

        public string SpeciesSubType { get; set; }
    }
}
