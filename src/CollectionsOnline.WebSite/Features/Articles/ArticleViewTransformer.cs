using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Features.Shared;
using Raven.Client.Indexes;

namespace CollectionsOnline.WebSite.Features.Articles
{
    public class ArticleViewTransformer : AbstractTransformerCreationTask<Article>
    {
        public ArticleViewTransformer()
        {
            TransformResults = articles => from article in articles
                select new
                {
                    Article = article,
                    RelatedItems = from itemId in article.RelatedItemIds
                        let relatedItem = LoadDocument<Item>(itemId)
                        where relatedItem != null && !relatedItem.IsHidden
                        select new
                        {
                            relatedItem.Id, 
                            relatedItem.ThumbnailUri,
                            relatedItem.DisplayTitle
                        },
                    RelatedSpecimens = from specimenId in article.RelatedSpecimenIds
                        let relatedSpecimen = LoadDocument<Specimen>(specimenId)
                        where relatedSpecimen != null && !relatedSpecimen.IsHidden
                        select new
                        {
                            relatedSpecimen.Id,
                            relatedSpecimen.ThumbnailUri,
                            relatedSpecimen.DisplayTitle
                        },
                    ParentArticle = (LoadDocument<Article>(article.ParentArticleId) != null && !LoadDocument<Article>(article.ParentArticleId).IsHidden) ? LoadDocument<Article>(article.ParentArticleId) : null,
                    ChildArticles = from articleId in article.ChildArticleIds
                        let relatedArticle = LoadDocument<Article>(articleId)
                        where relatedArticle != null && !relatedArticle.IsHidden
                        select new
                        {
                            relatedArticle.Id,
                            relatedArticle.ThumbnailUri,
                            relatedArticle.DisplayTitle
                        },
                    RelatedArticles = from articleId in article.RelatedArticleIds
                        let relatedArticle = LoadDocument<Article>(articleId)
                        where relatedArticle != null && !relatedArticle.IsHidden
                        select new
                        {
                            relatedArticle.Id,
                            relatedArticle.ThumbnailUri,
                            relatedArticle.DisplayTitle
                        }
                };
        }
    }

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