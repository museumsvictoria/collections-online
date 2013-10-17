using System.Linq;
using System.Web;
using CollectionsOnline.Core.Models;
using Nancy;
using Nancy.Responses.Negotiation;
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
                    var items = documentSession
                        .Query<Item>()
                        .Statistics(out Statistics)
                        .Skip(Offset)
                        .Take(Size)
                        .ToList();

                    return Response.AsJson(items);
                };

            Get["/{itemId}"] = parameters =>
                {
                    string itemId = parameters.itemId;
                    var item = documentSession
                        .Load<Item>("items/" + itemId);

                    if (item == null)
                    {
                        // TODO: Throw Error
                    }

                    return Response.AsJson(item);
                };
        }
    }
}