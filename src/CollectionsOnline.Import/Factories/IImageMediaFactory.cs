using CollectionsOnline.Core.Models;
using ImageProcessor.Imaging;

namespace CollectionsOnline.Import.Factories
{
    public interface IImageMediaFactory
    {
        bool Make(ref ImageMedia imageMedia, ResizeMode? thumbnailResizeMode);
    }
}
