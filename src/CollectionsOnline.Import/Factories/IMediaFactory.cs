using System.Collections.Generic;
using CollectionsOnline.Core.Models;
using ImageProcessor.Imaging;
using IMu;

namespace CollectionsOnline.Import.Factories
{
    public interface IMediaFactory
    {
        Media Make(Map map, ResizeMode? thumbnailResizeMode = null);

        IList<Media> Make(IEnumerable<Map> maps, ResizeMode? thumbnailResizeMode = null);
    }
}