using System;
using System.Linq;
using CollectionsOnline.Core.Extensions;
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
            var geocodeQueryCount = 0;

            var assNotFoundGeocodeSuccess = 0;
            var assNotFoundGeocodeFailure = 0;
            var assFoundGeocodeSuccess = 0;
            var assFoundGeocodeFailure = 0;

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

                    _log.Debug("GeocodingItemPlaceAssociations. item-id:{0}, associations-count:{1}", item.Id, item.Associations.Count);

                    foreach (var association in item.Associations.Where(x => x.GeocodeStatus == GeocodeStatus.UnAttempted))
                    {
                        associationCount++;

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

                                geocodeQueryCount++;

                                // Only try and find street level geocoded locations
                                var address = addresses.FirstOrDefault(x =>
                                    ((GoogleAddress)x).Type == GoogleAddressType.StreetAddress ||
                                    ((GoogleAddress)x).Type == GoogleAddressType.Premise ||
                                    ((GoogleAddress)x).Type == GoogleAddressType.Subpremise ||
                                    ((GoogleAddress)x).Type == GoogleAddressType.Route) as GoogleAddress;

                                if (address != null)
                                {
                                    association.GeocodeStatus = GeocodeStatus.Success;
                                    association.GeocodePlace = address.FormattedAddress;

                                    _log.Debug("AssociationNotFound-GeocoderSuccess. association-place:{0}, geocoded-address:{1}", association.Place, address.FormattedAddress);
                                    assNotFoundGeocodeSuccess++;

                                    // updated association, continue on
                                    documentSession.SaveChanges();
                                    continue;
                                }
                                
                                _log.Debug("AssociationNotFound-GeocoderFailure. association-place:{0}, geocoded-address:{1}", association.Place, (addresses != null && addresses.FirstOrDefault() != null) ? addresses.FirstOrDefault().FormattedAddress : string.Empty);
                                assNotFoundGeocodeFailure++;
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
                                association.GeocodePlace = similarAssociation.GeocodePlace;
                                association.GeocodeStatus = similarAssociation.GeocodeStatus;

                                _log.Debug("AssociationFound-GeocoderSuccess. association-place:{0}, geocoded-address:{1}", association.Place, association.GeocodePlace);
                                assFoundGeocodeSuccess++;

                                // updated association, continue on
                                documentSession.SaveChanges();
                                continue;
                            }

                            _log.Debug("AssociationFound-GeocoderFailure. association-place:{0}", association.Place);
                            assFoundGeocodeFailure++;
                        }

                        association.GeocodeStatus = GeocodeStatus.Failure;
                        documentSession.SaveChanges();                        
                    }                    
                }
            }

            _log.Debug("Geocoding of {0} associations complete, queried geocoder {1} times, associations-not-found-geocode-success:{2}, associations-not-found-geocode-failure:{3}, associations-found-geocode-success:{4}, associations-found-geocode-failure:{5}", 
                associationCount, 
                geocodeQueryCount, 
                assNotFoundGeocodeSuccess, 
                assNotFoundGeocodeFailure, 
                assFoundGeocodeSuccess, 
                assFoundGeocodeFailure);
        }

        public int Order
        {
            get { return 20; }
        }

        private bool ImportCanceled()
        {
            return Program.ImportCanceled;
        }
    }
}