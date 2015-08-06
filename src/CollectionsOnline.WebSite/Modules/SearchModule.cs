using CollectionsOnline.WebSite.Factories;
using CollectionsOnline.WebSite.Models;
using CollectionsOnline.WebSite.Queries;
using Nancy;
using Nancy.ModelBinding;

namespace CollectionsOnline.WebSite.Modules
{
    public class SearchModule : NancyModule
    {
        public SearchModule(
            ISearchViewModelQuery searchViewModelQuery,
            IMetadataViewModelFactory metadataViewModelFactory)
        {
            Get["search-index", "/search"] = parameters =>
            {
                var searchInputModel = this.Bind<SearchInputModel>();

                ViewBag.metadata = metadataViewModelFactory.MakeSearchIndex();

                return View["SearchIndex", searchViewModelQuery.BuildSearchIndex(searchInputModel)].WithCookies(searchInputModel.Cookies);
            };
        }
    }
}