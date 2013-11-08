using System;
using System.Collections;
using System.Collections.Generic;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import;
using CollectionsOnline.Import.Importers;
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
                    new[] { new Application { DataImportRunning = true }}
                };
            var import = Substitute.For<IImport<EmuAggregateRoot>>();
            var importRunner = new ImportRunner(DocumentSession.Advanced.DocumentStore, new[] { import });

            // When
            importRunner.Run();

            // Then
            var application = DocumentSession.Load<Application>(Constants.ApplicationId);

            application.DataImportRunning.ShouldBe(true);
            import.DidNotReceive().Run(Arg.Any<DateTime>());
        }

        [Fact]
        public void GivenImportNotRunning_RunImport_CompletesImport()
        {
            // Given
            DataToBeSeeded = new List<IEnumerable>
                {
                    new[] { new Application() }
                };
            var import = Substitute.For<IImport<EmuAggregateRoot>>();
            var importRunner = new ImportRunner(DocumentSession.Advanced.DocumentStore, new[] { import });

            // When
            importRunner.Run();

            // Then
            var application = DocumentSession.Load<Application>(Constants.ApplicationId);

            application.DataImportRunning.ShouldBe(false);
            import.Received().Run(default(DateTime));
        }
    }
}