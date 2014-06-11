using CollectionsOnline.Core.Models;
using IMu;

namespace CollectionsOnline.Import.Factories
{
    public interface ITaxonomyFactory
    {
        Taxonomy Make(Map map);
    }
}