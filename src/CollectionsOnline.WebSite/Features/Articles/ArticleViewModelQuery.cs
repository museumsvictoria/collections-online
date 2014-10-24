using System.Linq;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using Raven.Client;
using Raven.Client.Linq;

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