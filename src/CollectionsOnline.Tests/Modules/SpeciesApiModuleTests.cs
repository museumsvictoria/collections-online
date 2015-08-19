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
    public class SpeciesApiModuleTests : RavenDbTestBase
    {
        public SpeciesApiModuleTests()
        {
            DataToBeSeeded = new List<IEnumerable>
                {
                    FakeSpecies.CreateFakeSpecies(5)
                };

            IndexesToExecute = new List<Type>
                {
                    typeof(CombinedIndex)
                };

            Browser = new Browser(with =>
                {
                    with.Module<SpeciesApiModule>();
                    with.Dependency(DocumentStore.OpenSession());
                    with.ApplicationStartup((container, pipelines) => AutomapperConfig.Initialize());
                });
        }

        protected Browser Browser { get; set; }

        [Fact]
        public void GetSpecies_ReturnsSpecies()
        {
            // Given When
            var result = Browser.Get(string.Format("/{0}{1}/species", Constants.ApiBasePath, Constants.CurrentApiVersionPath), with => with.HttpRequest());

            // Then
            result.Body.DeserializeJson<IEnumerable<Species>>().Count().ShouldBe(5);
        }

        [Fact]
        public void GivenAnInvalidId_GetSpecies_ReturnsNotFound()
        {
            // Given When
            var result = Browser.Get(string.Format("/{0}{1}/species/6", Constants.ApiBasePath, Constants.CurrentApiVersionPath), with => with.HttpRequest());

            // Then
            result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }
    }
}