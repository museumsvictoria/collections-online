using System.Collections.Generic;

namespace CollectionsOnline.WebSite.Models
{
    public class MetadataViewModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string CanonicalUri { get; set; }

        public IList<KeyValuePair<string, string>> MetaProperties { get; set; }
        
        public MetadataViewModel()
        {
            // Defaults
            Title = "Museums Victoria Collections";

            MetaProperties = new List<KeyValuePair<string, string>>();
        }
    }
}