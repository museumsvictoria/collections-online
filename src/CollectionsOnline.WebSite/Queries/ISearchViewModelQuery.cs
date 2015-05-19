using CollectionsOnline.WebSite.Models;

namespace CollectionsOnline.WebSite.Queries
{
    public interface ISearchViewModelQuery
    {
        SearchIndexViewModel BuildSearchIndex(SearchInputModel searchInputModel);
    }
}