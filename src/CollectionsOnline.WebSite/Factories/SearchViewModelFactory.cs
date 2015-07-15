using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.WebSite.Models;
using Humanizer;
using Raven.Abstractions.Data;

namespace CollectionsOnline.WebSite.Factories
{
    public class SearchViewModelFactory : ISearchViewModelFactory
    {
        public SearchIndexViewModel MakeSearchIndex(
            IList<EmuAggregateRootViewModel> results,
            FacetResults facets,
            List<string> suggestions,
            int totalResults,
            SearchInputModel searchInputModel,
            long queryTimeElapsed,
            long facetTimeElapsed)
        {
            var searchIndexViewModel = new SearchIndexViewModel
            {
                Query = searchInputModel.Query,
                TotalResults = totalResults,
                TotalPages = (totalResults + searchInputModel.PerPage - 1) / searchInputModel.PerPage,
                Page = searchInputModel.Page,
                PerPage = searchInputModel.PerPage,
                QueryTimeElapsed = queryTimeElapsed,
                FacetTimeElapsed = facetTimeElapsed
            };

            // Build facets
            foreach (var facet in facets.Results)
            {
                var facetViewModel = new FacetViewModel { Name = facet.Key.Humanize() };

                foreach (var facetValue in facet.Value.Values)
                {
                    if (facetValue.Range != "NULL_VALUE")
                    {
                        var facetValueQueryString = HttpUtility.ParseQueryString(searchInputModel.CurrentQueryString);
                        facetValueQueryString.Remove("page");

                        var facetValues = facetValueQueryString.GetValues(facet.Key.ToLower());

                        var facetValueViewModel = new FacetValueViewModel
                        {
                            Facet = facet.Key,
                            Name = facetValue.Range,
                            Hits = facetValue.Hits,
                            Class = string.Equals(facet.Key, "RecordType", StringComparison.OrdinalIgnoreCase) ? facetValue.Range : null
                        };

                        if (facetValues != null && facetValues.Contains(facetValue.Range, StringComparison.OrdinalIgnoreCase))
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
                            ? String.Concat(searchInputModel.CurrentUrl, "?", facetValueQueryString)
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

            // Build ActiveQuery
            if (!string.IsNullOrWhiteSpace(searchInputModel.Query))
            {
                var termQueryString = HttpUtility.ParseQueryString(searchInputModel.CurrentQueryString);

                termQueryString.Remove("query");

                searchIndexViewModel.ActiveTerms.Add(new ActiveTermViewModel
                {
                    Name = searchInputModel.Query,
                    Term = "Query",
                    UrlToRemove = (termQueryString.Count > 0) ? String.Concat(searchInputModel.CurrentUrl, "?", termQueryString) : searchInputModel.CurrentUrl
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
                    UrlToRemove = (termQueryString.Count > 0) ? String.Concat(searchInputModel.CurrentUrl, "?", termQueryString) : searchInputModel.CurrentUrl
                });
            }

            // Build results
            foreach (var result in results)
            {
                // Trim summary to fit
                if (result.Summary != null)
                    result.Summary = StringExtensions.Truncate(result.Summary, Core.Config.Constants.SummaryMaxChars);

                searchIndexViewModel.Results.Add(result);
            }

            // Build next prev page links
            var queryString = HttpUtility.ParseQueryString(searchInputModel.CurrentQueryString);
            if ((searchInputModel.Page + 1) <= searchIndexViewModel.TotalPages)
            {
                queryString.Set("page", (searchInputModel.Page + 1).ToString());
                searchIndexViewModel.NextPageUrl = String.Concat(searchInputModel.CurrentUrl, "?", queryString);
            }

            if ((searchInputModel.Page - 1) >= 1)
            {
                queryString.Set("page", (searchInputModel.Page - 1).ToString());
                if ((searchInputModel.Page - 1) == 1)
                {
                    queryString.Remove("page");
                }
                searchIndexViewModel.PreviousPageUrl = (queryString.Count > 0) ? String.Concat(searchInputModel.CurrentUrl, "?", queryString) : searchInputModel.CurrentUrl;
            }

            // Build sort links
            //TODO: sort this mess out
            queryString = HttpUtility.ParseQueryString(searchInputModel.CurrentQueryString);
            queryString.Set("sort", "quality");
            searchIndexViewModel.QualitySortButton = new ButtonViewModel
            {
                Name = "Quality",
                Url = String.Concat(searchInputModel.CurrentUrl, "?", queryString),
                Active = searchInputModel.Sort == "quality" || string.IsNullOrWhiteSpace(searchInputModel.Sort)
            };

            queryString = HttpUtility.ParseQueryString(searchInputModel.CurrentQueryString);
            queryString.Set("sort", "relevance");
            searchIndexViewModel.RelevanceSortButton = new ButtonViewModel
            {
                Name = "Relevance",
                Url = String.Concat(searchInputModel.CurrentUrl, "?", queryString),
                Active = searchInputModel.Sort == "relevance"
            };

            queryString = HttpUtility.ParseQueryString(searchInputModel.CurrentQueryString);
            queryString.Set("sort", "date");
            searchIndexViewModel.DateSortButton = new ButtonViewModel
            {
                Name = "Date",
                Url = String.Concat(searchInputModel.CurrentUrl, "?", queryString),
                Active = searchInputModel.Sort == "date"
            };

            queryString = HttpUtility.ParseQueryString(searchInputModel.CurrentQueryString);
            queryString.Set("perpage", Core.Config.Constants.PagingPerPageDefault.ToString());
            queryString.Remove("page");
            searchIndexViewModel.DefaultPerPageButton = new ButtonViewModel
            {
                Name = Core.Config.Constants.PagingPerPageDefault.ToString(),
                Url = String.Concat(searchInputModel.CurrentUrl, "?", queryString),
                Active = searchInputModel.PerPage == Core.Config.Constants.PagingPerPageDefault
            };

            queryString = HttpUtility.ParseQueryString(searchInputModel.CurrentQueryString);
            queryString.Set("perpage", Core.Config.Constants.PagingPerPageMax.ToString());
            queryString.Remove("page");
            searchIndexViewModel.MaxPerPageButton = new ButtonViewModel
            {
                Name = Core.Config.Constants.PagingPerPageMax.ToString(),
                Url = String.Concat(searchInputModel.CurrentUrl, "?", queryString),
                Active = searchInputModel.PerPage == Core.Config.Constants.PagingPerPageMax
            };

            // View links
            queryString = HttpUtility.ParseQueryString(searchInputModel.CurrentQueryString);
            queryString.Set("view", "list");
            searchIndexViewModel.ListViewButton = new ButtonViewModel
            {
                Name = "List",
                Url = String.Concat(searchInputModel.CurrentUrl, "?", queryString),
                Active = searchInputModel.View == "list" || string.IsNullOrWhiteSpace(searchInputModel.View)
            };

            queryString = HttpUtility.ParseQueryString(searchInputModel.CurrentQueryString);
            queryString.Set("view", "grid");
            searchIndexViewModel.GridViewButton = new ButtonViewModel
            {
                Name = "Grid",
                Url = String.Concat(searchInputModel.CurrentUrl, "?", queryString),
                Active = searchInputModel.View == "grid"
            };

            // Build suggestions
            foreach (var suggestion in suggestions)
            {
                searchIndexViewModel.Suggestions.Add(new SuggestionViewModel
                {
                    Suggestion = suggestion,
                    Url = String.Concat(searchInputModel.CurrentUrl, "?query=", HttpUtility.UrlEncode(suggestion))
                });
            }
            
            return searchIndexViewModel;
        }
    }
}