using System.Collections.Generic;

namespace CollectionsOnline.WebSite.Models
{
    public class HomeViewModel
    {
        public int ArticleCount { get; set; }

        public int ItemCount { get; set; }

        public int SpeciesCount { get; set; }

        public int SpecimenCount { get; set; }

        public IList<EmuAggregateRootViewModel> RecentResults { get; set; }

        public HomeViewModel()
        {
            RecentResults = new List<EmuAggregateRootViewModel>();
        }
    }
}