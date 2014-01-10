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

                //if (!(facet.Key == "Class" && string.IsNullOrWhiteSpace(searchInputModel.Phylum)) &&
                //    !(facet.Key == "Order" && string.IsNullOrWhiteSpace(searchInputModel.Class)) &&
                //    !(facet.Key == "Family" && string.IsNullOrWhiteSpace(searchInputModel.Order)) &&
                //    !(facet.Key == "SpeciesSubType" && string.IsNullOrWhiteSpace(searchInputModel.SpeciesType)))
                //{
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
                //}

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
            if (!string.IsNullOrWhiteSpace(searchInputModel.ItemCollectionName))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("itemcollectionName");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.ItemCollectionName,
                    Term = "ItemCollectionName",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.ItemPrimaryClassification))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("itemprimaryclassification");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.ItemPrimaryClassification,
                    Term = "ItemPrimaryClassification",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.ItemSecondaryClassification))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("itemsecondaryclassification");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.ItemSecondaryClassification,
                    Term = "ItemSecondaryClassification",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.ItemTertiaryClassification))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("itemtertiaryclassification");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.ItemTertiaryClassification,
                    Term = "ItemTertiaryClassification",
                    Url = (termQueryString.Count > 0) ? String.Concat(baseUrl, "?", termQueryString) : baseUrl
                });
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.ItemAssociationName))
            {
                var termQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                termQueryString.Remove("itemassociationname");

                searchViewModel.ActiveTerms.Add(new TermViewModel
                {
                    Name = searchInputModel.ItemAssociationName,
                    Term = "ItemAssociationName",
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


            // Build results TODO: Eventually wont be featured on search results page but on object page.
            foreach (var result in results)
            {
                var searchResultViewModel = new SearchResultViewModel { Result = result };

                if (result.Tags != null && result.Tags.Any())
                {
                    foreach (var tag in result.Tags.Where(x => !string.IsNullOrWhiteSpace(x as string)))
                    {
                        searchResultViewModel.Terms.Add(new TermViewModel
                        {
                            Name = tag as string,
                            Term = "Tag",
                            Url = String.Concat(baseUrl, "?tag=", tag)
                        });
                    }
                }
                if (result.Country != null && result.Country.Any())
                {
                    foreach (var country in result.Country.Where(x => !string.IsNullOrWhiteSpace(x as string)))
                    {
                        searchResultViewModel.Terms.Add(new TermViewModel
                        {
                            Name = country as string,
                            Term = "Country",
                            Url = String.Concat(baseUrl, "?country=", HttpUtility.UrlEncode(country as string))
                        });
                    }
                }
                if (result.ItemCollectionNames != null && result.ItemCollectionNames.Any())
                {
                    foreach (var itemCollectionName in result.ItemCollectionNames.Where(x => !string.IsNullOrWhiteSpace(x as string)))
                    {
                        searchResultViewModel.Terms.Add(new TermViewModel
                        {
                            Name = itemCollectionName as string,
                            Term = "ItemCollectionName",
                            Url = String.Concat(baseUrl, "?itemcollectionname=", HttpUtility.UrlEncode(itemCollectionName as string))
                        });
                    }
                }
                if (!string.IsNullOrWhiteSpace(result.ItemPrimaryClassification))
                {
                    searchResultViewModel.Terms.Add(new TermViewModel
                    {
                        Name = result.ItemPrimaryClassification,
                        Term = "ItemPrimaryClassification",
                        Url = String.Concat(baseUrl, "?itemprimaryclassification=", HttpUtility.UrlEncode(result.ItemPrimaryClassification))
                    });
                }
                if (!string.IsNullOrWhiteSpace(result.ItemSecondaryClassification))
                {
                    searchResultViewModel.Terms.Add(new TermViewModel
                    {
                        Name = result.ItemSecondaryClassification,
                        Term = "ItemSecondaryClassification",
                        Url = String.Concat(baseUrl, "?itemsecondaryclassification=", HttpUtility.UrlEncode(result.ItemSecondaryClassification))
                    });
                }
                if (!string.IsNullOrWhiteSpace(result.ItemTertiaryClassification))
                {
                    searchResultViewModel.Terms.Add(new TermViewModel
                    {
                        Name = result.ItemTertiaryClassification,
                        Term = "ItemTertiaryClassification",
                        Url = String.Concat(baseUrl, "?itemtertiaryclassification=", HttpUtility.UrlEncode(result.ItemTertiaryClassification))
                    });
                }
                if (result.ItemAssociationNames != null && result.ItemAssociationNames.Any())
                {
                    foreach (var itemAssociationName in result.ItemAssociationNames.Where(x => !string.IsNullOrWhiteSpace(x as string)))
                    {
                        searchResultViewModel.Terms.Add(new TermViewModel
                        {
                            Name = itemAssociationName as string,
                            Term = "ItemCollectionName",
                            Url = String.Concat(baseUrl, "?itemassociationname=", HttpUtility.UrlEncode(itemAssociationName as string))
                        });
                    }
                }
                if (!string.IsNullOrWhiteSpace(result.ItemTradeLiteraturePrimarySubject))
                {
                    searchResultViewModel.Terms.Add(new TermViewModel
                    {
                        Name = result.ItemTradeLiteraturePrimarySubject,
                        Term = "ItemTradeLiteraturePrimarySubject",
                        Url = String.Concat(baseUrl, "?itemtradeliteratureprimarysubject=", HttpUtility.UrlEncode(result.ItemTradeLiteraturePrimarySubject))
                    });
                }
                if (!string.IsNullOrWhiteSpace(result.ItemTradeLiteraturePublicationDate))
                {
                    searchResultViewModel.Terms.Add(new TermViewModel
                    {
                        Name = result.ItemTradeLiteraturePublicationDate,
                        Term = "ItemTradeLiteraturePublicationDate",
                        Url = String.Concat(baseUrl, "?itemtradeliteraturepublicationdate=", HttpUtility.UrlEncode(result.ItemTradeLiteraturePublicationDate))
                    });
                }
                if (!string.IsNullOrWhiteSpace(result.ItemTradeLiteraturePrimaryRole))
                {
                    searchResultViewModel.Terms.Add(new TermViewModel
                    {
                        Name = result.ItemTradeLiteraturePrimaryRole,
                        Term = "ItemTradeLiteraturePrimaryRole",
                        Url = String.Concat(baseUrl, "?itemtradeliteratureprimaryrole=", HttpUtility.UrlEncode(result.ItemTradeLiteraturePrimaryRole))
                    });
                }
                if (!string.IsNullOrWhiteSpace(result.ItemTradeLiteraturePrimaryName))
                {
                    searchResultViewModel.Terms.Add(new TermViewModel
                    {
                        Name = result.ItemTradeLiteraturePrimaryName,
                        Term = "ItemTradeLiteraturePrimaryName",
                        Url = String.Concat(baseUrl, "?itemtradeliteratureprimaryname=", HttpUtility.UrlEncode(result.ItemTradeLiteraturePrimaryName))
                    });
                }

                searchViewModel.Results.Add(searchResultViewModel);

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