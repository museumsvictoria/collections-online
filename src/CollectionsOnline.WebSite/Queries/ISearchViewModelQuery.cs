using CollectionsOnline.WebSite.Models;

namespace CollectionsOnline.WebSite.Queries
{
    public interface ISearchViewModelQuery
    {
        SearchViewModel BuildSearch(SearchInputModel searchInputModel);
    }
}