using System.Collections;
using System.Collections.Generic;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Imports;
using CollectionsOnline.Import.Infrastructure;
using NSubstitute;
using Raven.Tests.Helpers;
using Shouldly;
using Xunit;

namespace CollectionsOnline.Tests.Import
{
    public class ImportRunnerTests : RavenTestBase
    {
        [Fact]
        public void GivenImportRunning_RunImport_KeepsRunning()
        {
            // Given
            using (
                var documentStore = NewDocumentStore(seedData: new[] {new[] {new Application {ImportsRunning = true}}}))
            {
                var import = Substitute.For<IImport>();
                var importRunner = new ImportRunner(documentStore, new[] {import});

                // When
                importRunner.Run();

                // Then
                using (var documentSession = documentStore.OpenSession())
                {
                    var application = documentSession.Load<Application>(Constants.ApplicationId);
                    application.ImportsRunning.ShouldBe(true);
                    import.DidNotReceive().Run();
                }
            }
        }

        // TODO: work out what to do with failing test and Raven/DocumentsByEntityName not existing in embedded db
        [Fact]
        public void GivenImportNotRunning_RunImport_CompletesImport()
        {
            // Given
            using (var documentStore = NewDocumentStore(seedData: new[] {new[] {new Application()}}))
            {
                var import = Substitute.For<IImport>();
                var importRunner = new ImportRunner(documentStore, new[] {import});

                // When
                importRunner.Run();

                // Then
                using (var documentSession = documentStore.OpenSession())
                {
                    var application = documentSession.Load<Application>(Constants.ApplicationId);
                    application.ImportsRunning.ShouldBe(false);
                    import.Received().Run();
                }
            }
        }
    }
}