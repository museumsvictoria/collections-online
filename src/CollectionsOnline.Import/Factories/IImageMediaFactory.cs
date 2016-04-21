using CollectionsOnline.Core.Models;

namespace CollectionsOnline.Import.Factories
{
    public interface IImageMediaFactory
    {
        bool Make(ref ImageMedia imageMedia);
    }
}
