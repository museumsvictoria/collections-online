using System;
using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Extensions;
using IMu;
using CollectionsOnline.Core.Extensions;

namespace CollectionsOnline.Import.Factories
{
    public class MuseumLocationFactory : IMuseumLocationFactory
    {
        public MuseumLocation Make(Map map)
        {
            if (map == null || map.GetEncodedString("LocLocationType") == null)
                return null;

            if (map.GetEncodedString("LocLocationType").Contains("holder", StringComparison.OrdinalIgnoreCase))
                return Make(map.GetMap("location"));
            
            var locationKey = new Tuple<string, string, string, string>(
                map.GetEncodedString("LocLevel1") ?? string.Empty,
                map.GetEncodedString("LocLevel2") ?? string.Empty,
                map.GetEncodedString("LocLevel3") ?? string.Empty,
                map.GetEncodedString("LocLevel4") ?? string.Empty);

            var location = Constants.MuseumLocations.Where(x =>
                StringComparer.OrdinalIgnoreCase.Equals(x.Key.Item1, locationKey.Item1) &&
                StringComparer.OrdinalIgnoreCase.Equals(x.Key.Item2, locationKey.Item2) &&
                StringComparer.OrdinalIgnoreCase.Equals(x.Key.Item3, locationKey.Item3) &&
                (StringComparer.OrdinalIgnoreCase.Equals(x.Key.Item4, locationKey.Item4) || x.Key.Item4 == null))
                .Select(x => new { x.Key, x.Value })
                .FirstOrDefault();

            return location != null ? location.Value : null;
        }
    }
}