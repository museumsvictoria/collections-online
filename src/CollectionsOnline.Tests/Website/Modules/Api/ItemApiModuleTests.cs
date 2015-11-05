using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Tests.Fakes;
using Nancy;
using Nancy.Testing;
using Raven.Tests.Helpers;
using Shouldly;
using Xunit;

namespace CollectionsOnline.Tests.Website.Modules.Api
{
    public class ItemApiModuleTests : RavenTestBase
    {
        [Fact]
        public void GetItems_ReturnsItems()
        {
            using (var documentStore = NewDocumentStore(seedData: new[] { FakeItems.CreateFakeItems(5) }))
            {
                // Given
                var browser = new Browser(new WebsiteBootstrapper(documentStore));

                // When
                var result = browser.Get(string.Format("{0}/items", Constants.ApiPathBase), with => with.HttpRequest());

                // Then
                result.Body.DeserializeJson<IEnumerable<Item>>().Count().ShouldBe(5);
            }
        }

        [Fact]
        public void GivenAnInvalidId_GetItem_ReturnsNotFound()
        {
            using (var documentStore = NewDocumentStore(seedData: new[] { FakeItems.CreateFakeItems(5) }))
            {
                // Given
                var browser = new Browser(new WebsiteBootstrapper(documentStore));

                // When
                var result = browser.Get(string.Format("{0}/items/6", Constants.ApiPathBase), with => with.HttpRequest());

                // Then
                result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
            }
        }
    }
}