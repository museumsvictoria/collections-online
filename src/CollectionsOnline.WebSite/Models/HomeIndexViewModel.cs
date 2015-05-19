using System.Collections.Generic;

namespace CollectionsOnline.WebSite.Models
{
    public class HomeIndexViewModel
    {
        public int ArticleCount { get; set; }

        public int ItemCount { get; set; }

        public int SpeciesCount { get; set; }

        public int SpecimenCount { get; set; }

        public IList<EmuAggregateRootViewModel> RecentResults { get; set; }

        public HomeIndexViewModel()
        {
            RecentResults = new List<EmuAggregateRootViewModel>();
        }
    }
}