using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CollectionsOnline.Core.Extensions
{
    public static class StringExtensions
    {
        public static string Concatenate(this IEnumerable<string> input, string delimiter)
        {
            var s = new StringBuilder();

            if (input != null)
            {
                foreach (var item in input)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        if (s.Length != 0)
                            s.Append(delimiter);

                        s.Append(item);
                    }
                }
            }

            return s.Length != 0 ? s.ToString() : null;
        }

        public static string ToSentenceCase(this string input)
        {
            return Regex.Replace(input.ToLower(), @"(?<=(^|[.;:])\s*)[a-z]", (match) => match.Value.ToUpper());
        }

        public static string Truncate(this string input, int maxChars)
        {
            return input.Length <= maxChars ? input : input.Substring(0, maxChars) + " ..";
        }

        public static bool Contains(this string input, string valueToCheck, StringComparison comparisonType)
        {
            if (string.IsNullOrWhiteSpace(input) || string.IsNullOrWhiteSpace(valueToCheck))
                return false;

            return input.IndexOf(valueToCheck, comparisonType) >= 0;
        }

        public static bool Contains(this IEnumerable<string> source, string value, StringComparison comparisonType)
        {
            var collection = source as ICollection<string>;
            if (collection == null || !collection.Any())
                return false;

            return source.Any(element => element.Contains(value, comparisonType));
        }

        public static string Remove(this string input, IEnumerable<string> values)
        {
            if (values != null && !string.IsNullOrWhiteSpace(input))
            {
                foreach (var value in values)
                {
                    input = Regex.Replace(input, value, "", RegexOptions.IgnoreCase).Trim();
                }
            }

            return input;
        }
    }
}
