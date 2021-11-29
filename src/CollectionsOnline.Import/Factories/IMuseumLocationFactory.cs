using CollectionsOnline.Core.Models;
using IMu;

namespace CollectionsOnline.Import.Factories
{
    public interface IMuseumLocationFactory
    {
        MuseumLocation Make(string parentType, Map[] objectStatusMaps, Map[] partsMaps);
        
        MuseumLocation MakeFromLocationMap(Map map);

        MuseumLocation MakeFromEventMap(Map map);
    }
}