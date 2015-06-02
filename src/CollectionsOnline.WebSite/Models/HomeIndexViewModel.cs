using System;
using System.Collections.Generic;
using CollectionsOnline.Core.Config;

namespace CollectionsOnline.WebSite.Models
{
    public class HomeIndexViewModel
    {
        public int ArticleCount { get; set; }

        public int ItemCount { get; set; }

        public int SpeciesCount { get; set; }

        public int SpecimenCount { get; set; }

        public IList<EmuAggregateRootViewModel> RecentResults { get; set; }

        // TODO: Automatically fetch and insert as inline styles hero images
        public string GetHeroImageClass
        {
            get
            {
                return string.Format("hero-image-{0}", DateTime.Now.DayOfYear % Constants.HomeHeroBackgroundImages + 1);
            }
        }

        public HomeIndexViewModel()
        {
            RecentResults = new List<EmuAggregateRootViewModel>();
        }
    }
}