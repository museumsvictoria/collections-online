using IMu;

namespace CollectionsOnline.Import.Factories
{
    public interface IDocumentFactory<out T>
    {
        string MakeModuleName();

        string[] MakeColumns();

        Terms MakeTerms();

        T MakeDocument(Map map);
    }
}