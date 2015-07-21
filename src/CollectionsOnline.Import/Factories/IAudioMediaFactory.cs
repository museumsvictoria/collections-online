using CollectionsOnline.Core.Models;

namespace CollectionsOnline.Import.Factories
{
    public interface IAudioMediaFactory
    {
        bool Make(ref AudioMedia audioMedia, string originalFileExtension);
    }
}
