using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using CollectionsOnline.Core.Extensions;

namespace CollectionsOnline.Core.Factories
{
    /// <summary>
    /// Stolen from RacoonBlog, with some slight modifications.
    /// </summary>
    public class SlugFactory : ISlugFactory
    {
        public string MakeSlug(string value, int maxLength = 0)
        {
            if (value == null) return value;

            // 1 - Strip diacritical marks using Michael Kaplan's function or equivalent
            value = RemoveDiacritics(value);

            // 2 - Lowercase the string for canonicalization
            value = value.ToLowerInvariant();

            // 3 - Replace appersands with and
            value = value.Replace("&", "and");

            // 4 - Replace all the non-word characters with dashes
            value = value.ReplaceNonWordWithDashes();

            // 5 - Trim to the max character length allowed, include entire words
            value = value.TrimToMaxLength(maxLength, '-');

            // 6 - Trim the string of leading/trailing whitespace
            value = value.Trim(' ', '-');

            return value;
        }

        // http://blogs.msdn.com/michkap/archive/2007/05/14/2629747.aspx
        /// <summary>
        /// Strips the value from any non English character by replacing those with their English equivalent.
        /// </summary>
        /// <param name="value">The string to normalize.</param>
        /// <returns>A string where all characters are part of the basic English ANSI encoding.</returns>
        /// <seealso cref="http://stackoverflow.com/questions/249087/how-do-i-remove-diacritics-accents-from-a-string-in-net"/>
        private string RemoveDiacritics(string value)
        {
            string stFormD = value.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }
    }
}