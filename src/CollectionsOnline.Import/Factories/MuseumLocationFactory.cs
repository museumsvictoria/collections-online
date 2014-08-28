using System;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Models;
using IMu;
using CollectionsOnline.Core.Extensions;

namespace CollectionsOnline.Import.Factories
{
    public class MuseumLocationFactory : IMuseumLocationFactory
    {
        public MuseumLocation Make(Map map)
        {
            if (map == null || map.GetString("LocLocationType") == null)
                return null;

            if(map.GetString("LocLocationType").Contains("holder", StringComparison.OrdinalIgnoreCase))
                return Make(map.GetMap("location"));
            
            var locationKey = new Tuple<string, string, string, string>(
                map.GetString("LocLevel1") ?? string.Empty,
                map.GetString("LocLevel2") ?? string.Empty,
                map.GetString("LocLevel3") ?? string.Empty,
                map.GetString("LocLevel4") ?? string.Empty);

            return Constants.MuseumLocations.ContainsKey(locationKey) ? Constants.MuseumLocations[locationKey] : null;
        }
    }
}