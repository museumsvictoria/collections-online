using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Utilities;
using Nancy;
using Raven.Abstractions.Data;

namespace CollectionsOnline.WebSite.Features.Search
{
    public class SearchViewModelFactory : ISearchViewModelFactory
    {
        public SearchViewModel MakeViewModel(IList<CombinedSearchResult> results, FacetResults facets, List<string> suggestions, Request request, int totalResults, SearchInputModel searchInputModel, long queryTimeElapsed, long facetTimeElapsed)
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
                var facetViewModel = new FacetViewModel { Name = facet.Key };

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
                            Name = facetValue.Range,
                            Hits = facetValue.Hits
                        };

                        if (facetValues != null && facetValues.Contains(facetValue.Range))
                        {
                            facetValueQueryString.Remove(facet.Key.ToLower());

                            foreach (var value in facetValues.Where(x => x != facetValue.Range))
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

            // Build ActiveTerms
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
            if (!string.IsNullOrWhiteSpace(searchInputModel.Tag))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("tag");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.Tag,
                    Term = "Tag",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.Country))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("country");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.Country,
                    Term = "Country",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.CollectionName))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("collectionname");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.CollectionName,
                    Term = "CollectionName",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.CollectionPlan))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("collectionplan");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.CollectionPlan,
                    Term = "CollectionPlan",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.PrimaryClassification))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("primaryclassification");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.PrimaryClassification,
                    Term = "PrimaryClassification",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.SecondaryClassification))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("secondaryclassification");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.SecondaryClassification,
                    Term = "SecondaryClassification",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.TertiaryClassification))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("tertiaryclassification");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.TertiaryClassification,
                    Term = "TertiaryClassification",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.AssociationName))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("associationname");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.AssociationName,
                    Term = "AssociationName",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.ItemTradeLiteraturePrimarySubject))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("itemtradeliteratureprimarysubject");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.ItemTradeLiteraturePrimarySubject,
                    Term = "ItemTradeLiteraturePrimarySubject",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.ItemTradeLiteraturePublicationDate))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("itemtradeliteraturepublicationdate");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.ItemTradeLiteraturePublicationDate,
                    Term = "ItemTradeLiteraturePublicationDate",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.ItemTradeLiteraturePrimaryRole))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("itemtradeliteratureprimaryrole");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.ItemTradeLiteraturePrimaryRole,
                    Term = "ItemTradeLiteraturePrimaryRole",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.ItemTradeLiteraturePrimaryName))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("itemtradeliteratureprimaryname");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.ItemTradeLiteraturePrimaryName,
                    Term = "ItemTradeLiteraturePrimaryName",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }

            // Build results
            foreach (var result in results)
            {
                // Trim summary to fit
                if (result.Summary != null)
                    result.Summary = result.Summary.Truncate(Core.Config.Constants.SummaryMaxChars);

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