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
//    public class SpeciesApiModuleTests : RavenTestBase
//    {
//        [Fact]
//        public void GetSpecies_ReturnsSpecies()
//        {
//            using (var documentStore = NewDocumentStore(seedData: new[] { FakeSpecies.CreateFakeSpecies(5) }, indexes: new[] { new CombinedIndex() }))
//            {
//                // Given
//                var browser = new Browser(with =>
//                {
//                    with.Module<SpeciesApiModule>();
//                    with.Dependency(documentStore.OpenSession());
//                    with.ApplicationStartup((container, pipelines) => AutomapperConfig.Initialize());
//                });
                
//                // When
//                var result = browser.Get(string.Format("/{0}{1}/species", Constants.ApiPathBase, Constants.CurrentApiVersionPath), with => with.HttpRequest());

//                // Then
//                result.Body.DeserializeJson<IEnumerable<Species>>().Count().ShouldBe(5);
//            }
//        }

//        [Fact]
//        public void GivenAnInvalidId_GetSpecies_ReturnsNotFound()
//        {
//            using (var documentStore = NewDocumentStore(seedData: new[] { FakeSpecies.CreateFakeSpecies(5) }, indexes: new[] { new CombinedIndex() }))
//            {
//                // Given
//                var browser = new Browser(with =>
//                {
//                    with.Module<SpeciesApiModule>();
//                    with.Dependency(documentStore.OpenSession());
//                    with.ApplicationStartup((container, pipelines) => AutomapperConfig.Initialize());
//                });

//                // When
//                var result = browser.Get(string.Format("/{0}{1}/species/6", Constants.ApiPathBase, Constants.CurrentApiVersionPath), with => with.HttpRequest());

//                // Then
//                result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
//            }
//        }
//    }
//}