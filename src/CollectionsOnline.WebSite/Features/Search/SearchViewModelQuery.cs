using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Web;
using CollectionsOnline.Core.Indexes;
using Nancy;
using Raven.Abstractions.Data;
using Raven.Client;
using Constants = CollectionsOnline.Core.Config.Constants;

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

            var queryStopwatch = new Stopwatch();
            var facetStopwatch = new Stopwatch();

            // perform query
            queryStopwatch.Start();
            var query = _documentSession.Advanced
                .LuceneQuery<CombinedSearchResult, CombinedSearch>()
                .Skip(searchInputModel.Offset)
                .Take(searchInputModel.Limit);

            // get facets
            facetStopwatch.Start();
            var facetQuery = _documentSession.Advanced
                .LuceneQuery<CombinedSearchResult, CombinedSearch>();

            // search query
            if (!string.IsNullOrWhiteSpace(searchInputModel.Query))
            {
                query = query.Search("Content", searchInputModel.Query);
                facetQuery = facetQuery.Search("Content", searchInputModel.Query);
            }

            // facet queries TODO: REFACTOR this
            if (!string.IsNullOrWhiteSpace(searchInputModel.Type))
            {
                query = query.AndAlso().WhereEquals("Type", searchInputModel.Type);
                facetQuery = facetQuery.AndAlso().WhereEquals("Type", searchInputModel.Type);
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.Category))
            {
                query = query.AndAlso().WhereEquals("Category", searchInputModel.Category);
                facetQuery = facetQuery.AndAlso().WhereEquals("Category", searchInputModel.Category);
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.HasImages))
            {
                query = query.AndAlso().WhereEquals("HasImages", searchInputModel.HasImages);
                facetQuery = facetQuery.AndAlso().WhereEquals("HasImages", searchInputModel.HasImages);
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.ItemType))
            {
                query = query.AndAlso().WhereEquals("ItemType", searchInputModel.ItemType);
                facetQuery = facetQuery.AndAlso().WhereEquals("ItemType", searchInputModel.ItemType);
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.SpeciesType))
            {
                query = query.AndAlso().WhereEquals("SpeciesType", searchInputModel.SpeciesType);
                facetQuery = facetQuery.AndAlso().WhereEquals("SpeciesType", searchInputModel.SpeciesType);
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.SpeciesSubType))
            {
                query = query.AndAlso().WhereEquals("SpeciesSubType", searchInputModel.SpeciesSubType);
                facetQuery = facetQuery.AndAlso().WhereEquals("SpeciesSubType", searchInputModel.SpeciesSubType);
            }
            if (searchInputModel.SpeciesHabitats != null && searchInputModel.SpeciesHabitats.Any())
            {
                foreach (var speciesHabitat in searchInputModel.SpeciesHabitats.Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    query = query.AndAlso().WhereEquals("SpeciesHabitats", speciesHabitat);
                    facetQuery = facetQuery.AndAlso().WhereEquals("SpeciesHabitats", speciesHabitat);
                }
            }
            if (searchInputModel.SpeciesDepths != null && searchInputModel.SpeciesDepths.Any())
            {
                foreach (var speciesDepth in searchInputModel.SpeciesDepths.Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    query = query.AndAlso().WhereEquals("SpeciesDepths", speciesDepth);
                    facetQuery = facetQuery.AndAlso().WhereEquals("SpeciesDepths", speciesDepth);
                }
            }
            if (searchInputModel.SpeciesWaterColumnLocations != null && searchInputModel.SpeciesWaterColumnLocations.Any())
            {
                foreach (var speciesWaterColumnLocation in searchInputModel.SpeciesWaterColumnLocations.Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    query = query.AndAlso().WhereEquals("SpeciesWaterColumnLocations", speciesWaterColumnLocation);
                    facetQuery = facetQuery.AndAlso().WhereEquals("SpeciesWaterColumnLocations", speciesWaterColumnLocation);
                }
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.Phylum))
            {
                query = query.AndAlso().WhereEquals("Phylum", searchInputModel.Phylum);
                facetQuery = facetQuery.AndAlso().WhereEquals("Phylum", searchInputModel.Phylum);
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.Class))
            {
                query = query.AndAlso().WhereEquals("Class", searchInputModel.Class);
                facetQuery = facetQuery.AndAlso().WhereEquals("Class", searchInputModel.Class);
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.Order))
            {
                query = query.AndAlso().WhereEquals("Order", searchInputModel.Order);
                facetQuery = facetQuery.AndAlso().WhereEquals("Order", searchInputModel.Order);
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.Family))
            {
                query = query.AndAlso().WhereEquals("Family", searchInputModel.Family);
                facetQuery = facetQuery.AndAlso().WhereEquals("Family", searchInputModel.Family);
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.SpecimenScientificGroup))
            {
                query = query.AndAlso().WhereEquals("SpecimenScientificGroup", searchInputModel.SpecimenScientificGroup);
                facetQuery = facetQuery.AndAlso().WhereEquals("SpecimenScientificGroup", searchInputModel.SpecimenScientificGroup);
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.SpecimenDiscipline))
            {
                query = query.AndAlso().WhereEquals("SpecimenDiscipline", searchInputModel.SpecimenDiscipline);
                facetQuery = facetQuery.AndAlso().WhereEquals("SpecimenDiscipline", searchInputModel.SpecimenDiscipline);
            }
            if (searchInputModel.StoryTypes != null && searchInputModel.StoryTypes.Any())
            {
                foreach (var storyType in searchInputModel.StoryTypes.Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    query = query.AndAlso().WhereEquals("StoryTypes", storyType);
                    facetQuery = facetQuery.AndAlso().WhereEquals("StoryTypes", storyType);
                }
            }

            /* Term queries */
            if (!string.IsNullOrWhiteSpace(searchInputModel.Tag))
            {
                query = query.AndAlso().WhereEquals("Tags", searchInputModel.Tag);
                facetQuery = facetQuery.AndAlso().WhereEquals("Tags", searchInputModel.Tag);
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.Country))
            {
                query = query.AndAlso().WhereEquals("Country", searchInputModel.Country);
                facetQuery = facetQuery.AndAlso().WhereEquals("Country", searchInputModel.Country);
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.ItemCollectionName))
            {
                query = query.AndAlso().WhereEquals("ItemCollectionNames", searchInputModel.ItemCollectionName);
                facetQuery = facetQuery.AndAlso().WhereEquals("ItemCollectionNames", searchInputModel.ItemCollectionName);
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.ItemPrimaryClassification))
            {
                query = query.AndAlso().WhereEquals("ItemPrimaryClassification", searchInputModel.ItemPrimaryClassification);
                facetQuery = facetQuery.AndAlso().WhereEquals("ItemPrimaryClassification", searchInputModel.ItemPrimaryClassification);
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.ItemSecondaryClassification))
            {
                query = query.AndAlso().WhereEquals("ItemSecondaryClassification", searchInputModel.ItemSecondaryClassification);
                facetQuery = facetQuery.AndAlso().WhereEquals("ItemSecondaryClassification", searchInputModel.ItemSecondaryClassification);
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.ItemTertiaryClassification))
            {
                query = query.AndAlso().WhereEquals("ItemTertiaryClassification", searchInputModel.ItemTertiaryClassification);
                facetQuery = facetQuery.AndAlso().WhereEquals("ItemTertiaryClassification", searchInputModel.ItemTertiaryClassification);
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.ItemAssociationName))
            {
                query = query.AndAlso().WhereEquals("ItemAssociationNames", searchInputModel.ItemAssociationName);
                facetQuery = facetQuery.AndAlso().WhereEquals("ItemAssociationNames", searchInputModel.ItemAssociationName);
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.ItemTradeLiteraturePrimarySubject))
            {
                query = query.AndAlso().WhereEquals("ItemTradeLiteraturePrimarySubject", searchInputModel.ItemTradeLiteraturePrimarySubject);
                facetQuery = facetQuery.AndAlso().WhereEquals("ItemTradeLiteraturePrimarySubject", searchInputModel.ItemTradeLiteraturePrimarySubject);
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.ItemTradeLiteraturePublicationDate))
            {
                query = query.AndAlso().WhereEquals("ItemTradeLiteraturePublicationDate", searchInputModel.ItemTradeLiteraturePublicationDate);
                facetQuery = facetQuery.AndAlso().WhereEquals("ItemTradeLiteraturePublicationDate", searchInputModel.ItemTradeLiteraturePublicationDate);
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.ItemTradeLiteraturePrimaryRole))
            {
                query = query.AndAlso().WhereEquals("ItemTradeLiteraturePrimaryRole", searchInputModel.ItemTradeLiteraturePrimaryRole);
                facetQuery = facetQuery.AndAlso().WhereEquals("ItemTradeLiteraturePrimaryRole", searchInputModel.ItemTradeLiteraturePrimaryRole);
            }
            if (!string.IsNullOrWhiteSpace(searchInputModel.ItemTradeLiteraturePrimaryName))
            {
                query = query.AndAlso().WhereEquals("ItemTradeLiteraturePrimaryName", searchInputModel.ItemTradeLiteraturePrimaryName);
                facetQuery = facetQuery.AndAlso().WhereEquals("ItemTradeLiteraturePrimaryName", searchInputModel.ItemTradeLiteraturePrimaryName);
            }

            var results = query
                .SelectFields<CombinedSearchResult>(
                    "Id",
                    "Name",
                    "Type",
                    "ItemType",
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
                .ToList();
            queryStopwatch.Stop();

            var facets = facetQuery.ToFacets("facets/combinedFacets");
            facetStopwatch.Stop();
            
            return _searchViewModelFactory.MakeViewModel(
                results,
                facets,
                request,
                query.QueryResult.TotalResults,
                searchInputModel,
                queryStopwatch.ElapsedMilliseconds,
                facetStopwatch.ElapsedMilliseconds);
        }
    }
}