using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Tests.Fakes;
using Nancy;
using Nancy.Testing;
using Raven.Tests.Helpers;
using Shouldly;
using Xunit;

namespace CollectionsOnline.Tests.Website.Modules.Api
{
    public class SpecimenApiModuleTests : RavenTestBase
    {
        [Fact]
        public void GetSpecimens_ReturnsSpecimens()
        {
            using (var documentStore = NewDocumentStore(seedData: new[] { FakeSpecimens.CreateFakeSpecimens(5) }))
            {
                // Given
                var browser = new Browser(new WebsiteBootstrapper(documentStore));

                // When
                var result = browser.Get(string.Format("/{0}{1}/specimens", Constants.ApiPathBase, Constants.CurrentApiVersionPath), with => with.HttpRequest());

                // Then
                result.Body.DeserializeJson<IEnumerable<Specimen>>().Count().ShouldBe(5);
            }
        }

        [Fact]
        public void GivenAnInvalidId_GetSpecies_ReturnsNotFound()
        {
            using (var documentStore = NewDocumentStore(seedData: new[] { FakeSpecimens.CreateFakeSpecimens(5) }))
            {
                // Given
                var browser = new Browser(new WebsiteBootstrapper(documentStore));

                // When
                var result = browser.Get(string.Format("/{0}{1}/specimens/6", Constants.ApiPathBase, Constants.CurrentApiVersionPath), with => with.HttpRequest());

                // Then
                result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
            }
        }
    }
}