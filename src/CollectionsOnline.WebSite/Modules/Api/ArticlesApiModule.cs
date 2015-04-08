using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models.Api;
using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebSite.Modules.Api
{
    public class ArticlesApiModule : BaseApiModule
    {
        public ArticlesApiModule(IDocumentSession documentSession)
            : base("/articles")
        {
            Get["articles-api-index", "/"] = parameters =>
                {
                    var articles = documentSession.Advanced
                        .DocumentQuery<Article, CombinedIndex>()
                        .WhereEquals("Type", "Article")
                        .Statistics(out Statistics)
                        .Skip((Page - 1) * PerPage)
                        .Take(PerPage)
                        .ToList();

                    return BuildResponse(Mapper.Map<IEnumerable<Article>, IEnumerable<ArticleApiViewModel>>(articles));
                };

            Get["articles-api-by-id", "/{id}"] = parameters =>
                {
                    var article = documentSession.Load<Article>("articles/" + parameters.id as string);

                    return (article == null || article.IsHidden) ? BuildErrorResponse(HttpStatusCode.NotFound, "Article {0} not found", parameters.id) : BuildResponse(Mapper.Map<Article, ArticleApiViewModel>(article));
                };
        }
    }
}