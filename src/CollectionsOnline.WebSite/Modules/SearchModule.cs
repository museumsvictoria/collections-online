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
            IMetadataViewModelFactory metadataViewModelFactory,
            ICsvResponseQuery csvResponseQuery)
        {
            Get["search-index", "/search"] = parameters =>
            {
                var searchInputModel = this.Bind<SearchInputModel>();

                ViewBag.metadata = metadataViewModelFactory.MakeSearchIndex();

                return View["SearchIndex", searchViewModelQuery.BuildSearchIndex(searchInputModel)].WithCookies(searchInputModel.Cookies);
            };

            Get["search-index-csv", "/search.csv"] = parameters =>
            {
                var searchInputModel = this.Bind<SearchInputModel>();

                return csvResponseQuery.BuildCsvResponse(searchViewModelQuery.BuiSearchIndexCsv(searchInputModel));
            };
        }
    }
}