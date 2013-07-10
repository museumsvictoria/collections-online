using IMu;

namespace CollectionsOnline.Import.Helpers
{
    public interface IImportHelper<out T>
    {
        string MakeModuleName();

        string[] MakeColumns();

        Terms MakeTerms();

        T MakeDocument(Map map);
    }
}