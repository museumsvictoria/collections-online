using System;
using System.Globalization;
using System.Linq;
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
        
        public MuseumLocation Make(string parentType, Map[] objectStatusMaps, Map[] partsMaps)
        {
            var museumLocation = new MuseumLocation() { DisplayStatus = DisplayStatus.NotOnDisplay };

            Map currentExhibitionObjectMap;

            bool FindValidExhibitionMap(Map x) =>
                string.Equals(x?.GetEncodedString("StaStatus"), "On display", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(x?.GetEncodedString("StaStatus"), "On loan", StringComparison.OrdinalIgnoreCase);
            
            // Look in parts for current exhibition object map if parent is conceptual type otherwise look in object status maps
            if (string.Equals(parentType, "conceptual", StringComparison.OrdinalIgnoreCase))
            {
                currentExhibitionObjectMap = partsMaps
                    .Select(x => x.GetMaps("objstatus"))
                    .Where(x => x.Any())
                    .SelectMany(x => x).FirstOrDefault(FindValidExhibitionMap);
            }
            else
            {
                currentExhibitionObjectMap = objectStatusMaps.FirstOrDefault(FindValidExhibitionMap);
            }
            
            var eventMap = currentExhibitionObjectMap?.GetMap("event");
            var venNameMap = eventMap?.GetMaps("venname").FirstOrDefault();
            var locationMap = currentExhibitionObjectMap?.GetMap("location");
            
            if (string.Equals(currentExhibitionObjectMap?.GetEncodedString("StaStatus"), "On display", 
                StringComparison.OrdinalIgnoreCase))
            {
                // Check event lookup then location lookup and return default museum location if nothing found
                museumLocation = this.MakeFromEventMap(eventMap) ?? this.MakeFromLocationMap(locationMap) ?? museumLocation;
            }

            // Check for on loan or for record that has incorrect StaStatus but correct On Loan location
            if (string.Equals(currentExhibitionObjectMap?.GetEncodedString("StaStatus"), "On loan",
                StringComparison.OrdinalIgnoreCase) || museumLocation.DisplayStatus == DisplayStatus.OnLoan)
            {
                // Set Start/End date
                museumLocation.DisplayStartDate = eventMap?.ParseDate("DatCommencementDate");
                museumLocation.DisplayEndDate = eventMap?.ParseDate("DatCompletionDate");
                
                // Get venue name
                museumLocation.DisplayStatus = DisplayStatus.OnLoan;
                museumLocation.Venue = venNameMap != null ? _partiesNameFactory.Make(venNameMap) : null;
                
                // If Start/End date is set and outside of current date then set to Not On Display
                if (!(DateTime.Now >= museumLocation.DisplayStartDate && DateTime.Now <= museumLocation.DisplayEndDate))
                {
                    museumLocation.DisplayStatus = DisplayStatus.NotOnDisplay;
                }
            }

            return museumLocation;
        }

        private MuseumLocation MakeFromLocationMap(Map map)
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

        private MuseumLocation MakeFromEventMap(Map map)
        {
            var eventNumber = map.GetEncodedString("EveEventNumber");

            return LocationDictionaries.Events.Where(ml => string.Equals(ml.Key, eventNumber, StringComparison.OrdinalIgnoreCase))
                .Select(ml => ml.Value)
                .FirstOrDefault();
        }
    }
}