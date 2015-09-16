using System.Configuration;
using CollectionsOnline.WebSite.Models;
using CollectionsOnline.WebSite.Queries;
using Nancy.ModelBinding;

namespace CollectionsOnline.WebSite.Modules.Api
{
    public class SearchApiModule : ApiModuleBase
    {
        public SearchApiModule(ISearchViewModelQuery searchViewModelQuery)
            : base("/search")
        {
            Get["search-api", ""] = parameters =>
            {
                var searchInputModel = this.Bind<SearchInputModel>();

                return BuildResponse(searchViewModelQuery.BuildSearchIndex(searchInputModel));
            };
        }
    }
}