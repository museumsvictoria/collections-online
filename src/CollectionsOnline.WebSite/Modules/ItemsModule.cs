using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Queries;
using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebSite.Modules
{
    public class ItemsModule : NancyModule
    {
        public ItemsModule(            
            IItemViewModelQuery itemViewModelQuery,
            IDocumentSession documentSession)            
        {
            Get["/items/{id}"] = parameters =>
            {
                var item = documentSession.Load<Item>("items/" + parameters.id as string);

                return (item == null || item.IsHidden) ? HttpStatusCode.NotFound : View["Items", itemViewModelQuery.BuildItem("items/" + parameters.id)];
            };
        }
    }
}