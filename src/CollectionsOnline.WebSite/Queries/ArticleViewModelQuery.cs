using System.Linq;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
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

            // Set Media
            result.ArticleImages = result.Article.Media.Where(x => x is ImageMedia).Cast<ImageMedia>().ToList();
            result.JsonArticleImages = JsonConvert.SerializeObject(result.ArticleImages);

            return result;
        }
    }
}