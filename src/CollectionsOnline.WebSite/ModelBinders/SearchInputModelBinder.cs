using System;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.WebSite.Models;
using Nancy;
using Nancy.Cookies;
using Nancy.ModelBinding;
using Raven.Abstractions.Extensions;

namespace CollectionsOnline.WebSite.ModelBinders
{
    public class SearchInputModelBinder : IModelBinder
    {
        public object Bind(NancyContext context, Type modelType, object instance, BindingConfig configuration, params string[] blackList)
        {
            var queryString = context.Request.Query;
            var searchInputModel = new SearchInputModel();

            // Find Cookies
            if (context.Request.Cookies.ContainsKey("perPage"))
            {
                int perPage;
                if (int.TryParse(context.Request.Cookies["perPage"], out perPage))
                    searchInputModel.PerPage = perPage;
            }
            if (context.Request.Cookies.ContainsKey("sort"))
            {
                searchInputModel.Sort = context.Request.Cookies["sort"];
            }
            if (context.Request.Cookies.ContainsKey("view"))
            {
                searchInputModel.View = context.Request.Cookies["view"];
            }

            // Bind view
            if (string.Equals(context.Request.Query.View, "list", StringComparison.OrdinalIgnoreCase))
                searchInputModel.View = "list";
            else if (string.Equals(context.Request.Query.View, "grid", StringComparison.OrdinalIgnoreCase))
                searchInputModel.View = "grid";
            else if (string.Equals(context.Request.Query.View, "data", StringComparison.OrdinalIgnoreCase))
                searchInputModel.View = "data";

            // Set Queries
            if (queryString.Query.HasValue && !string.IsNullOrWhiteSpace(queryString.Query.Value))
                searchInputModel.Queries.AddRange(((string)queryString.Query.Value).Split(',').Where(x => !string.IsNullOrWhiteSpace(x)));

            // Set urls
            searchInputModel.CurrentUrl = context.Request.Url.Path;
            searchInputModel.CurrentQueryString = context.Request.Url.Query;

            // Bind and normalize the regular stuff
            if (queryString.Page.HasValue)
                searchInputModel.Page = queryString.Page;
            if (searchInputModel.Page <= 0)
                searchInputModel.Page = 1;

            if (queryString.PerPage.HasValue)
                searchInputModel.PerPage = queryString.PerPage;
            if (searchInputModel.PerPage != Constants.PagingPerPageDefault && searchInputModel.PerPage != Constants.PagingPerPageMax)
                searchInputModel.PerPage = Constants.PagingPerPageDefault;

            if (string.Equals(queryString.Sort, "quality", StringComparison.OrdinalIgnoreCase))
                searchInputModel.Sort = "quality";
            else if (string.Equals(queryString.Sort, "relevance", StringComparison.OrdinalIgnoreCase))
                searchInputModel.Sort = "relevance";
            else if (string.Equals(queryString.Sort, "date", StringComparison.OrdinalIgnoreCase))
                searchInputModel.Sort = "date";

            // switch to quality if we have no query or no sort
            if ((searchInputModel.Queries.All(string.IsNullOrWhiteSpace) && (string.Equals(searchInputModel.Sort, "relevance", StringComparison.OrdinalIgnoreCase) || string.Equals(queryString.Sort, "relevance", StringComparison.OrdinalIgnoreCase)))
                || string.IsNullOrWhiteSpace(searchInputModel.Sort))
                searchInputModel.Sort = "quality";

            // if a new search set relevance as sort type
            if (queryString.Count == 1 && searchInputModel.Queries.Count == 1)
                searchInputModel.Sort = "relevance";

            // Facets
            if (queryString.RecordType.HasValue)
                searchInputModel.Facets.Add("RecordType", queryString.RecordType);
            if (queryString.Category.HasValue)
                searchInputModel.Facets.Add("Category", queryString.Category);
            if (queryString.HasImages.HasValue)
                searchInputModel.Facets.Add("HasImages", queryString.HasImages);
            if (queryString.OnDisplay.HasValue)
                searchInputModel.Facets.Add("OnDisplay", queryString.OnDisplay);
            if (queryString.ItemType.HasValue)
                searchInputModel.Facets.Add("ItemType", queryString.ItemType);
            if (queryString.SpeciesType.HasValue)
                searchInputModel.Facets.Add("SpeciesType", queryString.SpeciesType);
            if (queryString.SpecimenScientificGroup.HasValue)
                searchInputModel.Facets.Add("SpecimenScientificGroup", queryString.SpecimenScientificGroup);
            if (queryString.DisplayLocation.HasValue)
                searchInputModel.Facets.Add("DisplayLocation", queryString.DisplayLocation);

            // Multi-select Facets
            if (queryString.ArticleType.HasValue)
                searchInputModel.MultiFacets.Add("ArticleType", ((string)queryString.ArticleType.Value).Split(','));
            if (queryString.CollectingArea.HasValue)
                searchInputModel.MultiFacets.Add("CollectingArea", ((string)queryString.CollectingArea.Value).Split(','));

            // Terms
            if (queryString.Keyword.HasValue)
                searchInputModel.Terms.Add("Keyword", queryString.Keyword);
            if (queryString.Locality.HasValue)
                searchInputModel.Terms.Add("Locality", queryString.Locality);
            if (queryString.Collection.HasValue)
                searchInputModel.Terms.Add("Collection", queryString.Collection);
            if (queryString.Date.HasValue)
                searchInputModel.Terms.Add("Date", queryString.Date);
            if (queryString.CulturalGroup.HasValue)
                searchInputModel.Terms.Add("CulturalGroup", queryString.CulturalGroup);
            if (queryString.Classification.HasValue)
                searchInputModel.Terms.Add("Classification", queryString.Classification);
            if (queryString.Name.HasValue)
                searchInputModel.Terms.Add("Name", queryString.Name);
            if (queryString.Technique.HasValue)
                searchInputModel.Terms.Add("Technique", queryString.Technique);
            if (queryString.Denomination.HasValue)
                searchInputModel.Terms.Add("Denomination", queryString.Denomination);
            if (queryString.Habitat.HasValue)
                searchInputModel.Terms.Add("Habitat", queryString.Habitat);
            if (queryString.Taxon.HasValue)
                searchInputModel.Terms.Add("Taxon", queryString.Taxon);
            if (queryString.TypeStatus.HasValue)
                searchInputModel.Terms.Add("TypeStatus", queryString.TypeStatus);
            if (queryString.GeoType.HasValue)
                searchInputModel.Terms.Add("GeoType", queryString.GeoType);
            if (queryString.MuseumLocation.HasValue)
                searchInputModel.Terms.Add("MuseumLocation", queryString.MuseumLocation);
            if (queryString.Article.HasValue)
                searchInputModel.Terms.Add("Article", queryString.Article);
            if (queryString.SpeciesEndemicity.HasValue)
                searchInputModel.Terms.Add("SpeciesEndemicity", queryString.SpeciesEndemicity);

            // Add Cookies
            searchInputModel.Cookies.Add(new NancyCookie("perPage", searchInputModel.PerPage.ToString(), true, false, DateTime.Now.AddMonths(3)) { Path = "/search" });
            searchInputModel.Cookies.Add(new NancyCookie("sort", searchInputModel.Sort, true, false, DateTime.Now.AddMonths(3)) { Path = "/search" });
            searchInputModel.Cookies.Add(new NancyCookie("view", searchInputModel.View, true, false, DateTime.Now.AddMonths(3)) { Path = "/search" });
            
            return searchInputModel;
        }

        public bool CanBind(Type modelType)
        {
            return modelType == typeof(SearchInputModel);
        }
    }
}