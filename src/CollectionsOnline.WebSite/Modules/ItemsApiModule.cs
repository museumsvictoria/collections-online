using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models;
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

                    return BuildResponse(Mapper.Map<IEnumerable<Item>, IEnumerable<ItemApiViewModel>>(items));
                };

            Get["items-api-by-id", "/{id}"] = parameters =>
                {
                    var item = documentSession.Load<Item>("items/" + parameters.id as string);

                    return (item == null || item.IsHidden) ? BuildErrorResponse(HttpStatusCode.NotFound, "Item {0} not found", parameters.id) : BuildResponse(Mapper.Map<Item, ItemApiViewModel>(item));
                };
        }
    }
}