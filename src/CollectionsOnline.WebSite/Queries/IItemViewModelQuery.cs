using CollectionsOnline.WebSite.Models.Api;
using CollectionsOnline.WebSite.Transformers;

namespace CollectionsOnline.WebSite.Queries
{
    public interface IItemViewModelQuery
    {
        ItemViewTransformerResult BuildItem(string itemId);

        ApiViewModel BuildItemApiIndex(ApiInputModel apiInputModel);
    }
}