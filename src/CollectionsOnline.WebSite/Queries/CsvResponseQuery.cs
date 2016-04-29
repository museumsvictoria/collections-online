using CollectionsOnline.WebSite.Models;
using Nancy;

namespace CollectionsOnline.WebSite.Queries
{
    public class CsvResponseQuery : ICsvResponseQuery
    {
        public Response BuildCsvResponse(SearchIndexCsvModel searchIndexCsvModel)
        {
            return new Response();
        }
    }
}