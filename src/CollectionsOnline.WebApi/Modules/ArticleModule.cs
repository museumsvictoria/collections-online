using System.Linq;
using AutoMapper;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebApi.Models;
using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebApi.Modules
{
    public class ArticleModule : WebApiModule
    {
        public ArticleModule(IDocumentSession documentSession)
            : base("/articles")
        {
            Get["articles-index", "/"] = parameters =>
                {
                    var articles = documentSession.Advanced
                        .DocumentQuery<Article, Combined>()
                        .WhereEquals("Type", "Article")
                        .Statistics(out Statistics)
                        .Skip(Offset)
                        .Take(Limit)
                        .ToList();

                    return BuildResponse(articles);
                };

            Get["articles-by-id", "/{id}"] = parameters =>
                {
                    var article = documentSession.Load<Article>("articles/" + parameters.id as string);

                    return (article == null || article.IsHidden) ? BuildErrorResponse(HttpStatusCode.NotFound, "Article {0} not found", parameters.id) : BuildResponse(Mapper.Map<Article, ArticleViewModel>(article));
                };
        }
    }
}