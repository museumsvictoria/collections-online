using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Tests.Fakes;
using CollectionsOnline.WebSite.Infrastructure;
using CollectionsOnline.WebSite.Modules;
using CollectionsOnline.WebSite.Modules.Api;
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
                    typeof(CombinedIndex)
                };

            Browser = new Browser(with =>
                {
                    with.Module<ItemsApiModule>();
                    with.Dependency(DocumentStore.OpenSession());
                    with.ApplicationStartup((container, pipelines) => AutomapperConfig.Initialize());
                });
        }

        protected Browser Browser { get; set; }

        [Fact]
        public void GetItems_ReturnsItems()
        {
            // Given When
            var result = Browser.Get(string.Format("/{0}{1}/items", Constants.ApiBasePath, Constants.CurrentApiVersionPath), with => with.HttpRequest());

            // Then
            result.Body.DeserializeJson<IEnumerable<Item>>().Count().ShouldBe(5);
        }

        [Fact]
        public void GivenAnInvalidId_GetItem_ReturnsNotFound()
        {
            // Given When
            var result = Browser.Get(string.Format("/{0}{1}/items/6", Constants.ApiBasePath, Constants.CurrentApiVersionPath), with => with.HttpRequest());

            // Then
            result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }
    }
}