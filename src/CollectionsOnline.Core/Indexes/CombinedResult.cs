namespace CollectionsOnline.Core.Indexes
{
    public class CombinedResult
    {
        /* Update fields */
        public long[] MediaIrns { get; set; }

        public long TaxonomyIrn { get; set; }

        /* Content/Order fields */
        public string Id { get; set; }

        public string DisplayTitle { get; set; }

        public object[] Content { get; set; }

        public string Summary { get; set; }

        public string ThumbnailUri { get; set; }

        public int Quality { get; set; }

        /* facet fields */

        public string Type { get; set; }

        public string Category { get; set; }

        public string HasImages { get; set; }

        public string OnDisplay { get; set; }

        public object[] CollectionAreas { get; set; }

        public string ItemType { get; set; }

        public string SpeciesType { get; set; }

        public string SpeciesEndemicity { get; set; }

        public string SpecimenScientificGroup { get; set; }

        public object[] ArticleTypes { get; set; }

        public string OnDisplayLocation { get; set; }

        /* term fields */

        public object[] Keywords { get; set; }

        public object[] Localities { get; set; }

        public object[] Collections { get; set; }

        public object[] Dates { get; set; }

        public object[] CulturalGroups { get; set; }

        public object[] Classifications { get; set; }

        public object[] Names { get; set; }

        public string Technique { get; set; }

        public object[] Denominations { get; set; }

        public object[] Habitats { get; set; }

        public object[] Taxon { get; set; }

        public string TypeStatus { get; set; }

        public object[] GeoTypes { get; set; }

        public object[] MuseumLocations { get; set; }

        public object[] Articles { get; set; }
    }
}