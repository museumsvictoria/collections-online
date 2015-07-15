using System.Collections.Generic;

namespace CollectionsOnline.Core.Extensions
{
    public static class ListExtensions
    {
        public static void AddRangeUnique<T>(this IList<T> self, IEnumerable<T> items)
        {
            foreach(var item in items)
                if (!self.Contains(item))
                    self.Add(item);
        }
    }
}