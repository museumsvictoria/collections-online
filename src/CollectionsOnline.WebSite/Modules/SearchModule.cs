using CollectionsOnline.WebSite.Models;
using CollectionsOnline.WebSite.Queries;
using Nancy;
using Nancy.ModelBinding;

namespace CollectionsOnline.WebSite.Modules
{
    public class SearchModule : NancyModule
    {
        public SearchModule(
            ISearchViewModelQuery searchViewModelQuery)
        {
            Get["/search"] = parameters =>
            {
                var searchInputModel = this.Bind<SearchInputModel>();

                return View["search", searchViewModelQuery.BuildSearch(searchInputModel)];
            };
        }
    }
}