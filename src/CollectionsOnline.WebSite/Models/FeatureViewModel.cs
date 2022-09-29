using System.Collections.Generic;
using CollectionsOnline.Core.Models;

namespace CollectionsOnline.WebSite.Models
{
    public class FeatureViewModel
    {
        public Feature Feature { get; set; }

        public IList<EmuAggregateRootViewModel> FeaturedRecords { get; set; }

        public FeatureViewModel()
        {
            FeaturedRecords = new List<EmuAggregateRootViewModel>();
        }
    }
}