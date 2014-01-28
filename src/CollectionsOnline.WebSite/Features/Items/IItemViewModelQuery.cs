namespace CollectionsOnline.WebSite.Features.Items
{
    public interface IItemViewModelQuery
    {
        ItemViewModel BuildItem(string itemId);
    }
}