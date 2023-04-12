using System.Collections.Generic;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models;

namespace CollectionsOnline.WebSite.Transformers
{
    public class SpeciesViewTransformerResult
    {
        public Species Species { get; set; }

        public IList<EmuAggregateRootViewModel> RelatedItems { get; }

        public IList<EmuAggregateRootViewModel> RelatedSpecimens { get; }

        public IList<EmuAggregateRootViewModel> RelatedArticles { get; }

        public IList<EmuAggregateRootViewModel> RelatedSpecies { get; }

        public SpeciesViewTransformerResult()
        {
            RelatedItems = new List<EmuAggregateRootViewModel>();
            RelatedSpecimens = new List<EmuAggregateRootViewModel>();
            RelatedArticles = new List<EmuAggregateRootViewModel>();
            RelatedSpecies = new List<EmuAggregateRootViewModel>();
        }
    }
}