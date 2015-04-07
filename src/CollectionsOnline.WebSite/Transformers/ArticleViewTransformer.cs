using System.Linq;
using CollectionsOnline.Core.Models;
using Raven.Client.Indexes;

namespace CollectionsOnline.WebSite.Transformers
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
                            relatedItem.DisplayTitle,
                            SubDisplayTitle = relatedItem.RegistrationNumber
                        },
                    RelatedSpecimens = from specimenId in article.RelatedSpecimenIds
                        let relatedSpecimen = LoadDocument<Specimen>(specimenId)
                        where relatedSpecimen != null && !relatedSpecimen.IsHidden
                        select new
                        {
                            relatedSpecimen.Id,
                            relatedSpecimen.ThumbnailUri,
                            relatedSpecimen.DisplayTitle,
                            SubDisplayTitle = relatedSpecimen.RegistrationNumber
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
}