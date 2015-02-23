using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Tests.Fakes;
using CollectionsOnline.WebSite.Modules;
using Nancy.Testing;
using Newtonsoft.Json;
using Shouldly;
using WorldDomination.Raven.Tests.Helpers;
using Xunit;

namespace CollectionsOnline.Tests.Modules
{
    public class BaseModuleTests : RavenDbTestBase
    {
        public BaseModuleTests()
        {
            DataToBeSeeded = new List<IEnumerable>
                {
                    FakeItems.CreateFakeItems(15)
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
        public void GivenEnvelopeRequest_GetItems_ReturnsEnvelope()
        {
            // Given When
            var result = Browser.Get("/api/v1/items", with =>
                {
                    with.HttpRequest();
                    with.Query("envelope", "true");
                });

            // Then
            dynamic bodyResult = JsonConvert.DeserializeObject<ExpandoObject>(result.Body.AsString());
            ((object)bodyResult.response).ShouldNotBe(null);
        }

        [Fact]
        public void GivenOneLimitRequest_GetItems_ReturnsOneItem()
        {
            // Given When
            var result = Browser.Get("/api/v1/items", with =>
            {
                with.HttpRequest();
                with.Query("limit", "1");
            });

            // Then
            result.Body.DeserializeJson<IEnumerable<Item>>().Count().ShouldBe(1);
        }

        [Fact]
        public void GivenOneLimitRequest_GetItems_ReturnsLinkHeader()
        {
            // Given When
            var result = Browser.Get("/api/v1/items", with =>
            {
                with.HttpRequest();
                with.Query("limit", "1");
            });

            // Then
            result.Headers["Link"].ShouldNotBe(string.Empty);
        }

        [Fact]
        public void GivenThirtyLimitRequest_GetItems_DoesNotReturnLinkHeader()
        {
            // Given When
            var result = Browser.Get("/api/v1/items", with =>
            {
                with.HttpRequest();
                with.Query("limit", "30");
            });

            // Then
            result.Headers["Link"].ShouldBe(string.Empty);
        }

        [Fact]
        public void GivenOffsetRequest_GetItems_ReturnsCorrectItem()
        {
            // Given When
            var result = Browser.Get("/api/v1/items", with =>
            {
                with.HttpRequest();
                with.Query("limit", "10");
                with.Query("offset", "10");
            });

            // Then
            result.Body.DeserializeJson<IEnumerable<Item>>().First().Id.ShouldBe("items/11");
        }
    }
}