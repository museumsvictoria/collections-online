using System.Collections.Generic;

namespace CollectionsOnline.Core.Models
{
    public class Feature : AggregateRoot
    {
        public Feature()
        {
            InitializeCollections();
        }
        
        public bool IsHidden { get; set; }
        
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        public IList<string> FeaturedIds { get; set; }

        private void InitializeCollections()
        {
            FeaturedIds = new List<string>();
        }
    }
}