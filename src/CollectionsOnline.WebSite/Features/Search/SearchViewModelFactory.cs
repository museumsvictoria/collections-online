using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CollectionsOnline.Core.Indexes;
using Nancy;
using Nancy.ViewEngines;
using Raven.Abstractions.Data;
using Raven.Client;

namespace CollectionsOnline.WebSite.Features.Search
{
    public class SearchViewModelFactory : ISearchViewModelFactory
    {
        public SearchViewModel MakeViewModel(IList<CombinedSearchResult> results, FacetResults facets, Request request, int totalResults, SearchInputModel searchInputModel)
        {
            var searchViewModel = new SearchViewModel
            {
                TotalResults = totalResults,
                Limit = searchInputModel.Limit,
                Offset = searchInputModel.Offset
            };

            var baseUrl = string.Format("{0}{1}", request.Url.SiteBase, request.Url.Path);

            // Build facets
            foreach (var facet in facets.Results)
            {
                var facetViewModel = new FacetViewModel { Name = facet.Key };

                foreach (var facetValue in facet.Value.Values)
                {
                    if (facetValue.Range != "NULL_VALUE")
                    {
                        var facetValueQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                        var facetValues = facetValueQueryString.GetValues(facet.Key.ToLower());

                        if (facetValues != null && facetValues.Contains(facetValue.Range))
                        {
                            facetValueQueryString.Remove(facet.Key.ToLower());
                            foreach (var value in facetValues.Where(x => x != facetValue.Range))
                            {
                                facetValueQueryString.Add(facet.Key.ToLower(), value);
                            }
                        }
                        else
                        {
                            facetValueQueryString.Add(facet.Key.ToLower(), facetValue.Range);
                        }

                        var facetValueUrl = (facetValueQueryString.Count > 0) ? String.Concat(baseUrl, "?", facetValueQueryString) : baseUrl;

                        facetViewModel.Values.Add(new KeyValuePair<string, string>(string.Format("{0} ({1})", facetValue.Range, facetValue.Hits), facetValueUrl));
                    }
                }

                if (facetViewModel.Values.Any())
                    searchViewModel.Facets.Add(facetViewModel);
            }

            // Build results
            foreach (var result in results)
            {
                var searchResultViewModel = new SearchResultViewModel { Result = result };

                searchViewModel.Results.Add(searchResultViewModel);
            }


            return searchViewModel;
        }
    }
}