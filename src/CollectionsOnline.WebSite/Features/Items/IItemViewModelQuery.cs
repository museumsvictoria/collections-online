namespace CollectionsOnline.WebSite.Features.Items
{
    public interface IItemViewModelQuery
    {
        ItemViewTransformerResult BuildItem(string itemId);
    }
}