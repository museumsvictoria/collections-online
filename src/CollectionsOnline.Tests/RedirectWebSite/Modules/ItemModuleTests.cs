using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Models;
using CollectionsOnline.RedirectWebSite.Modules;
using CollectionsOnline.Tests.Fakes;
using CollectionsOnline.WebApi.Modules;
using Nancy;
using Nancy.Testing;
using Shouldly;
using WorldDomination.Raven.Tests.Helpers;
using Xunit;

namespace CollectionsOnline.Tests.RedirectWebSite.Modules
{
    public class RedirectModuleTests : RavenDbTestBase
    {
        public RedirectModuleTests()
        {
            Browser = new Browser(with => with.Module<RedirectModule>());
        }

        protected Browser Browser { get; set; }

        [Fact]
        public void GetIndex_ReturnsMovedPermanently()
        {
            // Given When
            var result = Browser.Get("/", with => with.HttpRequest());

            // Then
            result.StatusCode.ShouldBe(HttpStatusCode.MovedPermanently);
        }
    }
}