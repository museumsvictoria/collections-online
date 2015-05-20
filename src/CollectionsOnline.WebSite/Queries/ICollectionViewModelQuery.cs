using CollectionsOnline.WebSite.Models;
using CollectionsOnline.WebSite.Transformers;

namespace CollectionsOnline.WebSite.Queries
{
    public interface ICollectionViewModelQuery
    {
        CollectionIndexViewModel BuildCollectionIndex();

        CollectionViewTransformerResult BuildCollection(string collectionId);
    }
}