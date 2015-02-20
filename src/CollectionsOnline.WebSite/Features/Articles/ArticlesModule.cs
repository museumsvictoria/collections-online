using CollectionsOnline.Core.Models;
using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebSite.Features.Articles
{
    public class ArticlesModule : NancyModule
    {
        public ArticlesModule(
            IArticleViewModelQuery articleViewModelQuery,
            IDocumentSession documentSession)            
        {
            Get["/articles/{id}"] = parameters =>
            {
                var article = documentSession.Load<Article>("articles/" + parameters.id as string);

                return (article == null || article.IsHidden) ? HttpStatusCode.NotFound : View["articles", articleViewModelQuery.BuildArticle("articles/" + parameters.id)];
            };
        }
    }
}