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
            Mapper.CreateMap<Article, ArticleViewModel>();

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

            Get["articles-by-id", "/{articleId}"] = parameters =>
                {
                    string articleId = parameters.articleId;
                    var article = documentSession
                        .Load<Article>("articles/" + articleId);

                    return (article == null || article.IsHidden) ? BuildErrorResponse(HttpStatusCode.NotFound, "Article {0} not found", articleId) : BuildResponse(Mapper.Map<Article, ArticleViewModel>(article));
                };
        }
    }
}