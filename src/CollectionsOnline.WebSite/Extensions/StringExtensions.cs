using CollectionsOnline.Core.Extensions;

namespace CollectionsOnline.WebSite.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Convert a string in a view into a string suitable for use as a css class
        /// </summary>
        /// <param name="input">string to convert</param>
        /// <returns>string suitable for use as a class</returns>
        public static string ToClass(this string input)
        {
            return input
                .ToLowerInvariant()
                .Replace("&", "and")
                .ReplaceNonWordWithDashes();
        }
    }
}