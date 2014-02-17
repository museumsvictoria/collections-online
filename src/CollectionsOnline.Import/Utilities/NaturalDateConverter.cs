using System.Linq;
using System.Text.RegularExpressions;

namespace CollectionsOnline.Import.Utilities
{
    public static class NaturalDateConverter
    {
        public static string ConvertToYearSpan(string date, int yearSpan = 10)
        {
            if (!string.IsNullOrWhiteSpace(date))
            {
                // Only parses 4 digit year dates at present.
                var years = Regex.Matches(date, @"\d{4}").Cast<Match>().Select(x => int.Parse(x.Value)).ToList();
                if (years.Any())
                {
                    var yearAverage = ((int)years.Average());

                    var decadeStart = yearAverage - (yearAverage % yearSpan);
                    var decadeEnd = (yearAverage - (yearAverage % yearSpan)) + yearSpan;

                    return string.Format("{0} - {1}", decadeStart, decadeEnd);
                }
            }
            
            return null;
        }
    }
}