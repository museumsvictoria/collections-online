using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.WebSite.Models;
using Humanizer;
using Raven.Abstractions.Data;

namespace CollectionsOnline.WebSite.Factories
{
    public class SearchViewModelFactory : ISearchViewModelFactory
    {
        public SearchViewModel MakeViewModel(
            IList<CombinedResultViewModel> results,
            FacetResults facets,
            List<string> suggestions,
            int totalResults,
            SearchInputModel searchInputModel,
            long queryTimeElapsed,
            long facetTimeElapsed)
        {
            var searchViewModel = new SearchViewModel
            {
                Query = searchInputModel.Query,
                TotalResults = totalResults,
                Limit = searchInputModel.Limit,
                Offset = searchInputModel.Offset,
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
                        facetValueQueryString.Remove("offset");

                        var facetValues = facetValueQueryString.GetValues(facet.Key.ToLower());

                        var facetValueViewModel = new FacetValueViewModel
                        {
                            Facet = facet.Key,
                            Name = facetValue.Range,
                            Hits = facetValue.Hits
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
                    searchViewModel.Facets.Add(facetViewModel);
            }

            // Build ActiveFacets
            searchViewModel.ActiveFacets = searchViewModel.Facets
                .SelectMany(x => x.Values)
                .Where(y => y.Active)
                .Select(x => new ActiveFacetValueViewModel
                {
                    Facet = x.Facet,
                    Name = x.Name,
                    UrlToRemove = x.Url
                }).ToList();

            // Build ActiveQuery
            if (!string.IsNullOrWhiteSpace(searchInputModel.Query))
            {
                var termQueryString = HttpUtility.ParseQueryString(searchInputModel.CurrentQueryString);

                termQueryString.Remove("query");

                searchViewModel.ActiveTerms.Add(new ActiveTermViewModel
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

                searchViewModel.ActiveTerms.Add(new ActiveTermViewModel
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

                searchViewModel.Results.Add(result);
            }

            // Build next prev page links
            var queryString = HttpUtility.ParseQueryString(searchInputModel.CurrentQueryString);
            if ((searchInputModel.Offset + searchInputModel.Limit) < totalResults)
            {
                queryString.Set("offset", (searchInputModel.Offset + searchInputModel.Limit).ToString());

                searchViewModel.NextPageUrl = String.Concat(searchInputModel.CurrentUrl, "?", queryString);
            }

            if ((searchInputModel.Offset - searchInputModel.Limit) >= 0)
            {
                queryString.Set("offset", (searchInputModel.Offset - searchInputModel.Limit).ToString());
                if ((searchInputModel.Offset - searchInputModel.Limit) == 0)
                {
                    queryString.Remove("offset");
                }

                searchViewModel.PrevPageUrl = (queryString.Count > 0) ? String.Concat(searchInputModel.CurrentUrl, "?", queryString) : searchInputModel.CurrentUrl;
            }

            // Build sort links
            queryString = HttpUtility.ParseQueryString(searchInputModel.CurrentQueryString);
            queryString.Set("sort", "quality");
            searchViewModel.QualitySortUrl = String.Concat(searchInputModel.CurrentUrl, "?", queryString);

            queryString = HttpUtility.ParseQueryString(searchInputModel.CurrentQueryString);
            queryString.Set("sort", "relevance");
            searchViewModel.RelevanceSortUrl = String.Concat(searchInputModel.CurrentUrl, "?", queryString);

            // Build suggestions
            foreach (var suggestion in suggestions)
            {
                searchViewModel.Suggestions.Add(new SuggestionViewModel
                {
                    Suggestion = suggestion,
                    Url = String.Concat(searchInputModel.CurrentUrl, "?query=", HttpUtility.UrlEncode(suggestion))
                });
            }
            
            return searchViewModel;
        }
    }
}