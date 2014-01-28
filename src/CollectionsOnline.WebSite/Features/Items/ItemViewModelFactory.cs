using CollectionsOnline.Core.Models;

namespace CollectionsOnline.WebSite.Features.Items
{
    public class ItemViewModelFactory : IItemViewModelFactory
    {
        public ItemViewModel MakeViewModel(Item item)
        {
            var itemViewModel = new ItemViewModel
                {
                    Item = item
                };

            return itemViewModel;
        }
    }
}