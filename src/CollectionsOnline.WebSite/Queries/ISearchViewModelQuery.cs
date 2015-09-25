using CollectionsOnline.WebSite.Models;
using CollectionsOnline.WebSite.Models.Api;

namespace CollectionsOnline.WebSite.Queries
{
    public interface ISearchViewModelQuery
    {
        SearchIndexViewModel BuildSearchIndex(SearchInputModel searchInputModel);

        ApiViewModel BuildSearchApi(SearchApiInputModel searchApiInputModel, ApiInputModel apiInputModel);
    }
}