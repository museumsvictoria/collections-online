using System.Dynamic;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using Nancy;
using Nancy.ModelBinding;
using Raven.Client;

namespace CollectionsOnline.WebSite.Features.Search
{
    public class SearchModule : NancyModule
    {
        public SearchModule(            
            ISearchViewModelQuery searchViewModelQuery)            
        {
            Get["/"] = parameters =>
            {
                var searchInputModel = this.Bind<SearchInputModel>();

                return View["search", searchViewModelQuery.BuildSearch(searchInputModel)];
            };
        }
    }
}