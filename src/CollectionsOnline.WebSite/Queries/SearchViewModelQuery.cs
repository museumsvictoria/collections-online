using System.Diagnostics;
using System.Linq;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.WebSite.Factories;
using CollectionsOnline.WebSite.Models;
using CollectionsOnline.WebSite.Models.Api;
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

        public SearchIndexViewModel BuildSearchIndex(SearchInputModel searchInputModel)
        {
            using (_documentSession.Advanced.DocumentStore.AggressivelyCacheFor(Constants.AggressiveCacheTimeSpan))
            {
                var queryStopwatch = new Stopwatch();
                var facetStopwatch = new Stopwatch();

                // perform query
                queryStopwatch.Start();
                var query = _documentSession.Advanced
                    .DocumentQuery<CombinedIndexResult, CombinedIndex>()
                    .Skip((searchInputModel.Page - 1) * searchInputModel.PerPage)
                    .Take(searchInputModel.PerPage);

                // get facets
                facetStopwatch.Start();
                var facetQuery = _documentSession.Advanced
                    .DocumentQuery<CombinedIndexResult, CombinedIndex>();

                // search query (only add AndAlso() after first query)
                for (int i = 0; i < searchInputModel.Queries.Count; i++)
                {
                    if (i == 0)
                    {
                        query = query.Search(x => x.Content, searchInputModel.Queries[i]);
                        facetQuery = facetQuery.Search(x => x.Content, searchInputModel.Queries[i]);
                    }
                    else
                    {
                        query = query.AndAlso().Search(x => x.Content, searchInputModel.Queries[i]);
                        facetQuery = facetQuery.AndAlso().Search(x => x.Content, searchInputModel.Queries[i]);                        
                    }
                }

                // Add sorting
                switch (searchInputModel.Sort)
                {
                    case "quality":
                        query = query
                            .OrderByDescending(x => x.Quality);
                        break;
                    case "date":
                        query = query
                            .OrderByDescending(x => x.DateModified)
                            .OrderByDescending(x => x.Quality);
                        break;
                }

                if (searchInputModel.Queries.Any())
                {
                    query = query.OrderByScoreDescending();
                }

                // facet queries
                foreach (var facet in searchInputModel.Facets)
                {
                    query = query.AndAlso().WhereEquals(facet.Key, facet.Value);
                    facetQuery = facetQuery.AndAlso().WhereEquals(facet.Key, facet.Value);
                }

                // multiple facet queries
                foreach (var multiFacets in searchInputModel.MultiFacets)
                {
                    foreach (var facetValue in multiFacets.Value)
                    {
                        query = query.AndAlso().WhereEquals(multiFacets.Key, facetValue);
                        facetQuery = facetQuery.AndAlso().WhereEquals(multiFacets.Key, facetValue);
                    }
                }

                // term queries
                foreach (var term in searchInputModel.Terms)
                {
                    query = query.AndAlso().WhereEquals(term.Key, term.Value);
                    facetQuery = facetQuery.AndAlso().WhereEquals(term.Key, term.Value);
                }

                RavenQueryStatistics statistics;
                var results = query
                    .SelectFields<EmuAggregateRootViewModel>()
                    .Statistics(out statistics)
                    .ToList();
                queryStopwatch.Stop();

                var facets = facetQuery.ToFacets("facets/combinedFacets");
                facetStopwatch.Stop();

                return _searchViewModelFactory.MakeSearchIndex(
                    results,
                    facets,
                    statistics.TotalResults,
                    searchInputModel,
                    queryStopwatch.ElapsedMilliseconds,
                    facetStopwatch.ElapsedMilliseconds);
            }
        }

        public SearchApiViewModel BuildSearchApi(SearchInputModel searchInputModel)
        {
            using (_documentSession.Advanced.DocumentStore.AggressivelyCacheFor(Constants.AggressiveCacheTimeSpan))
            {
                // perform query
                var query = _documentSession.Advanced
                    .DocumentQuery<dynamic, CombinedIndex>()
                    .Skip((searchInputModel.Page - 1)*searchInputModel.PerPage)
                    .Take(searchInputModel.PerPage);

                // search query (only add AndAlso() after first query)
                for (int i = 0; i < searchInputModel.Queries.Count; i++)
                {
                    if (i == 0)
                    {
                        query = query.Search("Content", searchInputModel.Queries[i]);
                    }
                    else
                    {
                        query = query.AndAlso().Search("Content", searchInputModel.Queries[i]);
                    }
                }

                // Add sorting
                switch (searchInputModel.Sort)
                {
                    case "quality":
                        query = query
                            .OrderByDescending("Quality");
                        break;
                    case "date":
                        query = query
                            .OrderByDescending("DateModified")
                            .OrderByDescending("Quality");
                        break;
                }

                if (searchInputModel.Queries.Any())
                {
                    query = query.OrderByScoreDescending();
                }

                // facet queries
                foreach (var facet in searchInputModel.Facets)
                {
                    query = query.AndAlso().WhereEquals(facet.Key, facet.Value);
                }

                // multiple facet queries
                foreach (var multiFacets in searchInputModel.MultiFacets)
                {
                    foreach (var facetValue in multiFacets.Value)
                    {
                        query = query.AndAlso().WhereEquals(multiFacets.Key, facetValue);
                    }
                }

                // term queries
                foreach (var term in searchInputModel.Terms)
                {
                    query = query.AndAlso().WhereEquals(term.Key, term.Value);
                }

                RavenQueryStatistics statistics;
                var results = query
                    .Statistics(out statistics)
                    .ToList();

                return new SearchApiViewModel()
                {
                    Results = results,
                    Statistics = statistics
                };
            }
        }
    }
}