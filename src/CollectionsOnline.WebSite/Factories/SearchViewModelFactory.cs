using System;
using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.WebSite.Extensions;
using CollectionsOnline.WebSite.Models;
using Humanizer;
using Nancy.Helpers;
using Raven.Abstractions.Data;

namespace CollectionsOnline.WebSite.Factories
{
    public class SearchViewModelFactory : ISearchViewModelFactory
    {
        public SearchIndexViewModel MakeSearchIndex(
            IList<EmuAggregateRootViewModel> results,
            FacetResults facets,
            int totalResults,
            SearchInputModel searchInputModel)
        {
            var searchIndexViewModel = new SearchIndexViewModel
            {
                Results = results,
                Queries = searchInputModel.Queries,
                TotalResults = totalResults,
                TotalPages = (totalResults + searchInputModel.PerPage - 1) / searchInputModel.PerPage,
                Page = searchInputModel.Page,
                PerPage = searchInputModel.PerPage,
            };

            // Build facets
            foreach (var facet in facets.Results)
            {
                // Dont add any deprecated facets to results
                if (Core.Config.Constants.DeprecatedFacets.Contains(facet.Key, StringComparison.OrdinalIgnoreCase))
                    continue;
                
                var facetViewModel = new FacetViewModel { Name = facet.Key.Humanize() };

                foreach (var facetValue in facet.Value.Values)
                {
                    if (facetValue.Range != "NULL_VALUE")
                    {
                        var facetValueQueryString = HttpUtility.ParseQueryString(searchInputModel.CurrentQueryString);
                        facetValueQueryString.Remove("page");

                        var facetValues = facetValueQueryString.GetQueryStringValues(facet.Key.ToLower());

                        var facetValueViewModel = new FacetValueViewModel
                        {
                            Facet = facet.Key,
                            Hits = facetValue.Hits,
                            Name = facetValue.Range.Transform(facetValue.Range.StartsWith("cc by") ? To.UpperCase : To.TitleCase),
                            Class = string.Equals(facet.Key, "RecordType", StringComparison.OrdinalIgnoreCase) ? facetValue.Range : null
                        };

                        if (facetValues != null && facetValues.Any(x => string.Equals(x, facetValue.Range, StringComparison.OrdinalIgnoreCase)))
                        {
                            facetValueQueryString.Remove(facet.Key.ToLower());

                            foreach (var value in facetValues.Where(x => !string.Equals(x, facetValue.Range, StringComparison.OrdinalIgnoreCase)))
                            {
                                facetValueQueryString.Add(facet.Key.ToLower(), value);
                            }

                            facetValueViewModel.Active = true;
                        }
                        else
                        {
                            facetValueQueryString.Add(facet.Key.ToLower(), facetValue.Range);
                        }

                        facetValueViewModel.Url = (facetValueQueryString.Count > 0)
                            ? String.Concat(searchInputModel.CurrentUrl, "?", facetValueQueryString.RenderQueryString())
                            : searchInputModel.CurrentUrl;

                        facetViewModel.Values.Add(facetValueViewModel);
                    }
                }

                if (facetViewModel.Values.Any())
                {
                    facetViewModel.Values = facetViewModel.Values
                        .OrderByDescending(x => x.Active)
                        .ThenByDescending(x => x.Hits)
                        .ToList();

                    searchIndexViewModel.Facets.Add(facetViewModel);
                }
            }

            // Build ActiveFacets
            searchIndexViewModel.ActiveFacets = searchIndexViewModel.Facets
                .SelectMany(x => x.Values)
                .Where(y => y.Active)
                .Select(x => new ActiveFacetValueViewModel
                {
                    Facet = x.Facet.Humanize(),
                    Name = x.Name,
                    UrlToRemove = x.Url
                }).ToList();

            // Build ActiveQueries
            foreach (var query in searchInputModel.Queries)
            {
                var termQueryString = HttpUtility.ParseQueryString(searchInputModel.CurrentQueryString);

                var otherQueries = termQueryString.GetQueryStringValues("query").Where(x => !string.Equals(x, query));

                termQueryString.Remove("query");

                foreach (var currentQuery in otherQueries)
                {
                    termQueryString.Add("query", currentQuery);
                }

                // remove sort relevance if set
                if (!string.IsNullOrWhiteSpace(termQueryString["sort"]) && string.Equals(termQueryString["sort"], "relevance", StringComparison.OrdinalIgnoreCase))
                    termQueryString.Remove("sort");

                searchIndexViewModel.ActiveTerms.Add(new ActiveTermViewModel
                {
                    Name = query,
                    Term = "Query",
                    UrlToRemove = (termQueryString.Count > 0) ? String.Concat(searchInputModel.CurrentUrl, "?", termQueryString.RenderQueryString()) : searchInputModel.CurrentUrl
                });
            }
            
            // Build ActiveTerms
            foreach (var term in searchInputModel.Terms)
            {
                var termQueryString = HttpUtility.ParseQueryString(searchInputModel.CurrentQueryString);

                termQueryString.Remove(term.Key);

                searchIndexViewModel.ActiveTerms.Add(new ActiveTermViewModel
                {
                    Name = term.Value,
                    Term = term.Key,
                    UrlToRemove = (termQueryString.Count > 0) ? String.Concat(searchInputModel.CurrentUrl, "?", termQueryString.RenderQueryString()) : searchInputModel.CurrentUrl
                });
            }
            
            // Build next prev page links
            var queryString = HttpUtility.ParseQueryString(searchInputModel.CurrentQueryString);
            if ((searchInputModel.Page + 1) <= searchIndexViewModel.TotalPages)
            {
                queryString.Set("page", (searchInputModel.Page + 1).ToString());
                searchIndexViewModel.NextPageUrl = String.Concat(searchInputModel.CurrentUrl, "?", queryString.RenderQueryString());
            }

            if ((searchInputModel.Page - 1) >= 1)
            {
                queryString = HttpUtility.ParseQueryString(searchInputModel.CurrentQueryString);
                queryString.Set("page", (searchInputModel.Page - 1).ToString());
                if ((searchInputModel.Page - 1) == 1)
                {
                    queryString.Remove("page");
                }
                searchIndexViewModel.PreviousPageUrl = (queryString.Count > 0) ? String.Concat(searchInputModel.CurrentUrl, "?", queryString.RenderQueryString()) : searchInputModel.CurrentUrl;
            }

            // Build sort links
            //TODO: sort this mess out
            queryString = HttpUtility.ParseQueryString(searchInputModel.CurrentQueryString);
            queryString.Set("sort", "quality");
            searchIndexViewModel.QualitySortButton = new ButtonViewModel
            {
                Name = "Quality",
                Url = String.Concat(searchInputModel.CurrentUrl, "?", queryString.RenderQueryString()),
                Active = searchInputModel.Sort == "quality" || string.IsNullOrWhiteSpace(searchInputModel.Sort)
            };

            queryString = HttpUtility.ParseQueryString(searchInputModel.CurrentQueryString);
            queryString.Set("sort", "relevance");
            searchIndexViewModel.RelevanceSortButton = new ButtonViewModel
            {
                Name = "Relevance",
                Url = String.Concat(searchInputModel.CurrentUrl, "?", queryString.RenderQueryString()),
                Active = searchInputModel.Sort == "relevance"
            };

            queryString = HttpUtility.ParseQueryString(searchInputModel.CurrentQueryString);
            queryString.Set("sort", "date");
            searchIndexViewModel.DateSortButton = new ButtonViewModel
            {
                Name = "Date modified",
                Url = String.Concat(searchInputModel.CurrentUrl, "?", queryString.RenderQueryString()),
                Active = searchInputModel.Sort == "date"
            };

            queryString = HttpUtility.ParseQueryString(searchInputModel.CurrentQueryString);
            queryString.Set("perpage", Core.Config.Constants.PagingPerPageDefault.ToString());
            queryString.Remove("page");
            searchIndexViewModel.DefaultPerPageButton = new ButtonViewModel
            {
                Name = Core.Config.Constants.PagingPerPageDefault.ToString(),
                Url = String.Concat(searchInputModel.CurrentUrl, "?", queryString.RenderQueryString()),
                Active = searchInputModel.PerPage == Core.Config.Constants.PagingPerPageDefault
            };

            queryString = HttpUtility.ParseQueryString(searchInputModel.CurrentQueryString);
            queryString.Set("perpage", Core.Config.Constants.PagingPerPageMax.ToString());
            queryString.Remove("page");
            searchIndexViewModel.MaxPerPageButton = new ButtonViewModel
            {
                Name = Core.Config.Constants.PagingPerPageMax.ToString(),
                Url = String.Concat(searchInputModel.CurrentUrl, "?", queryString.RenderQueryString()),
                Active = searchInputModel.PerPage == Core.Config.Constants.PagingPerPageMax
            };

            // View links
            queryString = HttpUtility.ParseQueryString(searchInputModel.CurrentQueryString);
            queryString.Set("view", "list");
            searchIndexViewModel.ListViewButton = new ButtonViewModel
            {
                Name = "List",
                Url = String.Concat(searchInputModel.CurrentUrl, "?", queryString.RenderQueryString()),
                Active = searchInputModel.View == "list" || string.IsNullOrWhiteSpace(searchInputModel.View)
            };

            queryString = HttpUtility.ParseQueryString(searchInputModel.CurrentQueryString);
            queryString.Set("view", "grid");
            searchIndexViewModel.GridViewButton = new ButtonViewModel
            {
                Name = "Grid",
                Url = String.Concat(searchInputModel.CurrentUrl, "?", queryString.RenderQueryString()),
                Active = searchInputModel.View == "grid"
            };

            queryString = HttpUtility.ParseQueryString(searchInputModel.CurrentQueryString);
            queryString.Set("view", "data");
            searchIndexViewModel.DataViewButton = new ButtonViewModel
            {
                Name = "Data",
                Url = String.Concat(searchInputModel.CurrentUrl, "?", queryString.RenderQueryString()),
                Active = searchInputModel.View == "data"
            };

            searchIndexViewModel.CsvDownloadUrl = String.Concat(searchInputModel.CurrentUrl, ".csv", searchInputModel.CurrentQueryString);

            return searchIndexViewModel;
        }
    }
}