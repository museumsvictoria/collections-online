using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Tests.Fakes;
using CollectionsOnline.WebSite.Infrastructure;
using CollectionsOnline.WebSite.Modules.Api;
using Nancy.Json;
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
                    FakeItems.CreateFakeItems(100)
                };

            IndexesToExecute = new List<Type>
                {
                    typeof(CombinedIndex)
                };

            Browser = new Browser(with =>
                {
                    with.Module<ItemsApiModule>();
                    with.Dependency(DocumentStore.OpenSession());
                    with.ApplicationStartup((container, pipelines) =>
                    {
                        JsonSettings.MaxJsonLength = Int32.MaxValue;
                        AutomapperConfig.Initialize();
                    });
                });
        }

        protected Browser Browser { get; set; }

        [Fact]
        public void GivenEnvelopeRequest_GetItems_ReturnsEnvelope()
        {
            // Given When
            var result = Browser.Get(string.Format("{0}{1}/items", Constants.ApiBasePath, Constants.CurrentApiVersionPath), with =>
                {
                    with.HttpRequest();
                    with.Query("envelope", "true");
                });

            // Then
            dynamic bodyResult = JsonConvert.DeserializeObject<ExpandoObject>(result.Body.AsString());
            ((object)bodyResult.response).ShouldNotBe(null);
        }

        [Fact]
        public void GivenOnePageRequestAndDefaultPerPage_GetItems_ReturnsOnePage()
        {
            // Given When
            var result = Browser.Get(string.Format("{0}{1}/items", Constants.ApiBasePath, Constants.CurrentApiVersionPath), with =>
            {
                with.HttpRequest();
                with.Query("page", "1");
            });

            var fdasfa = result.Body.DeserializeJson<IEnumerable<Item>>();

            // Then
            result.Body.DeserializeJson<IEnumerable<Item>>().Count().ShouldBe(Constants.PagingPerPageDefault);
        }

        [Fact]
        public void GivenOnePageRequest_GetItems_ReturnsLinkHeader()
        {
            // Given When
            var result = Browser.Get(string.Format("{0}{1}/items", Constants.ApiBasePath, Constants.CurrentApiVersionPath), with =>
            {
                with.HttpRequest();
                with.Query("page", "1");
            });

            // Then
            result.Headers["Link"].ShouldNotBe(string.Empty);
        }

        [Fact]
        public void GivenPageRequest_GetItems_ReturnsCorrectItem()
        {
            // Given When
            var result = Browser.Get(string.Format("{0}{1}/items", Constants.ApiBasePath, Constants.CurrentApiVersionPath), with =>
            {
                with.HttpRequest();
                with.Query("page", "2");
            });

            // Then
            result.Body.DeserializeJson<IEnumerable<Item>>().First().Id.ShouldBe("items/41");
        }
    }
}