using CollectionsOnline.Core.Models;
using IMu;

namespace CollectionsOnline.Import.Factories
{
    public interface ICollectionSiteFactory
    {
        CollectionSite Make(Map map, string type, string discipline, string scientificGroup);
    }
}