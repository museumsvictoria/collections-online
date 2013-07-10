using System.Collections.Generic;
using System.Text;

namespace CollectionsOnline.Core.Extensions
{
    public static class StringExtensions
    {
        public static string Concatenate(this IEnumerable<string> source, string delimiter)
        {
            var s = new StringBuilder();

            foreach (var item in source)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    if (s.Length != 0)
                        s.Append(delimiter);

                    s.Append(item);
                }
            }

            return s.Length != 0 ? s.ToString() : null;
        }
    }
}
