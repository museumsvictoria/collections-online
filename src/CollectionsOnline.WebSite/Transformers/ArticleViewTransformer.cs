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
                let parentArticle = LoadDocument<Article>(article.ParentArticleId)
                select new
                {
                    Article = article,
                    RelatedItems = article.RelatedItemIds
                        .Take(30)
                        .Select(LoadDocument<Item>)
                        .Where(item => item != null && !item.IsHidden)
                        .Select(item => new
                        {
                            item.Id,
                            item.DisplayTitle,
                            SubDisplayTitle = item.RegistrationNumber,
                            item.Summary,
                            item.ThumbnailUri,
                            RecordType = "Item"
                        }),
                    RelatedSpecimens = article.RelatedSpecimenIds
                        .Take(30)
                        .Select(LoadDocument<Specimen>)
                        .Where(specimen => specimen != null && !specimen.IsHidden)
                        .Select(specimen => new
                        {
                            specimen.Id,
                            specimen.DisplayTitle,
                            SubDisplayTitle = specimen.RegistrationNumber,
                            specimen.Summary,
                            specimen.ThumbnailUri,
                            RecordType = "Specimen"
                        }),
                    ParentArticle = parentArticle != null && !parentArticle.IsHidden
                        ? new
                        {
                            parentArticle.Id,
                            parentArticle.DisplayTitle,
                            parentArticle.Summary,
                            parentArticle.ThumbnailUri,
                            RecordType = "Article"
                        }
                        : null,
                    ChildArticles = article.ChildArticleIds
                        .Select(LoadDocument<Article>)
                        .Where(article => article != null && !article.IsHidden)
                        .Select(article => new
                        {
                            article.Id,
                            article.DisplayTitle,
                            article.Summary,
                            article.ThumbnailUri,
                            RecordType = "Article"
                        }),
                    RelatedArticles = article.RelatedArticleIds
                        .Select(LoadDocument<Article>)
                        .Where(article => article != null && !article.IsHidden)
                        .Select(article => new
                        {
                            article.Id,
                            article.DisplayTitle,
                            article.Summary,
                            article.ThumbnailUri,
                            RecordType = "Article"
                        }),
                    RelatedSpecies = article.RelatedSpeciesIds
                        .Select(LoadDocument<Species>)
                        .Where(article => article != null && !article.IsHidden)
                        .Select(article => new
                        {
                            article.Id,
                            article.DisplayTitle,
                            article.Summary,
                            article.ThumbnailUri,
                            RecordType = "Species"
                        })
                };
        }
    }
}