using CollectionsOnline.WebSite.Features.Search;
using Nancy;

namespace CollectionsOnline.WebSite.Features.Item
{
    public interface IItemViewModelQuery
    {
        ItemViewModel BuildItem();
    }
}