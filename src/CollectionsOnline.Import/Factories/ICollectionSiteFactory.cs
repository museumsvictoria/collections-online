using CollectionsOnline.Core.Models;
using IMu;

namespace CollectionsOnline.Import.Factories
{
    public interface ICollectionSiteFactory
    {
        CollectionSite Make(Map map, string discipline, string scientificGroup);
    }
}