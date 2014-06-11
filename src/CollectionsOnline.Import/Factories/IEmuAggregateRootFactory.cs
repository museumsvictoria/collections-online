using CollectionsOnline.Core.Models;
using IMu;

namespace CollectionsOnline.Import.Factories
{
    public interface IEmuAggregateRootFactory<out T> where T : EmuAggregateRoot
    {
        string ModuleName { get; }

        string[] Columns { get; }

        Terms Terms { get; }

        T MakeDocument(Map map);

        void RegisterAutoMapperMap();
    }
}