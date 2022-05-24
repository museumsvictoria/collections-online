using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models.Api;
using CollectionsOnline.WebSite.Transformers;
using Nancy.Helpers;
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
            using (_documentSession.Advanced.DocumentStore.AggressivelyCacheFor(Constants.AggressiveCacheTimeSpan))
            {
                var result = _documentSession.Load<SpecimenViewTransformer, SpecimenViewTransformerResult>(specimenId);

                if (result.Specimen.Taxonomy != null)
                {
                    var query = _documentSession.Advanced
                        .DocumentQuery<CombinedIndexResult, CombinedIndex>()
                        .WhereEquals("Taxon", result.Specimen.Taxonomy.TaxonName)
                        .Take(1);

                    // Dont allow a link to search page if the current specimen is the only result
                    if (query.SelectFields<CombinedIndexResult>("Id").Select(x => x.Id).Except(new[] {specimenId}).Any())
                    {
                        result.RelatedSpeciesSpecimenItemCount = query.QueryResult.TotalResults;
                    }
                }

                // TODO: move to view factory and create dedicated view model
                // Set Geospatial
                if (result.Specimen.CollectionSite != null)
                {
                    if (!string.IsNullOrWhiteSpace(result.Specimen.CollectionSite.SiteCode))
                        result.GeoSpatial.Add(new KeyValuePair<string, string>("Site Code", result.Specimen.CollectionSite.SiteCode));
                    if (!string.IsNullOrWhiteSpace(result.Specimen.CollectionSite.Ocean))
                        result.GeoSpatial.Add(new KeyValuePair<string, string>("Ocean", $@"<a href=""/search?locality={HttpUtility.UrlEncode(result.Specimen.CollectionSite.Ocean)}"">{result.Specimen.CollectionSite.Ocean}</a>"));
                    if (!string.IsNullOrWhiteSpace(result.Specimen.CollectionSite.Continent))
                        result.GeoSpatial.Add(new KeyValuePair<string, string>("Continent", $@"<a href=""/search?locality={HttpUtility.UrlEncode(result.Specimen.CollectionSite.Continent)}"">{result.Specimen.CollectionSite.Continent}</a>"));
                    if (!string.IsNullOrWhiteSpace(result.Specimen.CollectionSite.Country))
                        result.GeoSpatial.Add(new KeyValuePair<string, string>("Country", $@"<a href=""/search?locality={HttpUtility.UrlEncode(result.Specimen.CollectionSite.Country)}"">{result.Specimen.CollectionSite.Country}</a>"));
                    if (!string.IsNullOrWhiteSpace(result.Specimen.CollectionSite.State))
                        result.GeoSpatial.Add(new KeyValuePair<string, string>("State", $@"<a href=""/search?locality={HttpUtility.UrlEncode(result.Specimen.CollectionSite.State)}"">{result.Specimen.CollectionSite.State}</a>"));
                    if (!string.IsNullOrWhiteSpace(result.Specimen.CollectionSite.District))
                        result.GeoSpatial.Add(new KeyValuePair<string, string>("District", $@"<a href=""/search?locality={HttpUtility.UrlEncode(result.Specimen.CollectionSite.District)}"">{result.Specimen.CollectionSite.District}</a>"));
                    if (!string.IsNullOrWhiteSpace(result.Specimen.CollectionSite.Town))
                        result.GeoSpatial.Add(new KeyValuePair<string, string>("Town", $@"<a href=""/search?locality={HttpUtility.UrlEncode(result.Specimen.CollectionSite.Town)}"">{result.Specimen.CollectionSite.Town}</a>"));
                    if (!string.IsNullOrWhiteSpace(result.Specimen.CollectionSite.NearestNamedPlace))
                        result.GeoSpatial.Add(new KeyValuePair<string, string>("Nearest Named Place", $@"<a href=""/search?locality={HttpUtility.UrlEncode(result.Specimen.CollectionSite.NearestNamedPlace)}"">{result.Specimen.CollectionSite.NearestNamedPlace}</a>"));
                    if (!string.IsNullOrWhiteSpace(result.Specimen.CollectionSite.PreciseLocation))
                        result.GeoSpatial.Add(new KeyValuePair<string, string>("Precise Location", result.Specimen.CollectionSite.PreciseLocation));
                    if (!string.IsNullOrWhiteSpace(result.Specimen.CollectionSite.MinimumElevation))
                        result.GeoSpatial.Add(new KeyValuePair<string, string>("Minimum Elevation", result.Specimen.CollectionSite.MinimumElevation));
                    if (!string.IsNullOrWhiteSpace(result.Specimen.CollectionSite.MaximumElevation))
                        result.GeoSpatial.Add(new KeyValuePair<string, string>("Maximum Elevation", result.Specimen.CollectionSite.MaximumElevation));
                    if (result.Specimen.CollectionSite.Latitude != null)
                        result.GeoSpatial.Add(new KeyValuePair<string, string>("Latitude", result.Specimen.CollectionSite.Latitude.ToString()));
                    if (result.Specimen.CollectionSite.Longitude != null)
                        result.GeoSpatial.Add(new KeyValuePair<string, string>("Longitude", result.Specimen.CollectionSite.Longitude.ToString()));
                    if (!string.IsNullOrWhiteSpace(result.Specimen.CollectionSite.GeodeticDatum))
                        result.GeoSpatial.Add(new KeyValuePair<string, string>("Geodetic Datum", result.Specimen.CollectionSite.GeodeticDatum));
                    if (!string.IsNullOrWhiteSpace(result.Specimen.CollectionSite.SiteRadius))
                        result.GeoSpatial.Add(new KeyValuePair<string, string>("Site Radius", result.Specimen.CollectionSite.SiteRadius));
                    if (!string.IsNullOrWhiteSpace(result.Specimen.CollectionSite.GeoreferenceSource))
                        result.GeoSpatial.Add(new KeyValuePair<string, string>("Georeference Source", result.Specimen.CollectionSite.GeoreferenceSource));
                    if (!string.IsNullOrWhiteSpace(result.Specimen.CollectionSite.GeoreferenceProtocol))
                        result.GeoSpatial.Add(new KeyValuePair<string, string>("Georeference Protocol", result.Specimen.CollectionSite.GeoreferenceProtocol));
                    if (!string.IsNullOrWhiteSpace(result.Specimen.CollectionSite.GeoreferenceDate))
                        result.GeoSpatial.Add(new KeyValuePair<string, string>("Georeference Date", result.Specimen.CollectionSite.GeoreferenceDate));
                    if (!string.IsNullOrWhiteSpace(result.Specimen.CollectionSite.GeoreferenceBy))
                        result.GeoSpatial.Add(new KeyValuePair<string, string>("Georeference By", result.Specimen.CollectionSite.GeoreferenceBy));
                    if (!string.IsNullOrWhiteSpace(result.Specimen.CollectionSite.GeologyEra))
                        result.GeoSpatial.Add(new KeyValuePair<string, string>("Era", result.Specimen.CollectionSite.GeologyEra));
                    if (!string.IsNullOrWhiteSpace(result.Specimen.CollectionSite.GeologyPeriod))
                        result.GeoSpatial.Add(new KeyValuePair<string, string>("Period", result.Specimen.CollectionSite.GeologyPeriod));
                    if (!string.IsNullOrWhiteSpace(result.Specimen.CollectionSite.GeologyEpoch))
                        result.GeoSpatial.Add(new KeyValuePair<string, string>("Epoch", result.Specimen.CollectionSite.GeologyEpoch));
                    if (!string.IsNullOrWhiteSpace(result.Specimen.CollectionSite.GeologyStage))
                        result.GeoSpatial.Add(new KeyValuePair<string, string>("Stage", result.Specimen.CollectionSite.GeologyStage));
                    if (!string.IsNullOrWhiteSpace(result.Specimen.CollectionSite.GeologyGroup))
                        result.GeoSpatial.Add(new KeyValuePair<string, string>("Geological group", result.Specimen.CollectionSite.GeologyGroup));
                    if (!string.IsNullOrWhiteSpace(result.Specimen.CollectionSite.GeologyFormation))
                        result.GeoSpatial.Add(new KeyValuePair<string, string>("Geological formation", result.Specimen.CollectionSite.GeologyFormation));
                    if (!string.IsNullOrWhiteSpace(result.Specimen.CollectionSite.GeologyMember))
                        result.GeoSpatial.Add(new KeyValuePair<string, string>("Geological member", result.Specimen.CollectionSite.GeologyMember));
                    if (!string.IsNullOrWhiteSpace(result.Specimen.CollectionSite.GeologyRockType))
                        result.GeoSpatial.Add(new KeyValuePair<string, string>("Rock Type", result.Specimen.CollectionSite.GeologyRockType));
                }

                // Add Uris for everything except Paleo and Geology
                if (result.Specimen.Taxonomy != null &&
                    !(string.Equals(result.Specimen.Discipline, "Palaeontology", StringComparison.OrdinalIgnoreCase) ||
                      string.Equals(result.Specimen.ScientificGroup, "Geology", StringComparison.OrdinalIgnoreCase)))
                {
                    result.Specimen.Media.Add(new UriMedia
                    {
                        Caption = "See more specimens of this species in OZCAM",
                        Uri = $"http://ozcam.ala.org.au/occurrences/search?taxa={result.Specimen.Taxonomy.TaxonName}"
                    });
                }

                return result;
            }
        }

        public ApiViewModel BuildSpecimenApiIndex(ApiInputModel apiInputModel)
        {
            using (_documentSession.Advanced.DocumentStore.AggressivelyCacheFor(Constants.AggressiveCacheTimeSpan))
            {
                RavenQueryStatistics statistics;
                var results = _documentSession.Advanced
                    .DocumentQuery<dynamic, CombinedIndex>()
                    .WhereEquals("RecordType", "Specimen")
                    .Statistics(out statistics)
                    .Skip((apiInputModel.Page - 1)*apiInputModel.PerPage)
                    .Take(apiInputModel.PerPage);

                return new ApiViewModel
                {
                    Results = results.Cast<Specimen>().Select<Specimen, dynamic>(Mapper.Map<Specimen, SpecimenApiViewModel>).ToList(),
                    ApiPageInfo = new ApiPageInfo(statistics.TotalResults, apiInputModel.PerPage)
                };
            }
        }
    }
}