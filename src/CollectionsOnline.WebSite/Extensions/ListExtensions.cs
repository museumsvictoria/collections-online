using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Models;

namespace CollectionsOnline.WebSite.Extensions
{
    public static class ListExtensions
    {
        public static IEnumerable<Media> WithThumbnails(this IList<Media> self)
        {
            return self.OfType<IHasThumbnail>().Cast<Media>();
        }
    }
}