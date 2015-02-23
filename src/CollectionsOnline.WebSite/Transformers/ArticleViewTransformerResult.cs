using System.Collections.Generic;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models;

namespace CollectionsOnline.WebSite.Transformers
{
    public class ArticleViewTransformerResult
    {
        public Article Article { get; set; }

        public IList<RelatedDocumentViewModel> RelatedItems { get; set; }

        public IList<RelatedDocumentViewModel> RelatedSpecimens { get; set; }

        public RelatedDocumentViewModel ParentArticle { get; set; }

        public IList<RelatedDocumentViewModel> ChildArticles { get; set; }

        public IList<RelatedDocumentViewModel> RelatedArticles { get; set; }

        public int RelatedItemSpecimenCount { get; set; }

        public ImageMedia ArticleHeroImage { get; set; }

        public IList<ImageMedia> ArticleImages { get; set; }

        public ArticleViewTransformerResult()
        {
            RelatedItems = new List<RelatedDocumentViewModel>();
            RelatedSpecimens = new List<RelatedDocumentViewModel>();
            ChildArticles = new List<RelatedDocumentViewModel>();
            RelatedArticles = new List<RelatedDocumentViewModel>();
            ArticleImages = new List<ImageMedia>();
        }
    }
}