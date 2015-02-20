using System.Linq;
using AutoMapper;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebApi.Models;
using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebApi.Modules
{
    public class ItemModule : WebApiModule
    {
        public ItemModule(IDocumentSession documentSession)
            : base("/items")
        {
            Get["items-index", "/"] = parameters =>
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

            Get["items-by-id", "/{id}"] = parameters =>
                {
                    var item = documentSession.Load<Item>("items/" + parameters.id as string);

                    return (item == null || item.IsHidden) ? BuildErrorResponse(HttpStatusCode.NotFound, "Item {0} not found", parameters.id) : BuildResponse(Mapper.Map<Item, ItemViewModel>(item));
                };
        }
    }
}