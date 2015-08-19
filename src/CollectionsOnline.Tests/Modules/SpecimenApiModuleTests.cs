using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Tests.Fakes;
using CollectionsOnline.WebSite.Infrastructure;
using CollectionsOnline.WebSite.Modules.Api;
using Nancy;
using Nancy.Testing;
using Shouldly;
using WorldDomination.Raven.Tests.Helpers;
using Xunit;

namespace CollectionsOnline.Tests.Modules
{
    public class SpecimenApiModuleTests : RavenDbTestBase
    {
        public SpecimenApiModuleTests()
        {
            DataToBeSeeded = new List<IEnumerable>
                {
                    FakeSpecimens.CreateFakeSpecimens(5)
                };

            IndexesToExecute = new List<Type>
                {
                    typeof(CombinedIndex)
                };

            Browser = new Browser(with =>
                {
                    with.Module<SpecimensApiModule>();
                    with.Dependency(DocumentStore.OpenSession());
                    with.ApplicationStartup((container, pipelines) => AutomapperConfig.Initialize());
                });
        }

        protected Browser Browser { get; set; }

        [Fact]
        public void GetSpecimens_ReturnsSpecimens()
        {
            // Given When
            var result = Browser.Get(string.Format("/{0}{1}/specimens", Constants.ApiBasePath, Constants.CurrentApiVersionPath), with => with.HttpRequest());

            // Then
            result.Body.DeserializeJson<IEnumerable<Specimen>>().Count().ShouldBe(5);
        }

        [Fact]
        public void GivenAnInvalidId_GetSpecies_ReturnsNotFound()
        {
            // Given When
            var result = Browser.Get(string.Format("/{0}{1}/specimens/6", Constants.ApiBasePath, Constants.CurrentApiVersionPath), with => with.HttpRequest());

            // Then
            result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }
    }
}