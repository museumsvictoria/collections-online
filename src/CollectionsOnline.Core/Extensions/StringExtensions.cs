using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace CollectionsOnline.Core.Extensions
{
    public static class StringExtensions
    {
        public static string Concatenate(this IEnumerable<string> input, string delimiter)
        {
            var s = new StringBuilder();

            foreach (var item in input)
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

        public static string ToSentenceCase(this string input)
        {
            return Regex.Replace(input.ToLower(), @"(?<=(^|[.;:])\s*)[a-z]", (match) => match.Value.ToUpper());
        }
    }
}
