using System;
using System.Linq;
using CollectionsOnline.Core.Models;
using Geocoding;
using Geocoding.Google;
using NLog;
using Raven.Client;

namespace CollectionsOnline.Import.Imports
{
    public class GeocodeImport : IImport
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly IDocumentStore _documentStore;
        private readonly IGeocoder _geocoder;

        public GeocodeImport(
            IDocumentStore documentStore,
            IGeocoder geocoder)
        {
            _documentStore = documentStore;
            _geocoder = geocoder;
        }

        public void Run()
        {
            _log.Debug("Beginning Geocoding of associations");

            var associationCount = 0;
            var geocodeCount = 0;

            while (true)
            {
                using (var documentSession = _documentStore.OpenSession())
                {
                    if (ImportCanceled())
                        return;

                    var item = documentSession
                        .Query<Item>()
                        .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                        .FirstOrDefault(x => x.Associations.Any(y => y.GeocodeStatus == GeocodeStatus.UnAttempted));

                    if (item == null)
                        break;

                    foreach (var association in item.Associations.Where(x => x.GeocodeStatus == GeocodeStatus.UnAttempted))
                    {
                        // Try find item association that has the same place key and geocoding has been attempted
                        var similarItem = documentSession
                            .Query<Item>()
                            .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                            .FirstOrDefault(x => x.Associations.Any(y => y.GeocodeStatus != GeocodeStatus.UnAttempted && y.PlaceKey == association.PlaceKey));                        

                        // No similar association found, geocode place name.
                        if (similarItem == null && !string.IsNullOrWhiteSpace(association.Place))
                        {
                            try
                            {
                                var addresses = _geocoder.Geocode(association.Place);

                                geocodeCount++;

                                var address = addresses.FirstOrDefault(x =>
                                    ((GoogleAddress)x).Type == GoogleAddressType.Locality ||
                                    ((GoogleAddress)x).Type == GoogleAddressType.SubLocality ||
                                    ((GoogleAddress)x).Type == GoogleAddressType.Neighborhood ||
                                    ((GoogleAddress)x).Type == GoogleAddressType.NaturalFeature ||
                                    ((GoogleAddress)x).Type == GoogleAddressType.Park) as GoogleAddress;

                                if (address != null)
                                {
                                    // Find locality
                                    var locality = address.Components.FirstOrDefault(x => x.Types.Any(y =>
                                        y == GoogleAddressType.Locality ||
                                        y == GoogleAddressType.SubLocality ||
                                        y == GoogleAddressType.Neighborhood ||
                                        y == GoogleAddressType.NaturalFeature ||
                                        y == GoogleAddressType.Park));

                                    if (locality != null)
                                        association.Locality = locality.LongName;

                                    var country = address.Components.FirstOrDefault(x => x.Types.Any(y => y == GoogleAddressType.Country));

                                    if (country != null)
                                        association.Country = country.LongName;

                                    association.Latitude = address.Coordinates.Latitude;
                                    association.Longitude = address.Coordinates.Longitude;
                                    association.GeocodeStatus = GeocodeStatus.Success;

                                    _log.Debug("Similar association was not found, queried geocoder successfully. placeKey:{0}, geocoded Location:{1}, {2}", association.PlaceKey, association.Locality, association.Country);

                                    // updated association, continue on
                                    documentSession.SaveChanges();
                                    continue;
                                }

                                _log.Debug("Similar association not found, queried geocoder un-successfully. placeKey:{0}", association.PlaceKey);
                            }
                            catch (Exception e)
                            {
                                if (e is GoogleGeocodingException)
                                {
                                    _log.Debug("Error geocoding associations status:{0}", ((GoogleGeocodingException)e).Status);

                                    // return from import as we have encountered a problem with the geocoder
                                    return;
                                }

                                // Unexpected error so rethrow.
                                throw;
                            }
                        }
                        // similar item found
                        else if(similarItem != null)
                        {
                            // find relevant association
                            var similarAssociation = similarItem
                                .Associations
                                .First(y => y.GeocodeStatus != GeocodeStatus.UnAttempted &&
                                    y.PlaceKey == association.PlaceKey);

                            if (similarAssociation.GeocodeStatus == GeocodeStatus.Success)
                            {
                                association.Locality = similarAssociation.Locality;
                                association.Country = similarAssociation.Country;
                                association.Latitude = similarAssociation.Latitude;
                                association.Longitude = similarAssociation.Longitude;
                                association.GeocodeStatus = similarAssociation.GeocodeStatus;

                                _log.Debug("Similar association found that queried geocoder successfully. placeKey:{0}, geocoded Location:{1}, {2}", association.PlaceKey, association.Locality, association.Country);

                                // updated association, continue on
                                documentSession.SaveChanges();
                                continue;
                            }

                            _log.Debug("Similar association found that queried geocoder un-successfully. placeKey:{0}", association.PlaceKey);
                        }

                        association.GeocodeStatus = GeocodeStatus.Failure;
                        documentSession.SaveChanges();
                        associationCount++;
                    }                    
                }
            }

            _log.Debug("Geocoding of {0} associations complete, queried geocoder {1} times", associationCount, geocodeCount);
        }

        private bool ImportCanceled()
        {
            return Program.ImportCanceled;
        }
    }
}