using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Indexes;
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
    public class SpeciesModuleTests : RavenDbTestBase
    {
        public SpeciesModuleTests()
        {
            DataToBeSeeded = new List<IEnumerable>
                {
                    FakeSpecies.CreateFakeSpecies(5)
                };

            IndexesToExecute = new List<Type>
                {
                    typeof(Combined)
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
            // Given When
            var result = Browser.Get("/v1/species", with => with.HttpRequest());

            // Then
            result.Body.DeserializeJson<IEnumerable<Species>>().Count().ShouldBe(5);
        }

        [Fact]
        public void GivenAnInvalidId_GetSpecies_ReturnsNotFound()
        {
            // Given When
            var result = Browser.Get("/v1/species/6", with => with.HttpRequest());

            // Then
            result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }
    }
}