using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using AutoMapper;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models.Api;
using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebSite.Modules.Api
{
    public class ArticlesApiModule : ApiModuleBase
    {
        public ArticlesApiModule(IDocumentSession documentSession)
            : base("/articles")
        {
            Get["articles-api-index", "/"] = parameters =>
                {
                    var articles = documentSession.Advanced
                        .DocumentQuery<Article, CombinedIndex>()
                        .WhereEquals("RecordType", "Article")
                        .Statistics(out Statistics)
                        .Skip((Page - 1) * PerPage)
                        .Take(PerPage)
                        .ToList();

                    return BuildResponse(Mapper.Map<IEnumerable<Article>, IEnumerable<ArticleApiViewModel>>(articles));
                };

            Get["articles-api-by-id", "/{id}"] = parameters =>
                {
                    var article = documentSession.Load<Article>("articles/" + parameters.id as string);

                    return (article == null || article.IsHidden) ? HttpStatusCode.NotFound : BuildResponse(Mapper.Map<Article, ArticleApiViewModel>(article));
                };
        }
    }
}