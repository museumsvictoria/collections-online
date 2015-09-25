using AutoMapper;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models.Api;
using CollectionsOnline.WebSite.Queries;
using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebSite.Modules.Api
{
    public class ArticlesApiModule : ApiModuleBase
    {
        public ArticlesApiModule(
            IDocumentSession documentSession,
            IArticleViewModelQuery articleViewModelQuery)
            : base("/articles")
        {
            Get["articles-api-index", "/"] = parameters =>
            {
                var apiViewModel = articleViewModelQuery.BuildArticleApiIndex(ApiInputModel);

                return BuildResponse(apiViewModel.Results, apiPageInfo: apiViewModel.ApiPageInfo);
            };

            Get["articles-api-by-id", "/{id}"] = parameters =>
                {
                    var article = documentSession.Load<Article>("articles/" + parameters.id as string);

                    return (article == null || article.IsHidden) ? HttpStatusCode.NotFound : BuildResponse(Mapper.Map<Article, ArticleApiViewModel>(article));
                };
        }
    }
}