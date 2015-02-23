using System.Linq;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebSite.Modules
{
    public class ItemsApiModule : BaseApiModule
    {
        public ItemsApiModule(IDocumentSession documentSession)
            : base("/items")
        {
            Get["items-api-index", "/"] = parameters =>
                {
                    var items = documentSession.Advanced
                        .DocumentQuery<Item, Combined>()
                        .WhereEquals("Type", "Item")
                        .Statistics(out Statistics)
                        .Skip(Offset)
                        .Take(Limit)
                        .ToList();

                    return BuildResponse(items);
                };

            Get["items-api-by-id", "/{id}"] = parameters =>
                {
                    var item = documentSession.Load<Item>("items/" + parameters.id as string);

                    return (item == null || item.IsHidden) ? BuildErrorResponse(HttpStatusCode.NotFound, "Item {0} not found", parameters.id) : BuildResponse(item);
                };
        }
    }
}