using CollectionsOnline.Core.Models;
using IMu;

namespace CollectionsOnline.Import.Factories
{
    public interface IMuseumLocationFactory
    {
        MuseumLocation GetMuseumLocation(Map map);
    }
}