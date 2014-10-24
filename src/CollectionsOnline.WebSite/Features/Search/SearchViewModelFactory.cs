using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Utilities;
using Humanizer;
using Nancy;
using Raven.Abstractions.Data;

namespace CollectionsOnline.WebSite.Features.Search
{
    public class SearchViewModelFactory : ISearchViewModelFactory
    {
        public SearchViewModel MakeViewModel(IList<CombinedResult> results, FacetResults facets, List<string> suggestions, Request request, int totalResults, SearchInputModel searchInputModel, long queryTimeElapsed, long facetTimeElapsed)
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

            var baseUrl = string.Format("{0}{1}", request.Url.SiteBase, request.Url.Path);

            // Build facets
            foreach (var facet in facets.Results)
            {
                var facetViewModel = new FacetViewModel { Name = facet.Key.Humanize() };

                foreach (var facetValue in facet.Value.Values)
                {
                    if (facetValue.Range != "NULL_VALUE")
                    {
                        var facetValueQueryString = HttpUtility.ParseQueryString(request.Url.Query);
                        facetValueQueryString.Remove("offset");

                        var facetValues = facetValueQueryString.GetValues(facet.Key.ToLower());

                        var facetValueViewModel = new FacetValueViewModel
                        {
                            Facet = facet.Key,
                            Name = facetValue.Range.Humanize(),
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
                            ? String.Concat(baseUrl, "?", facetValueQueryString)
                            : baseUrl;

                        facetViewModel.Values.Add(facetValueViewModel);
                    }
                }

                if (facetViewModel.Values.Any())
                    searchViewModel.Facets.Add(facetViewModel);
            }

            // Build ActiveFacets
            searchViewModel.ActiveFacets = searchViewModel.Facets.SelectMany(x => x.Values).Where(y => y.Active).ToList();

            // Build ActiveQuery
            if (!string.IsNullOrWhiteSpace(searchInputModel.Query))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("query");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.Query,
                    Term = "Query",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            
            // Build ActiveTerms
            if (!string.IsNullOrWhiteSpace(searchInputModel.Keyword))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("keyword");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.Keyword,
                    Term = "Keyword",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.Locality))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("locality");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.Locality,
                    Term = "Locality",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.Collection))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("collection");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.Collection,
                    Term = "Collection",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.CulturalGroup))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("culturalgroup");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.CulturalGroup,
                    Term = "CulturalGroup",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.Classification))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("classification");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.Classification,
                    Term = "classification",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.Name))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("name");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.Name,
                    Term = "Name",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.Technique))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("technique");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.Technique,
                    Term = "Technique",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.Denomination))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("denomination");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.Denomination,
                    Term = "Denomination",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.Habitat))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("habitat");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.Habitat,
                    Term = "Habitat",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.Phylum))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("phylum");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.Phylum,
                    Term = "Phylum",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.Class))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("class");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.Class,
                    Term = "Class",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.Order))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("order");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.Order,
                    Term = "Order",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.Family))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("family");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.Family,
                    Term = "Family",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.TypeStatus))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("typestatus");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.TypeStatus,
                    Term = "TypeStatus",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.GeoType))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("geotype");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.GeoType,
                    Term = "GeoType",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.MuseumLocation))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("museumlocation");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.MuseumLocation,
                    Term = "MuseumLocation",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.Article))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("article");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.Article,
                    Term = "article",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.Species))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("species");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.Species,
                    Term = "Species",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            //TODO: temp term query
            if (!string.IsNullOrWhiteSpace(searchInputModel.Taxonomy))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("taxonomy");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.Taxonomy,
                    Term = "Taxonomy",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
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
            var queryString = HttpUtility.ParseQueryString(request.Url.Query);
            if ((searchInputModel.Offset + searchInputModel.Limit) < totalResults)
            {
                queryString.Set("offset", (searchInputModel.Offset + searchInputModel.Limit).ToString());

                searchViewModel.NextPageUrl = String.Concat(baseUrl, "?", queryString);
            }

            if ((searchInputModel.Offset - searchInputModel.Limit) >= 0)
            {
                queryString.Set("offset", (searchInputModel.Offset - searchInputModel.Limit).ToString());
                if ((searchInputModel.Offset - searchInputModel.Limit) == 0)
                {
                    queryString.Remove("offset");
                }

                searchViewModel.PrevPageUrl = (queryString.Count > 0) ? String.Concat(baseUrl, "?", queryString) : baseUrl;
            }

            // Build suggestions
            foreach (var suggestion in suggestions)
            {
                searchViewModel.Suggestions.Add(new SuggestionViewModel
                {
                    Suggestion = suggestion,
                    Url = String.Concat(baseUrl, "?query=", HttpUtility.UrlEncode(suggestion))
                });
            }

            return searchViewModel;
        }
    }
}