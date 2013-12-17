using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebSite.Features.Search
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

        public SearchViewModel BuildSearch(SearchInputModel searchInputModel, Request request)
        {
            // normalize inputs
            if (searchInputModel.Offset < 0)
                searchInputModel.Offset = 0;
            if (searchInputModel.Limit <= 0 || searchInputModel.Limit > Constants.WebApiPagingPageSizeMax)
                searchInputModel.Limit = Constants.WebApiPagingPageSizeDefault;
            searchInputModel.Query = searchInputModel.Query ?? string.Empty;

            // get facets
            var facets = _documentSession.Advanced
                .LuceneQuery<CombinedSearchResult, CombinedSearch>();

            // perform query
            var query = _documentSession.Advanced
                .LuceneQuery<CombinedSearchResult, CombinedSearch>()
                .Skip(searchInputModel.Offset)
                .Take(searchInputModel.Limit);

            // search query
            if (!string.IsNullOrWhiteSpace(searchInputModel.Query))
            {
                query = query.Search("Content", searchInputModel.Query);
                facets = facets.Search("Content", searchInputModel.Query);
            }

            // facet queries TODO: REFACTOR this
            if (!string.IsNullOrWhiteSpace(searchInputModel.Type))
            {
                query = query.AndAlso().WhereEquals("Type", searchInputModel.Type);
                facets = facets.AndAlso().WhereEquals("Type", searchInputModel.Type);
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.Category))
            {
                query = query.AndAlso().WhereEquals("Category", searchInputModel.Category);
                facets = facets.AndAlso().WhereEquals("Category", searchInputModel.Category);
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.ItemType))
            {
                query = query.AndAlso().WhereEquals("ItemType", searchInputModel.ItemType);
                facets = facets.AndAlso().WhereEquals("ItemType", searchInputModel.ItemType);
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.SpeciesType))
            {
                query = query.AndAlso().WhereEquals("SpeciesType", searchInputModel.SpeciesType);
                facets = facets.AndAlso().WhereEquals("SpeciesType", searchInputModel.SpeciesType);
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.SpeciesSubType))
            {
                query = query.AndAlso().WhereEquals("SpeciesSubType", searchInputModel.SpeciesSubType);
                facets = facets.AndAlso().WhereEquals("SpeciesSubType", searchInputModel.SpeciesSubType);
            }
            if (searchInputModel.SpeciesHabitats != null && searchInputModel.SpeciesHabitats.Any())
            {
                query = query.AndAlso().WhereIn("SpeciesHabitats", searchInputModel.SpeciesHabitats);
                facets = facets.AndAlso().WhereIn("SpeciesHabitats", searchInputModel.SpeciesHabitats);
            }

            return _searchViewModelFactory.MakeViewModel(
                query
                    .SelectFields<CombinedSearchResult>(
                        "Id",
                        "Name",
                        "Type",                        
                        "Tags",
                        "Country",
                        "ItemCollectionNames",
                        "ItemPrimaryClassification",
                        "ItemSecondaryClassification",
                        "ItemTertiaryClassification",
                        "ItemAssociationNames",
                        "ItemTradeLiteraturePrimarySubject",
                        "ItemTradeLiteraturePublicationDate",
                        "ItemTradeLiteraturePrimaryRole",
                        "ItemTradeLiteraturePrimaryName")
                    .ToList(),
                facets.ToFacets("facets/combinedFacets"),
                request,
                query.QueryResult.TotalResults,
                searchInputModel);
        }
    }
}