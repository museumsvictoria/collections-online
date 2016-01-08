using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Factories;
using CollectionsOnline.WebSite.Queries;
using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebSite.Modules
{
    public class ArticlesModule : NancyModule
    {
        public ArticlesModule(
            IArticleViewModelQuery articleViewModelQuery,
            IDocumentSession documentSession,
            IMetadataViewModelFactory metadataViewModelFactory,
            IMediaResponseQuery mediaResponseQuery)
        {
            Get["/articles/{id}"] = parameters =>
            {
                var article = documentSession.Load<Article>("articles/" + parameters.id as string);

                if (article == null || article.IsHidden) 
                    return HttpStatusCode.NotFound;                
                
                ViewBag.metadata = metadataViewModelFactory.Make(article);

                return View["Articles", articleViewModelQuery.BuildArticle("articles/" + parameters.id)];
            };

            Get["/articles/{id}/media/{mediaId}/{size}"] = parameters => mediaResponseQuery.BuildMediaResponse("articles/" + parameters.id, parameters.mediaId, parameters.size);
        }
    }
}