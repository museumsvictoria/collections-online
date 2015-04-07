using System.Collections.Generic;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models;

namespace CollectionsOnline.WebSite.Transformers
{
    public class ArticleViewTransformerResult
    {
        public Article Article { get; set; }

        public IList<EmuAggregateRootViewModel> RelatedItems { get; set; }

        public IList<EmuAggregateRootViewModel> RelatedSpecimens { get; set; }

        public EmuAggregateRootViewModel ParentArticle { get; set; }

        public IList<EmuAggregateRootViewModel> ChildArticles { get; set; }

        public IList<EmuAggregateRootViewModel> RelatedArticles { get; set; }

        public int RelatedItemSpecimenCount { get; set; }

        public IList<ImageMedia> ArticleImages { get; set; }

        public string JsonArticleImages { get; set; }

        public ArticleViewTransformerResult()
        {
            RelatedItems = new List<EmuAggregateRootViewModel>();
            RelatedSpecimens = new List<EmuAggregateRootViewModel>();
            ChildArticles = new List<EmuAggregateRootViewModel>();
            RelatedArticles = new List<EmuAggregateRootViewModel>();
            ArticleImages = new List<ImageMedia>();
        }
    }
}