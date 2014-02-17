using System;
using Nancy;
using Nancy.ModelBinding;
using NLog;

namespace CollectionsOnline.WebSite.Features.Search
{
    public class SearchModule : NancyModule
    {
        public SearchModule(            
            ISearchViewModelQuery searchViewModelQuery)            
        {
            Get["/search"] = parameters =>
            {
                var searchInputModel = this.Bind<SearchInputModel>();

                return View["search", searchViewModelQuery.BuildSearch(searchInputModel, Request)];
            };
        }
    }
}