using Nancy;

namespace CollectionsOnline.WebSite.Features.Search
{
    public interface ISearchViewModelQuery
    {
        SearchViewModel BuildSearch(SearchInputModel searchInputModel, Request request);
    }
}