//using System.Collections.Generic;
//using System.Linq;
//using CollectionsOnline.Core.Config;
//using CollectionsOnline.Core.Indexes;
//using CollectionsOnline.Core.Models;
//using CollectionsOnline.Tests.Fakes;
//using CollectionsOnline.WebSite.Infrastructure;
//using CollectionsOnline.WebSite.Modules.Api;
//using Nancy;
//using Nancy.Testing;
//using Raven.Tests.Helpers;
//using Shouldly;
//using Xunit;

//namespace CollectionsOnline.Tests.Website.Modules
//{
//    public class ArticleApiModuleTests : RavenTestBase
//    {
//        [Fact]
//        public void GetArticles_ReturnsArticles()
//        {
//            using (var documentStore = NewDocumentStore(seedData: new[] { FakeArticles.CreateFakeArticles(5) }, indexes: new[] { new CombinedIndex() }))
//            {
//                // Given
//                var browser = new Browser(with =>
//                    {
//                        with.Module<ArticlesApiModule>();
//                        with.Dependency(documentStore.OpenSession());
//                        with.ApplicationStartup((container, pipelines) => AutomapperConfig.Initialize());
//                    });

//                // When
//                var result = browser.Get(string.Format("/{0}{1}/articles", Constants.ApiPathBase, Constants.CurrentApiVersionPath), with => with.HttpRequest());

//                // Then
//                result.Body.DeserializeJson<IEnumerable<Article>>().Count().ShouldBe(5);
//            }
//        }

//        [Fact]
//        public void GivenAnInvalidId_GetArticles_ReturnsNotFound()
//        {
//            using (var documentStore = NewDocumentStore(seedData: new[] {FakeArticles.CreateFakeArticles(5)}, indexes: new[] {new CombinedIndex()}))
//            {
//                // Given
//                var browser = new Browser(with =>
//                {
//                    with.Module<ArticlesApiModule>();
//                    with.Dependency(documentStore.OpenSession());
//                    with.ApplicationStartup((container, pipelines) => AutomapperConfig.Initialize());
//                });

//                // When
//                var result = browser.Get(string.Format("/{0}{1}/articles/6", Constants.ApiPathBase, Constants.CurrentApiVersionPath), with => with.HttpRequest());

//                // Then
//                result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
//            }
//        }
//    }
//}