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
            if (map?.GetEncodedString("LocLocationType") == null)
                return null;

            if (map.GetEncodedString("LocLocationType").Contains("holder", StringComparison.OrdinalIgnoreCase))
                return Make(map.GetMap("location"));
            
            var locationKey = new Tuple<string, string, string, string>(
                map.GetEncodedString("LocLevel1") ?? string.Empty,
                map.GetEncodedString("LocLevel2") ?? string.Empty,
                map.GetEncodedString("LocLevel3") ?? string.Empty,
                map.GetEncodedString("LocLevel4") ?? string.Empty);

            var location = Constants.MuseumLocations.Where(ml =>
                StringComparer.OrdinalIgnoreCase.Equals(ml.Key.Item1, locationKey.Item1) &&
                StringComparer.OrdinalIgnoreCase.Equals(ml.Key.Item2, locationKey.Item2) &&
                StringComparer.OrdinalIgnoreCase.Equals(ml.Key.Item3, locationKey.Item3) &&
                (StringComparer.OrdinalIgnoreCase.Equals(ml.Key.Item4, locationKey.Item4) || ml.Key.Item4 == null))
                .Where(ml => !Constants.MuseumLocationsToExclude.Any(mle =>
                StringComparer.OrdinalIgnoreCase.Equals(mle.Item1, locationKey.Item1) &&
                StringComparer.OrdinalIgnoreCase.Equals(mle.Item2, locationKey.Item2) &&
                StringComparer.OrdinalIgnoreCase.Equals(mle.Item3, locationKey.Item3) &&
                StringComparer.OrdinalIgnoreCase.Equals(mle.Item4, locationKey.Item4)))
                .Select(ml => new { ml.Key, ml.Value })
                .FirstOrDefault();

            return location?.Value;
        }
    }
}