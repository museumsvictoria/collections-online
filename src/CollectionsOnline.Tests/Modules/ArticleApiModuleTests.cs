using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Tests.Fakes;
using CollectionsOnline.WebSite.Modules;
using Nancy;
using Nancy.Testing;
using Shouldly;
using WorldDomination.Raven.Tests.Helpers;
using Xunit;

namespace CollectionsOnline.Tests.Modules
{
    public class ArticleApiModuleTests : RavenDbTestBase
    {
        public ArticleApiModuleTests()
        {
            DataToBeSeeded = new List<IEnumerable>
                {
                    FakeArticles.CreateFakeArticles(5)
                };

            IndexesToExecute = new List<Type>
                {
                    typeof(Combined)
                };

            Browser = new Browser(with =>
                {
                    with.Module<ArticlesApiModule>();
                    with.Dependency(DocumentStore.OpenSession());
                });
        }

        protected Browser Browser { get; set; }

        [Fact]
        public void GetArticles_ReturnsArticles()
        {
            // Given When
            var result = Browser.Get("/api/v1/articles", with => with.HttpRequest());

            // Then
            result.Body.DeserializeJson<IEnumerable<Article>>().Count().ShouldBe(5);
        }

        [Fact]
        public void GivenAnInvalidId_GetArticles_ReturnsNotFound()
        {
            // Given When
            var result = Browser.Get("/api/v1/articles/6", with => with.HttpRequest());

            // Then
            result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }
    }
}