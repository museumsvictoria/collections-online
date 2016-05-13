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
            if (string.IsNullOrWhiteSpace(input))
                return input;

            return Regex.Replace(input.ToLower(), @"(?<=(^|[.;:])\s*)[a-z]", (match) => match.Value.ToUpper());
        }

        public static string Truncate(this string input, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            return input.Length <= maxLength ? input : input.TrimToMaxLength(maxLength, ' ') + " ...";
        }

        public static string TrimToMaxLength(this string input, int maxLength, char separator)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            if (maxLength > 0)
            {
                var builder = new StringBuilder();

                // Split string into words
                foreach (var word in input.Split(separator))
                {
                    if (builder.Length + word.Length > maxLength)
                    {
                        if (builder.Length == 0)
                            builder.Append(word);

                        break;
                    }

                    builder.Append(word + separator);
                }

                input = builder.ToString();
            }

            return input;
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

        public static string CleanForMultiFacets(this string input)
        {
            // Replace commas
            input = input.Replace(",", " and");

            // Replace em-dash
            input = input.Replace("–", "-");

            // Remove plural shorthand
            input = input.Replace("(s)", "");

            return input;
        }

        public static string ReplaceNonWordWithDashes(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            // Remove Apostrophe Tags
            input = Regex.Replace(input, "[’'“”\"&]{1,}", "", RegexOptions.None);

            // Replaces all non-alphanumeric character by a space
            var builder = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                builder.Append(char.IsLetterOrDigit(input[i]) ? input[i] : ' ');
            }

            input = builder.ToString();

            // Replace multiple spaces into a single dash
            input = Regex.Replace(input, "[ ]{1,}", "-", RegexOptions.None);

            // Trim unwanted dashes
            input = input.Trim('-');

            return input;
        }

        public static string RemoveLineBreaks(this string input, string delimiter = " ")
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            return Regex
                .Replace(input, @"\r\n?|\n", delimiter)
                .Trim();
        }

        public static string RemoveNonWordCharacters(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            return Regex.Replace(input, @"[^\w\s]", string.Empty);
        }
    }
}
