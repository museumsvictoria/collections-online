using System;
using System.Collections.Generic;
using CollectionsOnline.Core.Config;
using CollectionsOnline.WebSite.Models;
using Nancy;
using Nancy.ModelBinding;

namespace CollectionsOnline.WebSite.ModelBinders
{
    public class SearchInputModelBinder : IModelBinder
    {
        public object Bind(NancyContext context, Type modelType, object instance, BindingConfig configuration, params string[] blackList)
        {
            var searchInputModel = new SearchInputModel();
            var query = context.Request.Query;

            // Bind and normalize the regular stuff
            if (query.Offset.HasValue)
                searchInputModel.Offset = query.Offset;
            if (searchInputModel.Offset < 0)
                searchInputModel.Offset = 0;

            if (query.Limit.HasValue)
                searchInputModel.Limit = query.Limit;
            if (searchInputModel.Limit <= 0 || searchInputModel.Limit > Constants.PagingPageSizeMax)
                searchInputModel.Limit = Constants.PagingPageSizeDefault;

            if (string.Equals(query.Sort, "quality", StringComparison.OrdinalIgnoreCase))
                searchInputModel.Sort = "quality";
            else if (string.Equals(query.Sort, "relevance", StringComparison.OrdinalIgnoreCase))
                searchInputModel.Sort = "relevance";

            searchInputModel.Query = query.Query;

            // Facets
            searchInputModel.Facets = new Dictionary<string, string>();
            if(query.Type.HasValue)
                searchInputModel.Facets.Add("Type", query.Type);
            if (query.Category.HasValue)
                searchInputModel.Facets.Add("Category", query.Category);
            if (query.HasImages.HasValue)
                searchInputModel.Facets.Add("HasImages", query.HasImages);
            if (query.OnDisplay.HasValue)
                searchInputModel.Facets.Add("OnDisplay", query.OnDisplay);
            if (query.ItemType.HasValue)
                searchInputModel.Facets.Add("ItemType", query.ItemType);
            if (query.SpeciesType.HasValue)
                searchInputModel.Facets.Add("SpeciesType", query.SpeciesType);
            if (query.SpeciesEndemicity.HasValue)
                searchInputModel.Facets.Add("SpeciesEndemicity", query.SpeciesEndemicity);
            if (query.SpecimenScientificGroup.HasValue)
                searchInputModel.Facets.Add("SpecimenScientificGroup", query.SpecimenScientificGroup);
            if (query.OnDisplayLocation.HasValue)
                searchInputModel.Facets.Add("OnDisplayLocation", query.OnDisplayLocation);

            // Multi-select Facets
            searchInputModel.MultiFacets = new Dictionary<string, string[]>();
            if (query.ArticleType.HasValue)
                searchInputModel.MultiFacets.Add("ArticleType", ((string)query.ArticleType.Value).Split(','));
            if (query.CollectionArea.HasValue)
                searchInputModel.MultiFacets.Add("CollectionArea", ((string)query.CollectionArea.Value).Split(','));

            // Terms
            searchInputModel.Terms = new Dictionary<string, string>();
            if (query.Keyword.HasValue)
                searchInputModel.Terms.Add("Keyword", query.Keyword);
            if (query.Locality.HasValue)
                searchInputModel.Terms.Add("Locality", query.Locality);
            if (query.Collection.HasValue)
                searchInputModel.Terms.Add("Collection", query.Collection);
            if (query.CulturalGroup.HasValue)
                searchInputModel.Terms.Add("CulturalGroup", query.CulturalGroup);
            if (query.Classification.HasValue)
                searchInputModel.Terms.Add("Classification", query.Classification);
            if (query.Name.HasValue)
                searchInputModel.Terms.Add("Name", query.Name);
            if (query.Technique.HasValue)
                searchInputModel.Terms.Add("Technique", query.Technique);
            if (query.Denomination.HasValue)
                searchInputModel.Terms.Add("Denomination", query.Denomination);
            if (query.Habitat.HasValue)
                searchInputModel.Terms.Add("Habitat", query.Habitat);
            if (query.Taxon.HasValue)
                searchInputModel.Terms.Add("Taxon", query.Taxon);
            if (query.TypeStatus.HasValue)
                searchInputModel.Terms.Add("TypeStatus", query.TypeStatus);
            if (query.GeoType.HasValue)
                searchInputModel.Terms.Add("GeoType", query.GeoType);
            if (query.MuseumLocation.HasValue)
                searchInputModel.Terms.Add("MuseumLocation", query.MuseumLocation);
            if (query.Article.HasValue)
                searchInputModel.Terms.Add("Article", query.Article);

            return searchInputModel;
        }

        public bool CanBind(Type modelType)
        {
            return modelType == typeof(SearchInputModel);
        }
    }
}