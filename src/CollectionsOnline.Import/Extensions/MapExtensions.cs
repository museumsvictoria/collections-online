using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private static string EncodeString(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return Encoding.GetEncoding("Windows-1252").GetString(Encoding.GetEncoding("ISO-8859-1").GetBytes(value)).Trim();

            return value;
        }
    }
}
