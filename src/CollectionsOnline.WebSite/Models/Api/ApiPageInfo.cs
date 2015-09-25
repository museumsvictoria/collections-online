namespace CollectionsOnline.WebSite.Models.Api
{
    public class ApiPageInfo
    {
        public ApiPageInfo(int totalResults, int perPage)
        {
            TotalResults = totalResults;
            TotalPages = ((totalResults + perPage - 1) / perPage);
        }

        public int TotalResults { get; private set; }

        public int TotalPages { get; private set; }
    }
}