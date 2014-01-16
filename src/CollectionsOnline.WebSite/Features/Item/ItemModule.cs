using Nancy;

namespace CollectionsOnline.WebSite.Features.Item
{
    public class ItemModule : NancyModule
    {
        public ItemModule(            
            IItemViewModelQuery itemViewModelQuery)            
        {
            Get["/items"] = parameters =>
            {
                return View["item", itemViewModelQuery.BuildItem()];
            };
        }
    }
}