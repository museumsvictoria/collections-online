using CollectionsOnline.Core.Models;
using Raven.Client;

namespace CollectionsOnline.WebSite.Features.Articles
{
    public class ArticleViewModelQuery : IArticleViewModelQuery
    {
        private readonly IDocumentSession _documentSession;
        private readonly IArticleViewModelFactory _articleViewModelFactory;

        public ArticleViewModelQuery(
            IDocumentSession documentSession,
            IArticleViewModelFactory articleViewModelFactory)
        {
            _documentSession = documentSession;
            _articleViewModelFactory = articleViewModelFactory;
        }

        public ArticleViewModel BuildArticle(string articleId)
        {
            var article = _documentSession
                .Load<Article>(articleId);

            return _articleViewModelFactory.MakeViewModel(article);
        }
    }
}