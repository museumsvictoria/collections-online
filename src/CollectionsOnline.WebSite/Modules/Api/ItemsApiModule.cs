using AutoMapper;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models.Api;
using CollectionsOnline.WebSite.Queries;
using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebSite.Modules.Api
{
    public class ItemsApiModule : ApiModuleBase
    {
        public ItemsApiModule(
            IDocumentSession documentSession,
            IItemViewModelQuery itemViewModelQuery)
            : base("/items")
        {
            Get["items-api-index", "/"] = parameters =>
                {
                    var apiViewModel = itemViewModelQuery.BuildItemApiIndex(ApiInputModel);

                    return BuildResponse(apiViewModel.Results, apiPageInfo: apiViewModel.ApiPageInfo);
                };

            Get["items-api-by-id", "/{id}"] = parameters =>
                {
                    var item = documentSession.Load<Item>("items/" + parameters.id as string);
                    
                    return (item == null || item.IsHidden) ? HttpStatusCode.NotFound : BuildResponse(Mapper.Map<Item, ItemApiViewModel>(item));
                };
        }
    }
}