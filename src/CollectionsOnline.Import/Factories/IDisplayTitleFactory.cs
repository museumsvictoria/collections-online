using CollectionsOnline.Core.Models;

namespace CollectionsOnline.Import.Factories
{
    public interface IDisplayTitleFactory
    {
        string Make(Specimen specimen);
    }
}