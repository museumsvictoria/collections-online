using System.Collections.Generic;
using CollectionsOnline.Core.Models;

namespace CollectionsOnline.WebSite.Models
{
    public class CollectionIndexViewModel
    {
        public IList<Collection> Collections { get; set; }

        public CollectionIndexViewModel()
        {
            Collections = new List<Collection>();
        }
    }
}