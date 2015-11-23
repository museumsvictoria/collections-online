using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Factories;
using CollectionsOnline.WebSite.Models;
using CollectionsOnline.WebSite.Models.Api;
using Raven.Client;
using StackExchange.Profiling;
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
            using (MiniProfiler.Current.Step("Build Search Index view model"))
            using (_documentSession.Advanced.DocumentStore.AggressivelyCacheFor(Constants.AggressiveCacheTimeSpan))
            {
                RavenQueryStatistics statistics;
                IList<EmuAggregateRootViewModel> results;

                var query = QueryBuilder<EmuAggregateRootViewModel>.BuildIndexQuery(_documentSession, searchInputModel.Page, searchInputModel.PerPage, searchInputModel.Sort, searchInputModel.Queries, searchInputModel.Facets, searchInputModel.MultiFacets, searchInputModel.Terms);
                var facetQuery = QueryBuilder<EmuAggregateRootViewModel>.BuildIndexQuery(_documentSession, searchInputModel.Page, searchInputModel.PerPage, searchInputModel.Sort, searchInputModel.Queries, searchInputModel.Facets, searchInputModel.MultiFacets, searchInputModel.Terms);

                try
                {
                    results = query
                        .SelectFields<EmuAggregateRootViewModel>()
                        .Statistics(out statistics)
                        .ToList();
                }
                catch (Exception exception)
                {
                    if (IsQueryFailedException(exception))
                    {
                        // Re-build and escape search query
                        query = QueryBuilder<EmuAggregateRootViewModel>.BuildIndexQuery(_documentSession, searchInputModel.Page, searchInputModel.PerPage, searchInputModel.Sort, searchInputModel.Queries, searchInputModel.Facets, searchInputModel.MultiFacets, searchInputModel.Terms, escapeQueryOptions:EscapeQueryOptions.EscapeAll);
                        facetQuery = QueryBuilder<EmuAggregateRootViewModel>.BuildIndexQuery(_documentSession, searchInputModel.Page, searchInputModel.PerPage, searchInputModel.Sort, searchInputModel.Queries, searchInputModel.Facets, searchInputModel.MultiFacets, searchInputModel.Terms, escapeQueryOptions: EscapeQueryOptions.EscapeAll);

                        results = query
                            .SelectFields<EmuAggregateRootViewModel>()
                            .Statistics(out statistics)
                            .ToList();
                    }
                    else
                        throw;
                }
                
                var facets = facetQuery.ToFacets("facets/combinedFacets");

                return _searchViewModelFactory.MakeSearchIndex(
                    results,
                    facets,
                    statistics.TotalResults,
                    searchInputModel);
            }
        }

        public ApiViewModel BuildSearchApi(SearchApiInputModel searchApiInputModel, ApiInputModel apiInputModel)
        {
            using (MiniProfiler.Current.Step("Build Search Api view model"))
            using (_documentSession.Advanced.DocumentStore.AggressivelyCacheFor(Constants.AggressiveCacheTimeSpan))
            {
                RavenQueryStatistics statistics;
                IList<dynamic> queryResults;
                var results = new List<dynamic>();
                var query = QueryBuilder<dynamic>.BuildIndexQuery(_documentSession, apiInputModel.Page, apiInputModel.PerPage, searchApiInputModel.Sort, searchApiInputModel.Queries, searchApiInputModel.Facets, searchApiInputModel.MultiFacets, searchApiInputModel.Terms, searchApiInputModel.MinDateModified, searchApiInputModel.MaxDateModified);

                try
                {
                    queryResults = query
                        .Statistics(out statistics)
                        .ToList();
                }
                catch (Exception exception)
                {
                    if (IsQueryFailedException(exception))
                    {
                        // Re-build and escape search query
                        query = QueryBuilder<dynamic>.BuildIndexQuery(_documentSession, apiInputModel.Page, apiInputModel.PerPage, searchApiInputModel.Sort, searchApiInputModel.Queries, searchApiInputModel.Facets, searchApiInputModel.MultiFacets, searchApiInputModel.Terms, searchApiInputModel.MinDateModified, searchApiInputModel.MaxDateModified, EscapeQueryOptions.EscapeAll);

                        queryResults = query
                            .Statistics(out statistics)
                            .ToList();
                    }
                    else
                        throw;
                }

                foreach (var result in queryResults)
                {
                    if(result is Article)
                        results.Add(Mapper.Map<Article, ArticleApiViewModel>(result));

                    if (result is Item)
                        results.Add(Mapper.Map<Item, ItemApiViewModel>(result));

                    if (result is Species)
                        results.Add(Mapper.Map<Species, SpeciesApiViewModel>(result));

                    if (result is Specimen)
                        results.Add(Mapper.Map<Specimen, SpecimenApiViewModel>(result));
                }

                return new ApiViewModel
                {
                    Results = results,
                    ApiPageInfo = new ApiPageInfo(statistics.TotalResults, apiInputModel.PerPage)
                };
            }
        }

        private bool IsQueryFailedException(Exception exception)
        {
            return (exception is InvalidOperationException && exception.Message.Contains("Query failed"));
        }
    }
}