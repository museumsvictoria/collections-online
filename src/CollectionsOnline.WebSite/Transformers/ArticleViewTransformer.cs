using System.Linq;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models;
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
                            relatedItem.DisplayTitle,
                            SubDisplayTitle = relatedItem.RegistrationNumber,
                            relatedItem.Summary,
                            relatedItem.ThumbnailUri,                            
                            Type = "Item"
                        },
                    RelatedSpecimens = from specimenId in article.RelatedSpecimenIds
                        let relatedSpecimen = LoadDocument<Specimen>(specimenId)
                        where relatedSpecimen != null && !relatedSpecimen.IsHidden
                        select new
                        {
                            relatedSpecimen.Id,                            
                            relatedSpecimen.DisplayTitle,
                            SubDisplayTitle = relatedSpecimen.RegistrationNumber,
                            relatedSpecimen.Summary,
                            relatedSpecimen.ThumbnailUri,
                            Type = "Specimen"
                        },
                    ParentArticle = (LoadDocument<Article>(article.ParentArticleId) != null && !LoadDocument<Article>(article.ParentArticleId).IsHidden) ?
                        new
                        {
                            LoadDocument<Article>(article.ParentArticleId).Id,
                            LoadDocument<Article>(article.ParentArticleId).DisplayTitle,
                            LoadDocument<Article>(article.ParentArticleId).Summary,
                            LoadDocument<Article>(article.ParentArticleId).ThumbnailUri,
                            Type = "Article"
                        } : null,
                    ChildArticles = from articleId in article.ChildArticleIds
                        let relatedArticle = LoadDocument<Article>(articleId)
                        where relatedArticle != null && !relatedArticle.IsHidden
                        select new
                        {
                            relatedArticle.Id,
                            relatedArticle.DisplayTitle,
                            relatedArticle.Summary,
                            relatedArticle.ThumbnailUri,
                            Type = "Article"
                        },
                    RelatedArticles = from articleId in article.RelatedArticleIds
                        let relatedArticle = LoadDocument<Article>(articleId)
                        where relatedArticle != null && !relatedArticle.IsHidden
                        select new
                        {
                            relatedArticle.Id,
                            relatedArticle.DisplayTitle,
                            relatedArticle.Summary,
                            relatedArticle.ThumbnailUri,
                            Type = "Article"
                        }
                };
        }
    }
}