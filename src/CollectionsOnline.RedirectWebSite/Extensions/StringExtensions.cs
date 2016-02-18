using System;

namespace CollectionsOnline.RedirectWebSite.Extensions
{
    public static class StringExtensions
    {
        public static bool Contains(this string input, string valueToCheck, StringComparison comparisonType)
        {
            if (string.IsNullOrWhiteSpace(input) || string.IsNullOrWhiteSpace(valueToCheck))
                return false;

            return input.IndexOf(valueToCheck, comparisonType) >= 0;
        }
    }
}
