using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebApi;
using CollectionsOnline.WebApi.Modules;
using Nancy;
using Nancy.Json;
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
            var result = Browser.Get("/v1/items", with =>
                {
                    with.HttpRequest();
                });

            result.StatusCode.ShouldBe(HttpStatusCode.OK);
            result.Body.DeserializeJson<IEnumerable<Item>>().Count().ShouldBe(5);
        }
    }
}