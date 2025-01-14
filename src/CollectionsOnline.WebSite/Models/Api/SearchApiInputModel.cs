using System;
using System.Collections.Generic;

namespace CollectionsOnline.WebSite.Models.Api
{
    public class SearchApiInputModel
    {
        public string Sort { get; set; }

        public DateTime? MinDateModified { get; set; }

        public DateTime? MaxDateModified { get; set; }

        public IList<string> Queries { get; set; } = new List<string>();

        public IDictionary<string, string> Facets { get; set; } = new Dictionary<string, string>();

        public IDictionary<string, string[]> MultiFacets { get; set; } = new Dictionary<string, string[]>();

        public IDictionary<string, string> Terms { get; set; } = new Dictionary<string, string>();

        public IList<string> Ids { get; set; } = new List<string>();
    }
}