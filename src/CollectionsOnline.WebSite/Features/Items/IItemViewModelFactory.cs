using CollectionsOnline.Core.Models;

namespace CollectionsOnline.WebSite.Features.Items
{
    public interface IItemViewModelFactory
    {
        ItemViewModel MakeViewModel(Item item);
    }
}