using System;
using System.Configuration;
using System.IO;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Factories;
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
            var directory = string.Format("{0}\\sitemaps\\", AppDomain.CurrentDomain.BaseDirectory);

            // When
            PathFactory.CreateDestPath(directory);

            // Then
            new DirectoryInfo(directory).Exists.ShouldBe(true);

            // Cleanup
            Directory.Delete(directory);
        }

        [Fact]
        public void GivenFileToCreate_CreateDestPath_CreatesPath()
        {
            // Given
            var file = string.Format("{0}\\sitemaps\\sitemap-set-1.xml.gz", AppDomain.CurrentDomain.BaseDirectory);
            var directory = Path.GetDirectoryName(file);

            // When
            PathFactory.CreateDestPath(file);

            // Then
            new DirectoryInfo(directory).Exists.ShouldBe(true);

            // Cleanup
            Directory.Delete(directory);
        }

        [Fact]
        public void MakeDestPath_CreatesDestPath()
        {
            // Given 
            var file = string.Format("{0}\\content\\media\\{1}\\{2}", ConfigurationManager.AppSettings["WebSitePath"], 1, "1-small.jpg");
            var directory = Path.GetDirectoryName(file);

            // When
            PathFactory.MakeDestPath(1, ".jpg", FileDerivativeType.Small);

            // Then
            new DirectoryInfo(directory).Exists.ShouldBe(true);

            // Cleanup
            Directory.Delete(string.Format("{0}\\content\\", ConfigurationManager.AppSettings["WebSitePath"]), true);
        }
    }
}
