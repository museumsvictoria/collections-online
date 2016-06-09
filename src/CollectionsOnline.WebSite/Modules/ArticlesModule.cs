using System;
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
            Get["/articles/{id}"] = LoadArticle(articleViewModelQuery, documentSession, metadataViewModelFactory);

            Get["/articles/{id}/media/{mediaId}/{size}"] = parameters => mediaResponseQuery.BuildMediaResponse("articles/" + parameters.id, parameters.mediaId, parameters.size);

            Get["/aquarium"] = LoadArticle(articleViewModelQuery, documentSession, metadataViewModelFactory, "articles/15019");
        }

        private Func<dynamic, dynamic> LoadArticle(IArticleViewModelQuery articleViewModelQuery, IDocumentSession documentSession, IMetadataViewModelFactory metadataViewModelFactory, string articleId = null)
        {
            return parameters =>
            {
                var article = documentSession.Load<Article>(articleId ?? "articles/" + parameters.id as string);

                if (article == null || article.IsHidden)
                    return HttpStatusCode.NotFound;

                ViewBag.metadata = metadataViewModelFactory.Make(article);

                return View["Articles", articleViewModelQuery.BuildArticle(articleId ?? "articles/" + parameters.id)];
            };
        }
    }
}