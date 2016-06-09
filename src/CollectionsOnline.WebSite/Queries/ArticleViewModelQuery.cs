﻿using System.Linq;
using AutoMapper;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models.Api;
using CollectionsOnline.WebSite.Transformers;
using Raven.Client;
using StackExchange.Profiling;

namespace CollectionsOnline.WebSite.Queries
{
    public class ArticleViewModelQuery : IArticleViewModelQuery
    {
        private readonly IDocumentSession _documentSession;

        public ArticleViewModelQuery(
            IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public ArticleViewTransformerResult BuildArticle(string articleId)
        {
            using (MiniProfiler.Current.Step("Build Article view model"))
            using (_documentSession.Advanced.DocumentStore.AggressivelyCacheFor(Constants.AggressiveCacheTimeSpan))
            {
                ArticleViewTransformerResult result;
                using (MiniProfiler.Current.Step("Fetching ArticleViewTransformerResult"))
                {
                    result = _documentSession.Load<ArticleViewTransformer, ArticleViewTransformerResult>(articleId);
                }

                IDocumentQuery<CombinedIndexResult> query;
                using (MiniProfiler.Current.Step("Fetching RelatedItemSpecimenCount"))
                {
                    query = _documentSession.Advanced
                        .DocumentQuery<CombinedIndexResult, CombinedIndex>()
                        .WhereEquals("Article", result.Article.Title)
                        .Take(1);
                }

                result.RelatedItemSpecimenCount = query.QueryResult.TotalResults;

                return result;
            }
        }

        public ApiViewModel BuildArticleApiIndex(ApiInputModel apiInputModel)
        {
            using (MiniProfiler.Current.Step("Build Article Api Index view model"))
            using (_documentSession.Advanced.DocumentStore.AggressivelyCacheFor(Constants.AggressiveCacheTimeSpan))
            {
                RavenQueryStatistics statistics;
                var results = _documentSession.Advanced
                    .DocumentQuery<dynamic, CombinedIndex>()
                    .WhereEquals("RecordType", "Article")
                    .Statistics(out statistics)
                    .Skip((apiInputModel.Page - 1) * apiInputModel.PerPage)
                    .Take(apiInputModel.PerPage);

                return new ApiViewModel
                {
                    Results = results.Cast<Article>().Select<Article, dynamic>(Mapper.Map<Article, ArticleApiViewModel>).ToList(),
                    ApiPageInfo = new ApiPageInfo(statistics.TotalResults, apiInputModel.PerPage)
                };
            }
        }
    }
}