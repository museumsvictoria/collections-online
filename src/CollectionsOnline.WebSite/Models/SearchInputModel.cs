using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Extensions;
using Nancy.Cookies;

namespace CollectionsOnline.WebSite.Models
{
    public class SearchInputModel
    {
        public int Page { get; set; }

        public int PerPage { get; set; }

        public string Sort { get; set; }

        public string View { get; set; }

        public IList<string> Queries { get; set; }

        public string CurrentUrl { get; set; }

        public string CurrentQueryString { get; set; }

        public IDictionary<string, string> Facets { get; set; }

        public IDictionary<string, string[]> MultiFacets { get; set; }

        public IDictionary<string, string> Terms { get; set; }

        public IList<NancyCookie> Cookies { get; set; }

        public SearchInputModel()
        {
            Queries = new List<string>();
            Facets = new Dictionary<string, string>();
            MultiFacets = new Dictionary<string, string[]>();
            Terms = new Dictionary<string, string>();
            Cookies = new List<NancyCookie>();
        }

        public override string ToString()
        {
            var searchInputStrings = new List<string>
            {
                (Queries.Any()) ? string.Format("query-{0}", Queries.Select(x => x.ReplaceNonWordWithDashes()).Concatenate("-")) : null,
                (Facets.Any()) ? Facets.Select(x => string.Format("{0}-{1}", x.Key, x.Value)).Concatenate("-") : null,
                (MultiFacets.Any()) ? MultiFacets.Select(x => string.Format("{0}-{1}", x.Key, x.Value.Concatenate("-"))).Concatenate("-") : null,
                (Terms.Any()) ? Terms.Select(x => string.Format("{0}-{1}", x.Key, x.Value)).Concatenate("-") : null,
            };

            if (searchInputStrings.All(x => x == null))
            {
                searchInputStrings.Add("search");
            }

            searchInputStrings.Add(string.Format("page-{0}", Page));

            return searchInputStrings
                .Concatenate("-")
                .Replace("&", "and")
                .ReplaceNonWordWithDashes()
                .ToLowerInvariant();
        }
    }
}