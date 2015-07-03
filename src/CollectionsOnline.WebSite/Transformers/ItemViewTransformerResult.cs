using System.Collections.Generic;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models;

namespace CollectionsOnline.WebSite.Transformers
{
    public class ItemViewTransformerResult
    {
        public Item Item { get; set; }

        public IList<Media> ItemMedia { get; set; }

        public IList<EmuAggregateRootViewModel> RelatedItems { get; set; }

        public IList<EmuAggregateRootViewModel> RelatedSpecimens { get; set; }

        public IList<EmuAggregateRootViewModel> RelatedArticles { get; set; }

        public IList<EmuAggregateRootViewModel> RelatedSpecies { get; set; }

        public int RelatedSpeciesSpecimenItemCount { get; set; }

        public string JsonItemMedia { get; set; }
        
        public ItemViewTransformerResult()
        {
            ItemMedia = new List<Media>();
            RelatedItems = new List<EmuAggregateRootViewModel>();
            RelatedSpecimens = new List<EmuAggregateRootViewModel>();
            RelatedArticles = new List<EmuAggregateRootViewModel>();
            RelatedSpecies = new List<EmuAggregateRootViewModel>();
        }
    }
}