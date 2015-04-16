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
            ITermSearchViewModelQuery termSearchViewModelQuery)
        {
            Get["/search"] = parameters =>
            {
                var searchInputModel = this.Bind<SearchInputModel>();

                return View["search", searchViewModelQuery.BuildSearch(searchInputModel)];
            };

            Get["/search/term/locality"] = parameters =>
            {
                var termSearchInputModel = this.Bind<TermSearchInputModel>();

                return Response.AsJson(termSearchViewModelQuery.BuildLocalityTermSearch(termSearchInputModel));
            };

            Get["/search/term/keyword"] = parameters =>
            {
                var termSearchInputModel = this.Bind<TermSearchInputModel>();

                return Response.AsJson(termSearchViewModelQuery.BuildKeywordTermSearch(termSearchInputModel));
            };
        }
    }
}