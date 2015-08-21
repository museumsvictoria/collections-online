using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.WebSite.Transformers;
using Raven.Client;
using StackExchange.Profiling;

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
            using (_documentSession.Advanced.DocumentStore.AggressivelyCacheFor(Constants.AggressiveCacheTimeSpan))
            {
                ArticleViewTransformerResult result;
                using (MiniProfiler.Current.Step("Fetching ArticleViewTransformerResult"))
                {
                    result = _documentSession.Load<ArticleViewTransformer, ArticleViewTransformerResult>(articleId);
                }

                IDocumentQuery<CombinedIndexResult> query;
                using (MiniProfiler.Current.Step("Fetching RelatedItemSpecimenCount"))
                {
                    query = _documentSession.Advanced
                        .DocumentQuery<CombinedIndexResult, CombinedIndex>()
                        .WhereEquals("Article", result.Article.Title)
                        .Take(1);
                }

                result.RelatedItemSpecimenCount = query.QueryResult.TotalResults;

                return result;
            }
        }
    }
}