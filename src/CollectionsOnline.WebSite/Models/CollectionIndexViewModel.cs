using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Models;

namespace CollectionsOnline.WebSite.Models
{
    public class CollectionIndexViewModel
    {
        public IEnumerable<IGrouping<string, Collection>> Collections { get; set; }
    }
}