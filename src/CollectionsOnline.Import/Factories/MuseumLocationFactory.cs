using System;
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

            return Constants.MuseumLocations.ContainsKey(locationKey) ? Constants.MuseumLocations[locationKey] : null;
        }
    }
}