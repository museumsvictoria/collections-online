using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Models;

namespace CollectionsOnline.WebSite.Extensions
{
    public static class ListExtensions
    {
        public static IList<T> GetMultimedia<T>(this IList<T> self)
        {
            return self.Where(x => !(x is FileMedia) && !(x is UriMedia)).ToList();
        }

        public static IList<T> GetFiles<T>(this IList<T> self)
        {
            return self.Where(x => x is FileMedia).ToList();
        }

        public static IList<T> GetUris<T>(this IList<T> self)
        {
            return self.Where(x => x is UriMedia).ToList();
        }
    }
}