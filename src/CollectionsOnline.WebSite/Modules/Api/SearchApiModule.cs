using CollectionsOnline.WebSite.Models.Api;
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
                var searchApiInputModel = this.Bind<SearchApiInputModel>();
                
                var apiViewModel = searchViewModelQuery.BuildSearchApi(searchApiInputModel, ApiInputModel);

                return BuildResponse(apiViewModel.Results, apiPageInfo: apiViewModel.ApiPageInfo);
            };
        }
    }
}