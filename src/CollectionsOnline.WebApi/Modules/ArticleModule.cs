using System.Linq;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebApi.Modules
{
    public class ArticleModule : BaseModule
    {
        public ArticleModule(IDocumentSession documentSession)
            : base("/articles")
        {
            Get["/"] = parameters =>
                {
                    var articles = documentSession.Advanced
                        .LuceneQuery<Article, Combined>()
                        .WhereEquals("Type", "Article")
                        .Statistics(out Statistics)
                        .Skip(Offset)
                        .Take(Limit)
                        .ToList();

                    return BuildResponse(articles);
                };

            Get["/{articleId}"] = parameters =>
                {
                    string articleId = parameters.articleId;
                    var article = documentSession
                        .Load<Article>("articles/" + articleId);

                    return article == null ? BuildErrorResponse(HttpStatusCode.NotFound, "Article {0} not found", articleId) : BuildResponse(article);
                };
        }
    }
}