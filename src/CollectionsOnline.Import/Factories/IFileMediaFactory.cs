using CollectionsOnline.Core.Models;

namespace CollectionsOnline.Import.Factories
{
    public interface IFileMediaFactory
    {
        bool Make(ref FileMedia fileMedia, string originalFileExtension);
    }
}
