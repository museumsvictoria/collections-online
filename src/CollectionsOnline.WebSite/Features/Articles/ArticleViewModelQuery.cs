using CollectionsOnline.Core.Indexes;
using Raven.Client;

namespace CollectionsOnline.WebSite.Features.Articles
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
                .LuceneQuery<CombinedResult, Combined>()
                .WhereEquals("Articles", result.Article.Title);

            result.RelatedItemSpecimenCount = query.QueryResult.TotalResults;

            return result;
        }
    }
}