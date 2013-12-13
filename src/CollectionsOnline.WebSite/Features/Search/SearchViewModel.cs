namespace CollectionsOnline.WebSite.Features.Search
{
    public class SearchViewModel
    {
        public int Offset { get; set; }

        public int Limit { get; set; }

        public string Query { get; set; }

        public string Type { get; set; }
    }
}