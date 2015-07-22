using CollectionsOnline.Core.Models;

namespace CollectionsOnline.Import.Factories
{
    public interface ISummaryFactory
    {
        string Make(Article article);

        string Make(Item item);

        string Make(Species species);

        string Make(Specimen specimen);
    }
}