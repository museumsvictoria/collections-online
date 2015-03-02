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

        public object[] CollectionArea { get; set; }

        public string ItemType { get; set; }

        public string SpeciesType { get; set; }

        public string SpeciesEndemicity { get; set; }

        public string SpecimenScientificGroup { get; set; }

        public object[] ArticleType { get; set; }

        public string OnDisplayLocation { get; set; }

        /* term fields */

        public object[] Keyword { get; set; }

        public object[] Locality { get; set; }

        public object[] Collection { get; set; }

        public object[] Date { get; set; }

        public object[] CulturalGroup { get; set; }

        public object[] Classification { get; set; }

        public object[] Name { get; set; }

        public string Technique { get; set; }

        public object[] Denomination { get; set; }

        public object[] Habitat { get; set; }

        public object[] Taxon { get; set; }

        public string TypeStatus { get; set; }

        public object[] GeoType { get; set; }

        public object[] MuseumLocation { get; set; }

        public object[] Article { get; set; }
    }
}