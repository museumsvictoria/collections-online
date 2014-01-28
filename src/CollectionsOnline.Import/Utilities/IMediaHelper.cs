using CollectionsOnline.Core.Models;
using ImageResizer;

namespace CollectionsOnline.Import.Utilities
{
    public interface IMediaHelper
    {
        bool Save(long irn, FileFormatType fileFormat, ResizeSettings resizeSettings, string derivative = null);
    }
}
