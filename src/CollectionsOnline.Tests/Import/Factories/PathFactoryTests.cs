using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Factories;
using CollectionsOnline.Tests.Fakes;
using NSubstitute;
using Raven.Tests.Helpers;
using Shouldly;
using Xunit;

namespace CollectionsOnline.Tests.Import.Factories
{
    public class PathFactoryTests : RavenTestBase
    {
        [Fact]
        public void GivenDirectoryToCreate_CreateDestPath_CreatesPath()
        {
            // Given
            var directoryToCreate = string.Format("{0}\\sitemaps", AppDomain.CurrentDomain.BaseDirectory);

            // When
            PathFactory.CreateDestPath(directoryToCreate);

            // Then
            new DirectoryInfo(string.Format("{0}\\sitemaps\\", AppDomain.CurrentDomain.BaseDirectory)).Exists.ShouldBe(true);

            // Cleanup
            Directory.Delete(directoryToCreate);
        }

        [Fact]
        public void GivenFileToCreate_CreateDestPath_CreatesPath()
        {
            // Given
            var directoryToCreate = string.Format("{0}\\sitemaps\\sitemap-set-1.xml.gz", AppDomain.CurrentDomain.BaseDirectory);

            // When
            PathFactory.CreateDestPath(directoryToCreate);

            // Then
            new DirectoryInfo(string.Format("{0}\\sitemaps\\", AppDomain.CurrentDomain.BaseDirectory)).Exists.ShouldBe(true);

            // Cleanup
            Directory.Delete(directoryToCreate);
        }
    }
}
