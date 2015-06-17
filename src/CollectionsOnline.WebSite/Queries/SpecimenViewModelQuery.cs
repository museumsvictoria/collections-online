using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Transformers;
using Nancy.Helpers;
using Newtonsoft.Json;
using Raven.Client;

namespace CollectionsOnline.WebSite.Queries
{
    public class SpecimenViewModelQuery : ISpecimenViewModelQuery
    {
        private readonly IDocumentSession _documentSession;

        public SpecimenViewModelQuery(
            IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public SpecimenViewTransformerResult BuildSpecimen(string specimenId)
        {
            var result = _documentSession.Load<SpecimenViewTransformer, SpecimenViewTransformerResult>(specimenId);

            if (result.Specimen.Taxonomy != null)
            {
                var query = _documentSession.Advanced
                    .DocumentQuery<CombinedIndexResult, CombinedIndex>()
                    .WhereEquals("Taxon", result.Specimen.Taxonomy.TaxonName)
                    .Take(1);

                // Dont allow a link to search page if the current specimen is the only result
                if (query.SelectFields<CombinedIndexResult>("Id").Select(x => x.Id).Except(new[] { specimenId }).Any())
                {
                    result.RelatedSpeciesSpecimenItemCount = query.QueryResult.TotalResults;
                }
            }

            // Set Media            
            result.SpecimenImages = result.Specimen.Media.Where(x => x is ImageMedia).Cast<ImageMedia>().ToList();
            result.SpecimenFiles = result.Specimen.Media.Where(x => x is FileMedia).Cast<FileMedia>().ToList();
            result.JsonSpecimenImages = JsonConvert.SerializeObject(result.SpecimenImages);

            // TODO: move to view factory and create dedicated view model
            // Set Geospatial
            if (!string.IsNullOrWhiteSpace(result.Specimen.SiteCode))
                result.GeoSpatial.Add(new KeyValuePair<string, string>("Site Code", result.Specimen.SiteCode));
            if (!string.IsNullOrWhiteSpace(result.Specimen.Ocean))
                result.GeoSpatial.Add(new KeyValuePair<string, string>("Ocean", string.Format(@"<a href=""/search?locality={0}"">{1}</a>", HttpUtility.UrlEncode(result.Specimen.Ocean), result.Specimen.Ocean)));
            if (!string.IsNullOrWhiteSpace(result.Specimen.Continent))
                result.GeoSpatial.Add(new KeyValuePair<string, string>("Continent", string.Format(@"<a href=""/search?locality={0}"">{1}</a>", HttpUtility.UrlEncode(result.Specimen.Continent), result.Specimen.Continent)));
            if (!string.IsNullOrWhiteSpace(result.Specimen.Country))
                result.GeoSpatial.Add(new KeyValuePair<string, string>("Country", string.Format(@"<a href=""/search?locality={0}"">{1}</a>", HttpUtility.UrlEncode(result.Specimen.Country), result.Specimen.Country)));
            if (!string.IsNullOrWhiteSpace(result.Specimen.State))
                result.GeoSpatial.Add(new KeyValuePair<string, string>("State", string.Format(@"<a href=""/search?locality={0}"">{1}</a>", HttpUtility.UrlEncode(result.Specimen.State), result.Specimen.State)));
            if (!string.IsNullOrWhiteSpace(result.Specimen.District))
                result.GeoSpatial.Add(new KeyValuePair<string, string>("District", string.Format(@"<a href=""/search?locality={0}"">{1}</a>", HttpUtility.UrlEncode(result.Specimen.District), result.Specimen.District)));
            if (!string.IsNullOrWhiteSpace(result.Specimen.Town))
                result.GeoSpatial.Add(new KeyValuePair<string, string>("Town", string.Format(@"<a href=""/search?locality={0}"">{1}</a>", HttpUtility.UrlEncode(result.Specimen.Town), result.Specimen.Town)));
            if (!string.IsNullOrWhiteSpace(result.Specimen.NearestNamedPlace))
                result.GeoSpatial.Add(new KeyValuePair<string, string>("Nearest Named Place", string.Format(@"<a href=""/search?locality={0}"">{1}</a>", HttpUtility.UrlEncode(result.Specimen.NearestNamedPlace), result.Specimen.NearestNamedPlace)));
            if (!string.IsNullOrWhiteSpace(result.Specimen.PreciseLocation))
                result.GeoSpatial.Add(new KeyValuePair<string, string>("Precise Location", result.Specimen.PreciseLocation));
            if (!string.IsNullOrWhiteSpace(result.Specimen.MinimumElevation))
                result.GeoSpatial.Add(new KeyValuePair<string, string>("Minimum Elevation", result.Specimen.MinimumElevation));
            if (!string.IsNullOrWhiteSpace(result.Specimen.MaximumElevation))
                result.GeoSpatial.Add(new KeyValuePair<string, string>("Maximum Elevation", result.Specimen.MaximumElevation));
            if(result.Specimen.Latitudes.Any())
                result.GeoSpatial.Add(new KeyValuePair<string, string>("Latitude", result.Specimen.Latitudes.Concatenate(";")));
            if (result.Specimen.Longitudes.Any())
                result.GeoSpatial.Add(new KeyValuePair<string, string>("Longitude", result.Specimen.Longitudes.Concatenate(";")));
            if (!string.IsNullOrWhiteSpace(result.Specimen.GeodeticDatum))
                result.GeoSpatial.Add(new KeyValuePair<string, string>("Geodetic Datum", result.Specimen.GeodeticDatum));
            if (!string.IsNullOrWhiteSpace(result.Specimen.SiteRadius))
                result.GeoSpatial.Add(new KeyValuePair<string, string>("Site Radius", result.Specimen.SiteRadius));
            if (!string.IsNullOrWhiteSpace(result.Specimen.GeoreferenceSource))
                result.GeoSpatial.Add(new KeyValuePair<string, string>("Georeference Source", result.Specimen.GeoreferenceSource));
            if (!string.IsNullOrWhiteSpace(result.Specimen.GeoreferenceProtocol))
                result.GeoSpatial.Add(new KeyValuePair<string, string>("Georeference Protocol", result.Specimen.GeoreferenceProtocol));
            if (!string.IsNullOrWhiteSpace(result.Specimen.GeoreferenceDate))
                result.GeoSpatial.Add(new KeyValuePair<string, string>("Georeference Date", result.Specimen.GeoreferenceDate));
            if (!string.IsNullOrWhiteSpace(result.Specimen.GeoreferenceBy))
                result.GeoSpatial.Add(new KeyValuePair<string, string>("Georeference By", result.Specimen.GeoreferenceBy));
            if (!string.IsNullOrWhiteSpace(result.Specimen.GeologyEra))
                result.GeoSpatial.Add(new KeyValuePair<string, string>("Era", result.Specimen.GeologyEra));
            if (!string.IsNullOrWhiteSpace(result.Specimen.GeologyPeriod))
                result.GeoSpatial.Add(new KeyValuePair<string, string>("Period", result.Specimen.GeologyPeriod));
            if (!string.IsNullOrWhiteSpace(result.Specimen.GeologyEpoch))
                result.GeoSpatial.Add(new KeyValuePair<string, string>("Epoch", result.Specimen.GeologyEpoch));
            if (!string.IsNullOrWhiteSpace(result.Specimen.GeologyStage))
                result.GeoSpatial.Add(new KeyValuePair<string, string>("Stage", result.Specimen.GeologyStage));
            if (!string.IsNullOrWhiteSpace(result.Specimen.GeologyGroup))
                result.GeoSpatial.Add(new KeyValuePair<string, string>("Geological group", result.Specimen.GeologyGroup));
            if (!string.IsNullOrWhiteSpace(result.Specimen.GeologyFormation))
                result.GeoSpatial.Add(new KeyValuePair<string, string>("Geological formation", result.Specimen.GeologyFormation));
            if (!string.IsNullOrWhiteSpace(result.Specimen.GeologyMember))
                result.GeoSpatial.Add(new KeyValuePair<string, string>("Geological member", result.Specimen.GeologyMember));
            if (!string.IsNullOrWhiteSpace(result.Specimen.GeologyRockType))
                result.GeoSpatial.Add(new KeyValuePair<string, string>("Rock Type", result.Specimen.GeologyRockType));

            // Map
            var latlongs = new List<double[]>();
            if (result.Specimen.Latitudes.Any(x => !string.IsNullOrWhiteSpace(x)) && result.Specimen.Longitudes.Any(x => !string.IsNullOrWhiteSpace(x)))
            {
                latlongs = result.Specimen.Latitudes.Zip(result.Specimen.Longitudes, (lat, lon) => new[] { double.Parse(lat), double.Parse(lon) }).ToList();
            }
            result.JsonSpecimenLatLongs = JsonConvert.SerializeObject(latlongs); 

            return result;
        }
    }
}