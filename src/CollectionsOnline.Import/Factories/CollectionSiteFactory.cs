using System;
using System.Globalization;
using System.Linq;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Extensions;
using IMu;
using Raven.Abstractions.Extensions;

namespace CollectionsOnline.Import.Factories
{
    public class CollectionSiteFactory : ICollectionSiteFactory
    {
        private readonly IPartiesNameFactory _partiesNameFactory;

        public CollectionSiteFactory(
            IPartiesNameFactory partiesNameFactory)
        {
            _partiesNameFactory = partiesNameFactory;
        }

        public CollectionSite Make(Map map, string discipline, string scientificGroup)
        {
            if (map != null &&
                !((string.Equals(discipline, "Palaeontology", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(scientificGroup, "Geology", StringComparison.OrdinalIgnoreCase) ||
                   scientificGroup.Contains("Zoology", StringComparison.OrdinalIgnoreCase))
                  && string.Equals(map.GetEncodedString("AdmPublishWebNoPassword"), "no",
                      StringComparison.OrdinalIgnoreCase)))
            {
                var collectionSite = new CollectionSite
                {
                    Irn = long.Parse(map.GetEncodedString("irn"))
                };

                if (!string.Equals(discipline, "Palaeontology", StringComparison.OrdinalIgnoreCase))
                {
                    // Site Code
                    collectionSite.SiteCode = new[]
                    {
                        map.GetEncodedString("SitSiteCode"),
                        map.GetEncodedString("SitSiteNumber")
                    }.Concatenate("");

                    // Precise locality
                    collectionSite.PreciseLocation = map.GetEncodedString("LocPreciseLocation");
                    collectionSite.MinimumElevation = map.GetEncodedString("LocElevationASLFromMt");
                    collectionSite.MaximumElevation = map.GetEncodedString("LocElevationASLToMt");
                }

                // Lat/Long
                
                var latlongMap = map.GetMaps("latlong").FirstOrDefault(x => string.Equals(x.GetEncodedString("LatPreferred_tab"), "yes", StringComparison.OrdinalIgnoreCase));
                if (latlongMap != null &&
                    !(string.Equals(discipline, "Palaeontology", StringComparison.OrdinalIgnoreCase) || string.Equals(scientificGroup, "Geology", StringComparison.OrdinalIgnoreCase)))
                {
                    var decimalLatitudes = (object[])latlongMap["LatLatitudeDecimal_nesttab"];
                    if (decimalLatitudes != null && decimalLatitudes.Any(x => x != null))
                        collectionSite.Latitudes.AddRange(decimalLatitudes.Where(x => x != null).Select(x => x.ToString()));

                    var decimalLongitudes = (object[])latlongMap["LatLongitudeDecimal_nesttab"];
                    if (decimalLongitudes != null && decimalLongitudes.Any(x => x != null))
                        collectionSite.Longitudes.AddRange(decimalLongitudes.Where(x => x != null).Select(x => x.ToString()));

                    collectionSite.GeodeticDatum = string.IsNullOrWhiteSpace(latlongMap.GetEncodedString("LatDatum_tab")) ? "WGS84" : latlongMap.GetEncodedString("LatDatum_tab");
                    collectionSite.SiteRadius = latlongMap.GetEncodedString("LatRadiusNumeric_tab");
                    collectionSite.GeoreferenceBy = _partiesNameFactory.Make(latlongMap.GetMap("determinedBy"));

                    if (DateTime.TryParseExact(latlongMap.GetEncodedString("LatDetDate0"), "dd/MM/yyyy", new CultureInfo("en-AU"), DateTimeStyles.None, out var georeferenceDate))
                        collectionSite.GeoreferenceDate = georeferenceDate.ToString("s");

                    collectionSite.GeoreferenceProtocol = latlongMap.GetEncodedString("LatLatLongDetermination_tab");
                    collectionSite.GeoreferenceSource = latlongMap.GetEncodedString("LatDetSource_tab");
                }

                // Locality
                var geoMap = map.GetMaps("geo").FirstOrDefault();
                if (geoMap != null)
                {
                    collectionSite.Ocean = geoMap.GetEncodedString("LocOcean_tab");
                    collectionSite.Continent = geoMap.GetEncodedString("LocContinent_tab");
                    collectionSite.Country = geoMap.GetEncodedString("LocCountry_tab");
                    collectionSite.State = geoMap.GetEncodedString("LocProvinceStateTerritory_tab");
                    collectionSite.District = geoMap.GetEncodedString("LocDistrictCountyShire_tab");
                    collectionSite.Town = geoMap.GetEncodedString("LocTownship_tab");
                    collectionSite.NearestNamedPlace = geoMap.GetEncodedString("LocNearestNamedPlace_tab");
                }

                // Geology site fields
                if (!string.Equals(discipline, "Tektites", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(discipline, "Meteorites", StringComparison.OrdinalIgnoreCase))
                {
                    collectionSite.GeologyEra = map.GetEncodedString("EraEra");
                    collectionSite.GeologyPeriod = map.GetEncodedString("EraAge1");
                    collectionSite.GeologyEpoch = map.GetEncodedString("EraAge2");
                    collectionSite.GeologyStage = map.GetEncodedString("EraMvStage");
                    collectionSite.GeologyGroup = map.GetEncodedStrings("EraMvGroup_tab").Concatenate(", ");
                    collectionSite.GeologyFormation = map.GetEncodedStrings("EraMvRockUnit_tab").Concatenate(", ");
                    collectionSite.GeologyMember = map.GetEncodedStrings("EraMvMember_tab").Concatenate(", ");
                    collectionSite.GeologyRockType = map.GetEncodedStrings("EraLithology_tab").Concatenate(", ");
                }

                return collectionSite;
            }

            return null;
        }
    }
}