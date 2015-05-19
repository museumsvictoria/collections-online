using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Queries;
using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebSite.Modules
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

                return (article == null || article.IsHidden) ? HttpStatusCode.NotFound : View["Articles", articleViewModelQuery.BuildArticle("articles/" + parameters.id)];
            };
        }
    }
}