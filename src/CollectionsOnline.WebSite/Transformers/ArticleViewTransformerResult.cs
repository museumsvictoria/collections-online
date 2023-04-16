using System.Collections.Generic;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models;

namespace CollectionsOnline.WebSite.Transformers
{
    public class ArticleViewTransformerResult
    {
        public Article Article { get; set; }

        public IList<EmuAggregateRootViewModel> RelatedItems { get; }

        public IList<EmuAggregateRootViewModel> RelatedSpecimens { get; }

        public EmuAggregateRootViewModel ParentArticle { get; set; }

        public IList<EmuAggregateRootViewModel> ChildArticles { get; }

        public IList<EmuAggregateRootViewModel> RelatedArticles { get; }

        public IList<EmuAggregateRootViewModel> RelatedSpecies { get; }

        public int RelatedItemSpecimenCount { get; set; }

        public int TransformedItemSpecimenCount => RelatedItems.Count + RelatedSpecimens.Count;

        public ArticleViewTransformerResult()
        {
            RelatedItems = new List<EmuAggregateRootViewModel>();
            RelatedSpecimens = new List<EmuAggregateRootViewModel>();
            ChildArticles = new List<EmuAggregateRootViewModel>();
            RelatedArticles = new List<EmuAggregateRootViewModel>();
            RelatedSpecies = new List<EmuAggregateRootViewModel>();
        }
    }
}