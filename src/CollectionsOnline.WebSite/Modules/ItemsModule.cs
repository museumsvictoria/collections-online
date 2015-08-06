using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Factories;
using CollectionsOnline.WebSite.Queries;
using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebSite.Modules
{
    public class ItemsModule : NancyModule
    {
        public ItemsModule(            
            IItemViewModelQuery itemViewModelQuery,
            IDocumentSession documentSession,
            IMetadataViewModelFactory metadataViewModelFactory)            
        {
            Get["/items/{id}"] = parameters =>
            {
                var item = documentSession.Load<Item>("items/" + parameters.id as string);

                if (item == null || item.IsHidden) 
                    return HttpStatusCode.NotFound;

                ViewBag.metadata = metadataViewModelFactory.Make(item);

                return View["Items", itemViewModelQuery.BuildItem("items/" + parameters.id)];
            };
        }
    }
}