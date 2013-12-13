using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using Raven.Client;

namespace CollectionsOnline.WebSite.Features.Search
{
    public class SearchViewModelQuery : ISearchViewModelQuery
    {
        private readonly IDocumentSession _documentSession;

        public SearchViewModelQuery(
            IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public ExpandoObject BuildSearch(SearchInputModel searchInputModel)
        {
            // Normalize inputs
            if (searchInputModel.Offset < 0)
                searchInputModel.Offset = 0;
            if (searchInputModel.Limit <= 0 || searchInputModel.Limit > Constants.WebApiPagingPageSizeMax)
                searchInputModel.Limit = Constants.WebApiPagingPageSizeDefault;
            searchInputModel.Query = searchInputModel.Query ?? string.Empty;

            var facets = _documentSession.Advanced
                .LuceneQuery<CombinedSearchResult, CombinedSearch>();

            var query = _documentSession.Advanced
                .LuceneQuery<CombinedSearchResult, CombinedSearch>()
                .Skip(searchInputModel.Offset)
                .Take(searchInputModel.Limit);

            if (!string.IsNullOrWhiteSpace(searchInputModel.Query))
            {
                query = query.Search("Content", searchInputModel.Query);
                facets = facets.Search("Content", searchInputModel.Query);
            }

            if (!string.IsNullOrWhiteSpace(searchInputModel.Type))
            {
                query = query.WhereEquals("Type", searchInputModel.Type);
                facets = facets.WhereEquals("Type", searchInputModel.Type);
            }

            dynamic viewModel = new ExpandoObject();

            viewModel.Facets = facets.ToFacets("facets/combinedFacets");
            viewModel.Results = query.SelectFields<CombinedSearchResult>(
                "Id",
                "Name",
                "Type",
                "Content",
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
                "ItemTradeLiteraturePrimaryName");

            return viewModel;
        }
    }
}