using System;
using System.Linq;
using IMu;
using CollectionsOnline.Core.Extensions;

namespace CollectionsOnline.Import.Factories
{
    public class LocationFactory : ILocationFactory
    {
        public string GetLocation(Map map)
        {
            if (map == null)
                return null;

            if(map.GetString("LocLocationType").Contains("holder", StringComparison.OrdinalIgnoreCase))
                return GetLocation(map.GetMap("location"));
            
            return new[]
            {
                map.GetString("LocLevel1"),
                map.GetString("LocLevel2"),
                map.GetString("LocLevel3"),
                map.GetString("LocLevel4")
            }.Concatenate(", ");
        }
    }
}