//using System;
//using System.Collections.Generic;
//using System.Dynamic;
//using System.Linq;
//using CollectionsOnline.Core.Config;
//using CollectionsOnline.Core.Indexes;
//using CollectionsOnline.Core.Models;
//using CollectionsOnline.Tests.Fakes;
//using CollectionsOnline.WebSite.Infrastructure;
//using CollectionsOnline.WebSite.Modules.Api;
//using Nancy.Json;
//using Nancy.Testing;
//using Newtonsoft.Json;
//using Raven.Client;
//using Raven.Tests.Helpers;
//using Shouldly;
//using Xunit;

//namespace CollectionsOnline.Tests.Website.Modules
//{
//    public class ApiModuleBaseTests : RavenTestBase
//    {
//        private Browser NewBrowser(IDocumentStore documentStore)
//        {
//            return new Browser(new WebSiteBootstrapper());

//            //return new Browser(with =>
//            //{
//            //    with.Module<ItemsApiModule>();
//            //    with.Dependency(documentStore.OpenSession());
//            //    with.ApplicationStartup((container, pipelines) =>
//            //    {
//            //        JsonSettings.MaxJsonLength = Int32.MaxValue;
//            //        AutomapperConfig.Initialize();
//            //    });
//            //});
//        }

//        [Fact]
//        public void GivenEnvelopeRequest_GetItems_ReturnsEnvelope()
//        {
//            using (var documentStore = NewDocumentStore(seedData: new[] { FakeItems.CreateFakeItems(100) }, indexes: new[] { new CombinedIndex() }, requestedStorage: "esent"))
//            { 
//                // Given
//                var browser = NewBrowser(documentStore);
            
//                // When
//                var result = browser.Get(string.Format("/{0}{1}/items", Constants.ApiPathBase, Constants.CurrentApiVersionPath), with =>
//                    {
//                        with.HttpRequest();
//                        with.Query("envelope", "true");
//                    });

//                // Then
//                dynamic bodyResult = JsonConvert.DeserializeObject<ExpandoObject>(result.Body.AsString());
//                ((object)bodyResult.response).ShouldNotBe(null);
//            }
//        }

//        [Fact]
//        public void GivenOnePageRequestAndDefaultPerPage_GetItems_ReturnsOnePage()
//        {
//            using (var documentStore = NewDocumentStore(seedData: new[] { FakeItems.CreateFakeItems(100) }, indexes: new[] { new CombinedIndex() }))
//            {
//                // Given
//                var browser = NewBrowser(documentStore);
                
//                // When
//                var result = browser.Get(string.Format("/{0}{1}/items", Constants.ApiPathBase, Constants.CurrentApiVersionPath), with =>
//                {
//                    with.HttpRequest();
//                    with.Query("page", "1");
//                });

//                // Then
//                result.Body.DeserializeJson<IEnumerable<Item>>().Count().ShouldBe(Constants.PagingPerPageDefault);
//            }
//        }

//        [Fact]
//        public void GivenOnePageRequest_GetItems_ReturnsLinkHeader()
//        {
//            using (var documentStore = NewDocumentStore(seedData: new[] { FakeItems.CreateFakeItems(100) }, indexes: new[] { new CombinedIndex() }))
//            {
//                // Given
//                var browser = NewBrowser(documentStore);

//                // Given When
//                var result = browser.Get(string.Format("/{0}{1}/items", Constants.ApiPathBase, Constants.CurrentApiVersionPath), with =>
//                {
//                    with.HttpRequest();
//                    with.Query("page", "1");
//                });

//                // Then
//                result.Headers["Link"].ShouldNotBe(string.Empty);
//            }
//        }

//        [Fact]
//        public void GivenPageRequest_GetItems_ReturnsCorrectItem()
//        {
//            using (var documentStore = NewDocumentStore(seedData: new[] { FakeItems.CreateFakeItems(100) }, indexes: new[] { new CombinedIndex() }))
//            {
//                // Given
//                var browser = NewBrowser(documentStore);

//                // Given When
//                var result = browser.Get(string.Format("/{0}{1}/items", Constants.ApiPathBase, Constants.CurrentApiVersionPath), with =>
//                {
//                    with.HttpRequest();
//                    with.Query("page", "2");
//                });

//                // Then
//                result.Body.DeserializeJson<IEnumerable<Item>>().First().Id.ShouldBe("items/41");
//            }
//        }
//    }
//}