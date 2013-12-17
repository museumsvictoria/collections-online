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
    }
}