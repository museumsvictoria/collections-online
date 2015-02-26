using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.WebSite.Factories;
using CollectionsOnline.WebSite.Models;
using Raven.Abstractions.Data;
using Raven.Client;
using Constants = CollectionsOnline.Core.Config.Constants;

namespace CollectionsOnline.WebSite.Queries
{
    public class SearchViewModelQuery : ISearchViewModelQuery
    {
        private readonly IDocumentSession _documentSession;
        private readonly ISearchViewModelFactory _searchViewModelFactory;

        public SearchViewModelQuery(
            IDocumentSession documentSession,
            ISearchViewModelFactory searchViewModelFactory)
        {
            _documentSession = documentSession;
            _searchViewModelFactory = searchViewModelFactory;
        }

        public SearchViewModel BuildSearch(SearchInputModel searchInputModel)
        {
            // Aggressively cache due to slow facet performance
            using (_documentSession.Advanced.DocumentStore.AggressivelyCacheFor(Constants.AggressiveCacheTimeSpan))
            {
                // normalize inputs
                if (searchInputModel.Offset < 0)
                    searchInputModel.Offset = 0;
                if (searchInputModel.Limit <= 0 || searchInputModel.Limit > Constants.PagingPageSizeMax)
                    searchInputModel.Limit = Constants.PagingPageSizeDefault;
                searchInputModel.Query = searchInputModel.Query ?? string.Empty;

                var queryStopwatch = new Stopwatch();
                var facetStopwatch = new Stopwatch();

                // perform query
                queryStopwatch.Start();
                var query = _documentSession.Advanced
                    .DocumentQuery<CombinedResult, Combined>()
                    .Skip(searchInputModel.Offset)
                    .Take(searchInputModel.Limit);

                // get facets
                facetStopwatch.Start();
                var facetQuery = _documentSession.Advanced
                    .DocumentQuery<CombinedResult, Combined>();

                // search query
                if (!string.IsNullOrWhiteSpace(searchInputModel.Query))
                {
                    query = query.Search("Content", searchInputModel.Query);
                    facetQuery = facetQuery.Search("Content", searchInputModel.Query);
                }

                // facet queries
                if (!string.IsNullOrWhiteSpace(searchInputModel.Type))
                {
                    query = query.AndAlso().WhereEquals("Type", searchInputModel.Type);
                    facetQuery = facetQuery.AndAlso().WhereEquals("Type", searchInputModel.Type);
                }
                if (!string.IsNullOrWhiteSpace(searchInputModel.Category))
                {
                    query = query.AndAlso().WhereEquals("Category", searchInputModel.Category);
                    facetQuery = facetQuery.AndAlso().WhereEquals("Category", searchInputModel.Category);
                }
                if (!string.IsNullOrWhiteSpace(searchInputModel.HasImages))
                {
                    query = query.AndAlso().WhereEquals("HasImages", searchInputModel.HasImages);
                    facetQuery = facetQuery.AndAlso().WhereEquals("HasImages", searchInputModel.HasImages);
                }
                if (!string.IsNullOrWhiteSpace(searchInputModel.OnDisplay))
                {
                    query = query.AndAlso().WhereEquals("OnDisplay", searchInputModel.OnDisplay);
                    facetQuery = facetQuery.AndAlso().WhereEquals("OnDisplay", searchInputModel.OnDisplay);
                }
                if (searchInputModel.CollectionAreas != null && searchInputModel.CollectionAreas.Any())
                {
                    foreach (var collectionArea in searchInputModel.CollectionAreas)
                    {
                        query = query.AndAlso().WhereEquals("CollectionAreas", collectionArea);
                        facetQuery = facetQuery.AndAlso().WhereEquals("CollectionAreas", collectionArea);
                    }
                }
                if (!string.IsNullOrWhiteSpace(searchInputModel.ItemType))
                {
                    query = query.AndAlso().WhereEquals("ItemType", searchInputModel.ItemType);
                    facetQuery = facetQuery.AndAlso().WhereEquals("ItemType", searchInputModel.ItemType);
                }
                if (!string.IsNullOrWhiteSpace(searchInputModel.SpeciesType))
                {
                    query = query.AndAlso().WhereEquals("SpeciesType", searchInputModel.SpeciesType);
                    facetQuery = facetQuery.AndAlso().WhereEquals("SpeciesType", searchInputModel.SpeciesType);
                }
                if (!string.IsNullOrWhiteSpace(searchInputModel.SpeciesEndemicity))
                {
                    query = query.AndAlso().WhereEquals("SpeciesEndemicity", searchInputModel.SpeciesEndemicity);
                    facetQuery = facetQuery.AndAlso().WhereEquals("SpeciesEndemicity", searchInputModel.SpeciesEndemicity);
                }
                if (!string.IsNullOrWhiteSpace(searchInputModel.SpecimenScientificGroup))
                {
                    query = query.AndAlso().WhereEquals("SpecimenScientificGroup", searchInputModel.SpecimenScientificGroup);
                    facetQuery = facetQuery.AndAlso().WhereEquals("SpecimenScientificGroup", searchInputModel.SpecimenScientificGroup);
                }
                if (searchInputModel.ArticleTypes != null && searchInputModel.ArticleTypes.Any())
                {
                    foreach (var articleType in searchInputModel.ArticleTypes)
                    {
                        query = query.AndAlso().WhereEquals("ArticleTypes", articleType);
                        facetQuery = facetQuery.AndAlso().WhereEquals("ArticleTypes", articleType);
                    }
                }
                if (!string.IsNullOrWhiteSpace(searchInputModel.OnDisplayLocation))
                {
                    query = query.AndAlso().WhereEquals("OnDisplayLocation", searchInputModel.OnDisplayLocation);
                    facetQuery = facetQuery.AndAlso().WhereEquals("OnDisplayLocation", searchInputModel.OnDisplayLocation);
                }

                // Term queries
                if (!string.IsNullOrWhiteSpace(searchInputModel.Keyword))
                {
                    query = query.AndAlso().WhereEquals("Keywords", searchInputModel.Keyword);
                    facetQuery = facetQuery.AndAlso().WhereEquals("Keywords", searchInputModel.Keyword);
                }
                if (!string.IsNullOrWhiteSpace(searchInputModel.Locality))
                {
                    query = query.AndAlso().WhereEquals("Localities", searchInputModel.Locality);
                    facetQuery = facetQuery.AndAlso().WhereEquals("Localities", searchInputModel.Locality);
                }
                if (!string.IsNullOrWhiteSpace(searchInputModel.Collection))
                {
                    query = query.AndAlso().WhereEquals("Collections", searchInputModel.Collection);
                    facetQuery = facetQuery.AndAlso().WhereEquals("Collections", searchInputModel.Collection);
                }
                if (!string.IsNullOrWhiteSpace(searchInputModel.CulturalGroup))
                {
                    query = query.AndAlso().WhereEquals("CulturalGroups", searchInputModel.CulturalGroup);
                    facetQuery = facetQuery.AndAlso().WhereEquals("CulturalGroups", searchInputModel.CulturalGroup);
                }
                if (!string.IsNullOrWhiteSpace(searchInputModel.Classification))
                {
                    query = query.AndAlso().WhereEquals("Classifications", searchInputModel.Classification);
                    facetQuery = facetQuery.AndAlso().WhereEquals("Classifications", searchInputModel.Classification);
                }
                if (!string.IsNullOrWhiteSpace(searchInputModel.Name))
                {
                    query = query.AndAlso().WhereEquals("Names", searchInputModel.Name);
                    facetQuery = facetQuery.AndAlso().WhereEquals("Names", searchInputModel.Name);
                }
                if (!string.IsNullOrWhiteSpace(searchInputModel.Technique))
                {
                    query = query.AndAlso().WhereEquals("Technique", searchInputModel.Technique);
                    facetQuery = facetQuery.AndAlso().WhereEquals("Technique", searchInputModel.Technique);
                }
                if (!string.IsNullOrWhiteSpace(searchInputModel.Denomination))
                {
                    query = query.AndAlso().WhereEquals("Denominations", searchInputModel.Denomination);
                    facetQuery = facetQuery.AndAlso().WhereEquals("Denominations", searchInputModel.Denomination);
                }
                if (!string.IsNullOrWhiteSpace(searchInputModel.Habitat))
                {
                    query = query.AndAlso().WhereEquals("Habitats", searchInputModel.Habitat);
                    facetQuery = facetQuery.AndAlso().WhereEquals("Habitats", searchInputModel.Habitat);
                }
                if (!string.IsNullOrWhiteSpace(searchInputModel.Taxon))
                {
                    query = query.AndAlso().WhereEquals("Taxon", searchInputModel.Taxon);
                    facetQuery = facetQuery.AndAlso().WhereEquals("Taxon", searchInputModel.Taxon);
                }
                if (!string.IsNullOrWhiteSpace(searchInputModel.TypeStatus))
                {
                    query = query.AndAlso().WhereEquals("TypeStatus", searchInputModel.TypeStatus);
                    facetQuery = facetQuery.AndAlso().WhereEquals("TypeStatus", searchInputModel.TypeStatus);
                }
                if (!string.IsNullOrWhiteSpace(searchInputModel.GeoType))
                {
                    query = query.AndAlso().WhereEquals("GeoTypes", searchInputModel.GeoType);
                    facetQuery = facetQuery.AndAlso().WhereEquals("GeoTypes", searchInputModel.GeoType);
                }
                if (!string.IsNullOrWhiteSpace(searchInputModel.MuseumLocation))
                {
                    query = query.AndAlso().WhereEquals("MuseumLocations", searchInputModel.MuseumLocation);
                    facetQuery = facetQuery.AndAlso().WhereEquals("MuseumLocations", searchInputModel.MuseumLocation);
                }
                if (!string.IsNullOrWhiteSpace(searchInputModel.Article))
                {
                    query = query.AndAlso().WhereEquals("Articles", searchInputModel.Article);
                    facetQuery = facetQuery.AndAlso().WhereEquals("Articles", searchInputModel.Article);
                }

                RavenQueryStatistics statistics;
                var results = query
                    .SelectFields<CombinedResult>(
                        "Id",
                        "DisplayTitle",
                        "Summary",
                        "ThumbnailUri",
                        "Type")
                    .OrderByDescending(x => x.Quality)
                    .Statistics(out statistics)
                    .ToList();
                queryStopwatch.Stop();

                var facets = facetQuery.ToFacets("facets/combinedFacets");
                facetStopwatch.Stop();

                // Get suggestions if needed
                var suggestions = new List<string>();
                if (!string.IsNullOrWhiteSpace(searchInputModel.Query) &&
                    results.Count <= Constants.SuggestionsMinResultsSize)
                {
                    suggestions = _documentSession
                        .Query<CombinedResult, Combined>()
                        .Suggest(new SuggestionQuery()
                        {
                            Field = "Content",
                            Term = searchInputModel.Query,
                            Accuracy = 0.4f,
                            MaxSuggestions = 5,
                            Distance = StringDistanceTypes.JaroWinkler,
                            Popularity = true,
                        }).Suggestions.ToList();
                }                

                return _searchViewModelFactory.MakeViewModel(
                    results,
                    facets,
                    suggestions,
                    statistics.TotalResults,
                    searchInputModel,
                    queryStopwatch.ElapsedMilliseconds,
                    facetStopwatch.ElapsedMilliseconds);
            }
        }
    }
}