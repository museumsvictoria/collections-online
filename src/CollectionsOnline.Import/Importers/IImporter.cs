using IMu;

namespace CollectionsOnline.Import.Importers
{
    public interface IImporter<out T>
    {
        string ModuleName { get; }

        string[] Columns { get; }

        Terms Terms { get; }

        T MakeDocument(Map map);
    }
}