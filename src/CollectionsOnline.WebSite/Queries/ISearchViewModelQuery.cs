using CollectionsOnline.WebSite.Models;
using Nancy;

namespace CollectionsOnline.WebSite.Queries
{
    public interface ISearchViewModelQuery
    {
        SearchViewModel BuildSearch(SearchInputModel searchInputModel, Request request);
    }
}