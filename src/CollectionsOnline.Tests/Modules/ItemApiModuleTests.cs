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
    public class ItemApiModuleTests : RavenDbTestBase
    {
        public ItemApiModuleTests()
        {
            DataToBeSeeded = new List<IEnumerable>
                {
                    FakeItems.CreateFakeItems(5)
                };

            IndexesToExecute = new List<Type>
                {
                    typeof(Combined)
                };

            Browser = new Browser(with =>
                {
                    with.Module<ItemsApiModule>();
                    with.Dependency(DocumentStore.OpenSession());
                });
        }

        protected Browser Browser { get; set; }

        [Fact]
        public void GetItems_ReturnsItems()
        {
            // Given When
            var result = Browser.Get("/api/v1/items", with => with.HttpRequest());

            // Then
            result.Body.DeserializeJson<IEnumerable<Item>>().Count().ShouldBe(5);
        }

        [Fact]
        public void GivenAnInvalidId_GetItem_ReturnsNotFound()
        {
            // Given When
            var result = Browser.Get("/api/v1/items/6", with => with.HttpRequest());

            // Then
            result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }
    }
}