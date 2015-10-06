//using System.Collections.Generic;
//using System.Linq;
//using CollectionsOnline.Core.Config;
//using CollectionsOnline.Core.Indexes;
//using CollectionsOnline.Core.Models;
//using CollectionsOnline.Tests.Fakes;
//using CollectionsOnline.WebSite.Infrastructure;
//using CollectionsOnline.WebSite.Modules.Api;
//using Nancy;
//using Nancy.Testing;
//using Raven.Tests.Helpers;
//using Shouldly;
//using Xunit;

//namespace CollectionsOnline.Tests.Website.Modules
//{
//    public class SpecimenApiModuleTests : RavenTestBase
//    {
//        [Fact]
//        public void GetSpecimens_ReturnsSpecimens()
//        {
//            using (var documentStore = NewDocumentStore(seedData: new [] { FakeSpecimens.CreateFakeSpecimens(5) }, indexes: new[] { new CombinedIndex() }))
//            {
//                // Given
//                var browser = new Browser(with =>
//                    {
//                        with.Module<SpecimensApiModule>();
//                        with.Dependency(documentStore.OpenSession());
//                        with.ApplicationStartup((container, pipelines) => AutomapperConfig.Initialize());
//                    });

//                // When
//                var result = browser.Get(string.Format("/{0}{1}/specimens", Constants.ApiPathBase, Constants.CurrentApiVersionPath), with => with.HttpRequest());

//                // Then
//                result.Body.DeserializeJson<IEnumerable<Specimen>>().Count().ShouldBe(5);
//            }
//        }

//        [Fact]
//        public void GivenAnInvalidId_GetSpecies_ReturnsNotFound()
//        {
//            using (var documentStore = NewDocumentStore(seedData: new[] {FakeSpecimens.CreateFakeSpecimens(5)}, indexes: new[] {new CombinedIndex()}))
//            {
//                // Given
//                var browser = new Browser(with =>
//                {
//                    with.Module<SpecimensApiModule>();
//                    with.Dependency(documentStore.OpenSession());
//                    with.ApplicationStartup((container, pipelines) => AutomapperConfig.Initialize());
//                });

//                // When
//                var result = browser.Get(string.Format("/{0}{1}/specimens/6", Constants.ApiPathBase, Constants.CurrentApiVersionPath), with => with.HttpRequest());

//                // Then
//                result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
//            }
//        }
//    }
//}