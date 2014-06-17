using System.Collections.Generic;
using CollectionsOnline.Core.Models;
using IMu;

namespace CollectionsOnline.Import.Factories
{
    public interface IMediaFactory
    {
        Media Make(Map map);

        IList<Media> Make(IEnumerable<Map> maps);
    }
}