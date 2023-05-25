using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using CollectionsOnline.Core.Extensions;
using IMu;

namespace CollectionsOnline.Import.Extensions
{
    public static class MapExtensions
    {
        public static string GetEncodedString(this Map map, string name)
        {
            return EncodeString(map.GetString(name));
        }

        public static IList<string> GetEncodedStrings(this Map map, string name)
        {
            var mapStrings = map.GetStrings(name);

            if (mapStrings != null && mapStrings.Any())
                return mapStrings
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(EncodeString)
                    .ToList();

            return new List<string>();
        }

        public static string GetCleanEncodedString(this Map map, string name)
        {
            return EncodeString(map.GetString(name)).RemoveNonWordCharacters();
        }

        private static string EncodeString(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return Encoding.GetEncoding("Windows-1252").GetString(Encoding.GetEncoding("ISO-8859-1").GetBytes(value)).Trim();

            return value;
        }
        
        public static DateTime? ParseDate(this Map map, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            var date = map.GetEncodedString(name);
            
            if (DateTime.TryParseExact(date, "dd/MM/yyyy", new CultureInfo("en-AU"), DateTimeStyles.None, out var parsedDate))
            {
                return parsedDate;
            }
            else if (DateTime.TryParseExact(date, "/MM/yyyy", new CultureInfo("en-AU"), DateTimeStyles.None, out parsedDate))
            {
                return parsedDate;
            }
            else if (DateTime.TryParseExact(date, "yyyy", new CultureInfo("en-AU"), DateTimeStyles.None, out parsedDate))
            {
                return parsedDate;
            }

            return null;
        }
    }
}
