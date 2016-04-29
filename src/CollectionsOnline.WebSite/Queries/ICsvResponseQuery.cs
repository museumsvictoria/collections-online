using CollectionsOnline.WebSite.Models;
using Nancy;

namespace CollectionsOnline.WebSite.Queries
{
    public interface ICsvResponseQuery
    {
        Response BuildCsvResponse(SearchIndexCsvModel searchIndexCsvModel);
    }
}