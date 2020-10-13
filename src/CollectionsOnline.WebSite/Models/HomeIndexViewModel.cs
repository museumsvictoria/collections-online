using System;
using System.Collections.Generic;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Models;

namespace CollectionsOnline.WebSite.Models
{
    public class HomeIndexViewModel
    {
        public int ArticleCount { get; set; }

        public int ItemCount { get; set; }

        public int SpeciesCount { get; set; }

        public int SpecimenCount { get; set; }

        public IList<EmuAggregateRootViewModel> RecentResults { get; set; }

        public string HomeHeroUri { get; set; }
        
        public Feature Feature { get; set; }

        public List<EmuAggregateRootViewModel> FeaturedRecords { get; set; }

        public HomeIndexViewModel()
        {
            RecentResults = new List<EmuAggregateRootViewModel>();
            FeaturedRecords = new List<EmuAggregateRootViewModel>();
        }
    }
}