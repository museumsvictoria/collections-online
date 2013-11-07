using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebApi.Modules;
using Nancy.Testing;
using Newtonsoft.Json;
using Shouldly;
using WorldDomination.Raven.Tests.Helpers;
using Xunit;

namespace CollectionsOnline.Tests.WebApi.Modules
{
    public class BaseModuleTests : RavenDbTestBase
    {
        public BaseModuleTests()
        {
            DataToBeSeeded = new List<IEnumerable>
                {
                    FakeItems.CreateFakeItems(15)
                };

            Browser = new Browser(with =>
                {
                    with.Module<ItemModule>();
                    with.Dependency(DocumentSession);
                });
        }

        protected Browser Browser { get; set; }

        [Fact]
        public void GivenEnvelopeRequest_GetItems_ReturnsEnvelope()
        {
            var result = Browser.Get("/v1/items", with =>
                {
                    with.HttpRequest();
                    with.Query("envelope", "true");
                });

            dynamic bodyResult = JsonConvert.DeserializeObject<ExpandoObject>(result.Body.AsString());

            ((object)bodyResult.Response).ShouldNotBe(null);
        }

        [Fact]
        public void GivenOneLimitRequest_GetItems_ReturnsOneItem()
        {
            var result = Browser.Get("/v1/items", with =>
            {
                with.HttpRequest();
                with.Query("limit", "1");
            });

            result.Body.DeserializeJson<IEnumerable<Item>>().Count().ShouldBe(1);
        }

        [Fact]
        public void GivenOneLimitRequest_GetItems_ReturnsLinkHeader()
        {
            var result = Browser.Get("/v1/items", with =>
            {
                with.HttpRequest();
                with.Query("limit", "1");
            });

            result.Headers["Link"].ShouldNotBe(string.Empty);
        }

        [Fact]
        public void GivenThirtyLimitRequest_GetItems_DoesNotReturnLinkHeader()
        {
            var result = Browser.Get("/v1/items", with =>
            {
                with.HttpRequest();
                with.Query("limit", "30");
            });

            result.Headers["Link"].ShouldBe(string.Empty);
        }

        [Fact]
        public void GivenOffsetRequest_GetItems_ReturnsCorrectItem()
        {
            var result = Browser.Get("/v1/items", with =>
            {
                with.HttpRequest();
                with.Query("limit", "10");
                with.Query("offset", "10");
            });

            result.Body.DeserializeJson<IEnumerable<Item>>().First().Id.ShouldBe("items/11");
        }
    }
}