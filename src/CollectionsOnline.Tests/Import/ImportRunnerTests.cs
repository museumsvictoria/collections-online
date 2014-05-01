using System;
using System.Collections;
using System.Collections.Generic;
using CollectionsOnline.Core.Config;
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
        [Fact]
        public void GivenImportRunning_RunImport_KeepsRunning()
        {
            // Given
            DataToBeSeeded = new List<IEnumerable>
                {
                    new[] { new Application { ImportsRunning = true }}
                };
            var import = Substitute.For<IImport>();
            var importRunner = new ImportRunner(DocumentSession.Advanced.DocumentStore, new[] { import });

            // When
            importRunner.Run();

            // Then
            var application = DocumentSession.Load<Application>(Constants.ApplicationId);

            application.ImportsRunning.ShouldBe(true);
            import.DidNotReceive().Run();
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
            var importRunner = new ImportRunner(DocumentSession.Advanced.DocumentStore, new[] { import });

            // When
            importRunner.Run();

            // Then
            var application = DocumentSession.Load<Application>(Constants.ApplicationId);

            application.ImportsRunning.ShouldBe(false);
            import.Received().Run();
        }
    }
}