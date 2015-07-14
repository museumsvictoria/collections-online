using CollectionsOnline.Core.Indexes;
using CollectionsOnline.WebSite.Extensions;
using CollectionsOnline.WebSite.Transformers;
using Newtonsoft.Json;
using Raven.Client;

namespace CollectionsOnline.WebSite.Queries
{
    public class ArticleViewModelQuery : IArticleViewModelQuery
    {
        private readonly IDocumentSession _documentSession;

        public ArticleViewModelQuery(
            IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public ArticleViewTransformerResult BuildArticle(string articleId)
        {
            var result = _documentSession.Load<ArticleViewTransformer, ArticleViewTransformerResult>(articleId);

            var query = _documentSession.Advanced
                .DocumentQuery<CombinedIndexResult, CombinedIndex>()
                .WhereEquals("Article", result.Article.Title)
                .Take(1);

            result.RelatedItemSpecimenCount = query.QueryResult.TotalResults;

            // Create model for use in javascript
            result.JsonArticleMultimedia = JsonConvert.SerializeObject(result.Article.Media.GetMultimedia(), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });

            return result;
        }
    }
}