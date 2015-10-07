using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Tests.Fakes;
using Nancy.Testing;
using Newtonsoft.Json;
using Raven.Tests.Helpers;
using Shouldly;
using Xunit;

namespace CollectionsOnline.Tests.Website.Modules
{
    public class ApiModuleBaseTests : RavenTestBase
    {
        [Fact]
        public void GivenEnvelopeRequest_GetItems_ReturnsEnvelope()
        {
            using (var documentStore = NewDocumentStore(seedData: new[] { FakeItems.CreateFakeItems(100) }))
            {
                // Given
                var browser = new Browser(new WebsiteBootstrapper(documentStore));

                // When
                var result = browser.Get(string.Format("/{0}{1}/items", Constants.ApiPathBase, Constants.CurrentApiVersionPath), with =>
                    {
                        with.HttpRequest();
                        with.Query("envelope", "true");
                    });

                // Then
                dynamic bodyResult = JsonConvert.DeserializeObject<ExpandoObject>(result.Body.AsString());
                ((object)bodyResult.response).ShouldNotBe(null);
            }
        }

        [Fact]
        public void GivenOnePageRequestAndDefaultPerPage_GetItems_ReturnsOnePage()
        {
            using (var documentStore = NewDocumentStore(seedData: new[] { FakeItems.CreateFakeItems(100) }))
            {
                // Given
                var browser = new Browser(new WebsiteBootstrapper(documentStore));

                // When
                var result = browser.Get(string.Format("/{0}{1}/items", Constants.ApiPathBase, Constants.CurrentApiVersionPath), with =>
                {
                    with.HttpRequest();
                    with.Query("page", "1");
                });

                // Then
                result.Body.DeserializeJson<IEnumerable<Item>>().Count().ShouldBe(Constants.PagingPerPageDefault);
            }
        }

        [Fact]
        public void GivenOnePageRequest_GetItems_ReturnsLinkHeader()
        {
            using (var documentStore = NewDocumentStore(seedData: new[] { FakeItems.CreateFakeItems(100) }))
            {
                // Given
                var browser = new Browser(new WebsiteBootstrapper(documentStore));

                // Given When
                var result = browser.Get(string.Format("/{0}{1}/items", Constants.ApiPathBase, Constants.CurrentApiVersionPath), with =>
                {
                    with.HttpRequest();
                    with.Query("page", "1");
                });

                // Then
                result.Headers["Link"].ShouldNotBe(string.Empty);
            }
        }

        [Fact]
        public void GivenPageRequest_GetItems_ReturnsCorrectItem()
        {
            using (var documentStore = NewDocumentStore(seedData: new[] { FakeItems.CreateFakeItems(100) }))
            {
                // Given
                var browser = new Browser(new WebsiteBootstrapper(documentStore));

                // Given When
                var result = browser.Get(string.Format("/{0}{1}/items", Constants.ApiPathBase, Constants.CurrentApiVersionPath), with =>
                {
                    with.HttpRequest();
                    with.Query("page", "2");
                });

                // Then
                result.Body.DeserializeJson<IEnumerable<Item>>().First().Id.ShouldBe("items/41");
            }
        }
    }
}