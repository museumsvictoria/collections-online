using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models;

namespace CollectionsOnline.WebSite.Queries
{
    public interface ICollectionViewModelQuery
    {
        CollectionIndexViewModel BuildCollectionIndex();

        Collection BuildCollection(string collectionId);
    }
}