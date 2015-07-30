using System.Collections.Generic;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models;

namespace CollectionsOnline.WebSite.Transformers
{
    public class SpeciesViewTransformerResult
    {
        public Species Species { get; set; }

        public IList<EmuAggregateRootViewModel> RelatedItems { get; set; }

        public IList<EmuAggregateRootViewModel> RelatedSpecimens { get; set; }

        public IList<EmuAggregateRootViewModel> RelatedArticles { get; set; }

        public IList<EmuAggregateRootViewModel> RelatedSpecies { get; set; }

        public int RelatedSpecimenCount { get; set; }

        public SpeciesViewTransformerResult()
        {
            RelatedItems = new List<EmuAggregateRootViewModel>();
            RelatedSpecimens = new List<EmuAggregateRootViewModel>();
            RelatedArticles = new List<EmuAggregateRootViewModel>();
            RelatedSpecies = new List<EmuAggregateRootViewModel>();
        }
    }
}