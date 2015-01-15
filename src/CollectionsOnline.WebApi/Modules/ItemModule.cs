using System.Linq;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebApi.Modules
{
    public class ItemModule : BaseModule
    {
        public ItemModule(IDocumentSession documentSession)
            : base("/items")
        {
            Get["/"] = parameters =>
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

            Get["/{itemId}"] = parameters =>
                {
                    string itemId = parameters.itemId;
                    var item = documentSession
                        .Load<Item>("items/" + itemId);

                    return item == null ? BuildErrorResponse(HttpStatusCode.NotFound, "Item {0} not found", itemId) : BuildResponse(item);
                };
        }
    }
}