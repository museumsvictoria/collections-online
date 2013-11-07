using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebApi.Modules;
using Nancy;
using Nancy.Testing;
using Shouldly;
using WorldDomination.Raven.Tests.Helpers;
using Xunit;

namespace CollectionsOnline.Tests.WebApi.Modules
{
    public class SpeciesModuleTests : RavenDbTestBase
    {
        public SpeciesModuleTests()
        {
            DataToBeSeeded = new List<IEnumerable>
                {
                    FakeSpecies.CreateFakeSpecies(5)
                };

            Browser = new Browser(with =>
                {
                    with.Module<SpeciesModule>();
                    with.Dependency(DocumentSession);
                });
        }

        protected Browser Browser { get; set; }

        [Fact]
        public void GetSpecies_ReturnsSpecies()
        {
            var result = Browser.Get("/v1/species", with => with.HttpRequest());

            result.Body.DeserializeJson<IEnumerable<Species>>().Count().ShouldBe(5);
        }

        [Fact]
        public void GivenAnInvalidId_GetSpecies_ReturnsNotFound()
        {
            var result = Browser.Get("/v1/species/6", with => with.HttpRequest());

            result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }
    }
}