﻿using System.Collections.Generic;
using System.Linq;

namespace CollectionsOnline.WebSite.Models
{
    public class SearchViewModel
    {
        public int Page { get; set; }

        public int PerPage { get; set; }

        public string Sort { get; set; }

        public string View { get; set; }

        public string Query { get; set; }

        public int TotalResults { get; set; }

        public int TotalPages { get; set; }

        public long QueryTimeElapsed { get; set; }

        public long FacetTimeElapsed { get; set; }

        public ButtonViewModel NextPageButton { get; set; }

        public ButtonViewModel PreviousPageButton { get; set; }

        public ButtonViewModel QualitySortButton { get; set; }

        public ButtonViewModel RelevanceSortButton { get; set; }

        public ButtonViewModel DefaultPerPageButton { get; set; }

        public ButtonViewModel MaxPerPageButton { get; set; }

        public IList<FacetViewModel> Facets { get; set; }

        public int TotalFacetValues
        {
            get
            {
                return Facets.SelectMany(x => x.Values).Count();
            }
        }

        public IList<EmuAggregateRootViewModel> Results { get; set; }

        public IList<ActiveFacetValueViewModel> ActiveFacets { get; set; }

        public IList<ActiveTermViewModel> ActiveTerms { get; set; }

        public IList<SuggestionViewModel> Suggestions { get; set; }

        public SearchViewModel()
        {
            Facets = new List<FacetViewModel>();
            Results = new List<EmuAggregateRootViewModel>();
            ActiveFacets = new List<ActiveFacetValueViewModel>();
            ActiveTerms = new List<ActiveTermViewModel>();
            Suggestions = new List<SuggestionViewModel>();
        }
    }

    public class FacetViewModel
    {
        public string Name { get; set; }

        public IList<FacetValueViewModel> Values { get; set; }

        public FacetViewModel()
        {
            Values = new List<FacetValueViewModel>();
        }
    }

    public class FacetValueViewModel
    {
        public string Facet { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public bool Active { get; set; }

        public int Hits { get; set; }
    }

    public class ActiveFacetValueViewModel
    {
        public string Facet { get; set; }

        public string Name { get; set; }

        public string UrlToRemove { get; set; }
    }

    public class ActiveTermViewModel
    {
        public string Term { get; set; }

        public string Name { get; set; }

        public string UrlToRemove { get; set; }
    }

    public class SuggestionViewModel
    {
        public string Suggestion { get; set; }

        public string Url { get; set; }
    }

    public class ButtonViewModel
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public bool Active { get; set; }
    }
}