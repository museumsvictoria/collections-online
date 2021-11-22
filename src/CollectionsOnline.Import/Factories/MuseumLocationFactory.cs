using System;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Extensions;
using IMu;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Import.Config;

namespace CollectionsOnline.Import.Factories
{
    public class MuseumLocationFactory : IMuseumLocationFactory
    {
        private readonly IPartiesNameFactory _partiesNameFactory;

        public MuseumLocationFactory(IPartiesNameFactory partiesNameFactory)
        {
            _partiesNameFactory = partiesNameFactory;
        }
        
        public MuseumLocation Make(string parentType, Map[] exhibitionObjectMaps, Map[] partsMaps)
        {
            var museumLocation = new MuseumLocation() { DisplayStatus = DisplayStatus.NotOnDisplay };
            
            // Find correct exhibition object reference
            if (string.Equals(parentType, "conceptual", StringComparison.OrdinalIgnoreCase))
            {
                // Look in reverse linked children for event/location i.e. children map
                var exhibitionObjectMap = partsMaps.Select(x => x.GetMaps("exhobj")).Where(x => x.Any());
            }
            else
            {
                var exhibitionObjectMap = exhibitionObjectMaps.FirstOrDefault();
                var eventMap = exhibitionObjectMap?.GetMap("event");
                var venNameMap = eventMap?.GetMaps("venname").FirstOrDefault();
                var locationMap = exhibitionObjectMap?.GetMap("location");
                
                if (string.Equals(exhibitionObjectMap.GetEncodedString("StaStatus"), "On display", 
                    StringComparison.OrdinalIgnoreCase))
                {
                    // Check event lookup
                    museumLocation = this.MakeFromEventMap(eventMap) ?? this.MakeFromLocationMap(locationMap);
                }
                
                // Check for on loan or for record that has incorrect StaStatus but correct On Loan location
                if (string.Equals(exhibitionObjectMap.GetEncodedString("StaStatus"), "On loan",
                    StringComparison.OrdinalIgnoreCase) || museumLocation.DisplayStatus == DisplayStatus.OnLoan)
                {
                    // Get venue name and event title for Venue + Gallery
                    museumLocation.DisplayStatus = DisplayStatus.OnLoan;
                    museumLocation.Venue = venNameMap != null ? _partiesNameFactory.Make(venNameMap) : null;
                    museumLocation.Gallery = eventMap?.GetEncodedString("EveEventTitle");
                }
            }

            return museumLocation;
        }

        public MuseumLocation MakeFromLocationMap(Map map)
        {
            if (map?.GetEncodedString("LocLocationType") == null)
                return null;

            if (map.GetEncodedString("LocLocationType").Contains("holder", StringComparison.OrdinalIgnoreCase))
                return MakeFromLocationMap(map.GetMap("location"));
            
            var locationKey = new Tuple<string, string, string, string>(
                map.GetEncodedString("LocLevel1") ?? string.Empty,
                map.GetEncodedString("LocLevel2") ?? string.Empty,
                map.GetEncodedString("LocLevel3") ?? string.Empty,
                map.GetEncodedString("LocLevel4") ?? string.Empty);

            var location = LocationDictionaries.Locations.Where(ml =>
                    (ml.Key.Item1 == "*" || StringComparer.OrdinalIgnoreCase.Equals(ml.Key.Item1, locationKey.Item1)) &&
                    (ml.Key.Item2 == "*" || StringComparer.OrdinalIgnoreCase.Equals(ml.Key.Item2, locationKey.Item2)) &&
                    (ml.Key.Item3 == "*" || StringComparer.OrdinalIgnoreCase.Equals(ml.Key.Item3, locationKey.Item3)) &&
                    (ml.Key.Item4 == "*" || StringComparer.OrdinalIgnoreCase.Equals(ml.Key.Item4, locationKey.Item4)))
                .Where(ml => !LocationDictionaries.LocationsToExclude.Any(mle =>
                    StringComparer.OrdinalIgnoreCase.Equals(mle.Item1, locationKey.Item1) &&
                    StringComparer.OrdinalIgnoreCase.Equals(mle.Item2, locationKey.Item2) &&
                    StringComparer.OrdinalIgnoreCase.Equals(mle.Item3, locationKey.Item3) &&
                    StringComparer.OrdinalIgnoreCase.Equals(mle.Item4, locationKey.Item4)))
                .Select(ml => new { ml.Key, ml.Value })
                .OrderByDescending(x => x.Key.Item1)
                .ThenByDescending(x => x.Key.Item2)
                .ThenByDescending(x => x.Key.Item3)
                .ThenByDescending(x => x.Key.Item4)
                .ToList();

            return location.FirstOrDefault()?.Value;
        }

        public MuseumLocation MakeFromEventMap(Map map)
        {
            var irn = map.GetEncodedString("irn");

            return LocationDictionaries.Events.Where(ml => string.Equals(ml.Key, irn, StringComparison.OrdinalIgnoreCase))
                .Select(ml => ml.Value)
                .FirstOrDefault();
        }
    }
}