using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Tests.Fakes;
using CollectionsOnline.WebApi.Modules;
using Nancy;
using Nancy.Testing;
using Shouldly;
using WorldDomination.Raven.Tests.Helpers;
using Xunit;

namespace CollectionsOnline.Tests.WebApi.Modules
{
    public class ItemModuleTests : RavenDbTestBase
    {
        public ItemModuleTests()
        {
            DataToBeSeeded = new List<IEnumerable>
                {
                    FakeItems.CreateFakeItems(5)
                };

            Browser = new Browser(with =>
                {
                    with.Module<ItemModule>();
                    with.Dependency(DocumentSession);
                });
        }

        protected Browser Browser { get; set; }

        [Fact]
        public void GetItems_ReturnsItems()
        {
            // Given When
            var result = Browser.Get("/v1/items", with => with.HttpRequest());

            // Then
            result.Body.DeserializeJson<IEnumerable<Item>>().Count().ShouldBe(5);
        }

        [Fact]
        public void GivenAnInvalidId_GetItem_ReturnsNotFound()
        {
            // Given When
            var result = Browser.Get("/v1/items/6", with => with.HttpRequest());

            // Then
            result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }
    }
}