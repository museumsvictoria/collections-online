using System;
using System.Linq;
using CollectionsOnline.WebSite.Models.Api;
using Nancy;
using Nancy.ModelBinding;
using Raven.Abstractions.Extensions;

namespace CollectionsOnline.WebSite.ModelBinders
{
    public class SearchApiInputModelBinder : IModelBinder
    {
        public object Bind(NancyContext context, Type modelType, object instance, BindingConfig configuration, params string[] blackList)
        {
            var queryString = context.Request.Query;
            var searchApiInputModel = new SearchApiInputModel();

            // Set Queries
            if (queryString.Query.HasValue && !string.IsNullOrWhiteSpace(queryString.Query.Value))
                searchApiInputModel.Queries.AddRange(((string)queryString.Query.Value).Split(',').Where(x => !string.IsNullOrWhiteSpace(x)));

            // Set Sort
            if (string.Equals(queryString.Sort, "quality", StringComparison.OrdinalIgnoreCase))
                searchApiInputModel.Sort = "quality";
            else if (string.Equals(queryString.Sort, "relevance", StringComparison.OrdinalIgnoreCase))
                searchApiInputModel.Sort = "relevance";
            else if (string.Equals(queryString.Sort, "date", StringComparison.OrdinalIgnoreCase))
                searchApiInputModel.Sort = "date";

            // Set Date modified
            if (queryString.MinDateModified.HasValue)
            {
                try
                {
                    searchApiInputModel.MinDateModified = Convert.ChangeType(queryString.MinDateModified, typeof(DateTime));
                }
                catch
                {
                    searchApiInputModel.MinDateModified = null;
                }
            }
            if (queryString.MaxDateModified.HasValue)
            {
                try
                {
                    searchApiInputModel.MaxDateModified = Convert.ChangeType(queryString.MaxDateModified, typeof(DateTime));
                }
                catch
                {
                    searchApiInputModel.MaxDateModified = null;
                }
            }

            // Set include TODO:to implement 
            //bool include = false;
            //if (queryString.Include.HasValue && bool.TryParse(queryString.Include, out include))
            //    searchApiInputModel.Include = include;

            // switch to quality if we have no query or no sort
            if ((searchApiInputModel.Queries.All(string.IsNullOrWhiteSpace) && (string.Equals(searchApiInputModel.Sort, "relevance", StringComparison.OrdinalIgnoreCase) || string.Equals(queryString.Sort, "relevance", StringComparison.OrdinalIgnoreCase)))
                || string.IsNullOrWhiteSpace(searchApiInputModel.Sort))
                searchApiInputModel.Sort = "quality";

            // if a new search set relevance as sort type
            if (queryString.Count == 1 && searchApiInputModel.Queries.Count == 1)
                searchApiInputModel.Sort = "relevance";

            // Facets
            if (queryString.RecordType.HasValue)
                searchApiInputModel.Facets.Add("RecordType", queryString.RecordType);
            if (queryString.Category.HasValue)
                searchApiInputModel.Facets.Add("Category", queryString.Category);
            if (queryString.HasImages.HasValue)
                searchApiInputModel.Facets.Add("HasImages", queryString.HasImages);
            if (queryString.OnDisplay.HasValue)
                searchApiInputModel.Facets.Add("OnDisplay", queryString.OnDisplay);
            if (queryString.ItemType.HasValue)
                searchApiInputModel.Facets.Add("ItemType", queryString.ItemType);
            if (queryString.SpeciesType.HasValue)
                searchApiInputModel.Facets.Add("SpeciesType", queryString.SpeciesType);
            if (queryString.SpecimenScientificGroup.HasValue)
                searchApiInputModel.Facets.Add("SpecimenScientificGroup", queryString.SpecimenScientificGroup);
            if (queryString.DisplayLocation.HasValue)
                searchApiInputModel.Facets.Add("DisplayLocation", queryString.DisplayLocation);

            // Multi-select Facets
            if (queryString.ArticleType.HasValue)
                searchApiInputModel.MultiFacets.Add("ArticleType", ((string)queryString.ArticleType.Value).Split(','));
            if (queryString.CollectingArea.HasValue)
                searchApiInputModel.MultiFacets.Add("CollectingArea", ((string)queryString.CollectingArea.Value).Split(','));

            // Terms
            if (queryString.Keyword.HasValue)
                searchApiInputModel.Terms.Add("Keyword", queryString.Keyword);
            if (queryString.Locality.HasValue)
                searchApiInputModel.Terms.Add("Locality", queryString.Locality);
            if (queryString.Collection.HasValue)
                searchApiInputModel.Terms.Add("Collection", queryString.Collection);
            if (queryString.Date.HasValue)
                searchApiInputModel.Terms.Add("Date", queryString.Date);
            if (queryString.CulturalGroup.HasValue)
                searchApiInputModel.Terms.Add("CulturalGroup", queryString.CulturalGroup);
            if (queryString.Classification.HasValue)
                searchApiInputModel.Terms.Add("Classification", queryString.Classification);
            if (queryString.Name.HasValue)
                searchApiInputModel.Terms.Add("Name", queryString.Name);
            if (queryString.Technique.HasValue)
                searchApiInputModel.Terms.Add("Technique", queryString.Technique);
            if (queryString.Denomination.HasValue)
                searchApiInputModel.Terms.Add("Denomination", queryString.Denomination);
            if (queryString.Habitat.HasValue)
                searchApiInputModel.Terms.Add("Habitat", queryString.Habitat);
            if (queryString.Taxon.HasValue)
                searchApiInputModel.Terms.Add("Taxon", queryString.Taxon);
            if (queryString.TypeStatus.HasValue)
                searchApiInputModel.Terms.Add("TypeStatus", queryString.TypeStatus);
            if (queryString.GeoType.HasValue)
                searchApiInputModel.Terms.Add("GeoType", queryString.GeoType);
            if (queryString.MuseumLocation.HasValue)
                searchApiInputModel.Terms.Add("MuseumLocation", queryString.MuseumLocation);
            if (queryString.Article.HasValue)
                searchApiInputModel.Terms.Add("Article", queryString.Article);
            if (queryString.SpeciesEndemicity.HasValue)
                searchApiInputModel.Terms.Add("SpeciesEndemicity", queryString.SpeciesEndemicity);
            
            return searchApiInputModel;
        }

        public bool CanBind(Type modelType)
        {
            return modelType == typeof(SearchApiInputModel);
        }
    }
}