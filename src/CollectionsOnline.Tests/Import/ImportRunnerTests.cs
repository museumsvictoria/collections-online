using System;
using System.Collections;
using System.Collections.Generic;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Imports;
using CollectionsOnline.Import.Infrastructure;
using NSubstitute;
using Shouldly;
using WorldDomination.Raven.Tests.Helpers;
using Xunit;

namespace CollectionsOnline.Tests.Import
{
    public class ImportRunnerTests : RavenDbTestBase
    {
        public ImportRunnerTests()
        {
            IndexesToExecute = new List<Type>
            {
                typeof(Combined)
            };
        }

        [Fact]
        public void GivenImportRunning_RunImport_KeepsRunning()
        {
            // Given
            DataToBeSeeded = new List<IEnumerable>
                {
                    new[] { new Application { ImportsRunning = true }}
                };
            var import = Substitute.For<IImport>();
            var importRunner = new ImportRunner(DocumentStore, new[] { import });

            // When
            importRunner.Run();

            // Then
            using (var documentSession = DocumentStore.OpenSession())
            {
                var application = documentSession.Load<Application>(Constants.ApplicationId);
                application.ImportsRunning.ShouldBe(true);
                import.DidNotReceive().Run();
            }
        }

        [Fact]
        public void GivenImportNotRunning_RunImport_CompletesImport()
        {
            // Given
            DataToBeSeeded = new List<IEnumerable>
                {
                    new[] { new Application() }
                };
            var import = Substitute.For<IImport>();
            var importRunner = new ImportRunner(DocumentStore, new[] { import });

            // When
            // TODO: work out what to do with failing test and Raven/DocumentsByEntityName not existing in embedded db
            importRunner.Run();

            // Then
            using (var documentSession = DocumentStore.OpenSession())
            {
                var application = documentSession.Load<Application>(Constants.ApplicationId);
                application.ImportsRunning.ShouldBe(false);
                import.Received().Run();
            }
        }
    }
}