using System;
using System.Collections.Generic;

namespace CollectionsOnline.WebSite.Models.Api
{
    public class SearchApiInputModel
    {
        public string Sort { get; set; }

        public DateTime? MinDateModified { get; set; }

        public DateTime? MaxDateModified { get; set; }

        public IList<string> Queries { get; set; }

        public IDictionary<string, string> Facets { get; set; }

        public IDictionary<string, string[]> MultiFacets { get; set; }

        public IDictionary<string, string> Terms { get; set; }

        public SearchApiInputModel()
        {
            Queries = new List<string>();
            Facets = new Dictionary<string, string>();
            MultiFacets = new Dictionary<string, string[]>();
            Terms = new Dictionary<string, string>();
        }
    }
}