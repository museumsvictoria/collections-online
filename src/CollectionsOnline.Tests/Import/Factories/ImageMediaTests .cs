using System.IO;
using CollectionsOnline.Tests.Resources;
using ImageMagick;
using Shouldly;
using Xunit;

namespace CollectionsOnline.Tests.Import.Factories
{
    public class ImageMediaTests : FileBasedTest
    {
        public ImageMediaTests() : base(false)
        {
        }

        [Fact]
        public void ImageMagick_Correctly_SavesThumbnailImage()
        {
            // Given
            var outputFileName = $"{Files.OutputFolder}{Path.GetFileNameWithoutExtension(Files.AgileAntechinus)}-thumbnail.jpg";

            using (var image = new MagickImage())
            {
                image.Read(Files.AgileAntechinus);

                var profile = image.GetColorProfile();
                image.Strip();
                if (profile != null)
                    image.SetProfile(profile);

                image.Format = MagickFormat.Jpeg;
                image.Quality = 70;
                image.FilterType = FilterType.Lanczos;
                image.ColorSpace = ColorSpace.sRGB;
                
                image.Resize(new MagickGeometry(250) {FillArea = true});
                image.Crop(250, 250, Gravity.Center);
                image.UnsharpMask(0.5, 0.5, 0.5, 0.025);

                // When
                image.Write(outputFileName);
            }

            // Then
            File.Exists(outputFileName).ShouldBe(true);
        }
        
        [Fact]
        public void ImageMagick_Correctly_SavesSmallImage()
        {
            // Given
            var outputFileName = $"{Files.OutputFolder}{Path.GetFileNameWithoutExtension(Files.AgileAntechinus)}-small.jpg";

            using (var image = new MagickImage())
            {
                image.Read(Files.AgileAntechinus);

                var profile = image.GetColorProfile();
                image.Strip();
                if (profile != null)
                    image.SetProfile(profile);

                image.Format = MagickFormat.Jpeg;
                image.Quality = 76;
                image.FilterType = FilterType.Lanczos;
                image.ColorSpace = ColorSpace.sRGB;
                
                image.Resize(new MagickGeometry(0, 500));
                image.UnsharpMask(0.5, 0.5, 0.6, 0.025);

                // When
                image.Write(outputFileName);
            }

            // Then
            File.Exists(outputFileName).ShouldBe(true);
        }
        
        [Fact]
        public void ImageMagick_Correctly_SavesMediumImage()
        {
            // Given
            var outputFileName = $"{Files.OutputFolder}{Path.GetFileNameWithoutExtension(Files.AgileAntechinus)}-medium.jpg";

            using (var image = new MagickImage())
            {
                image.Read(Files.AgileAntechinus);

                var profile = image.GetColorProfile();
                image.Strip();
                if (profile != null)
                    image.SetProfile(profile);

                image.Format = MagickFormat.Jpeg;
                image.Quality = 76;
                image.FilterType = FilterType.Lanczos;
                image.ColorSpace = ColorSpace.sRGB;
                
                image.Resize(new MagickGeometry(1500) { Greater = true });
                image.UnsharpMask(0.5, 0.5, 0.6, 0.025);

                // When
                image.Write(outputFileName);
            }

            // Then
            File.Exists(outputFileName).ShouldBe(true);
        }
        
        [Fact]
        public void ImageMagick_Correctly_SavesLargeImage()
        {
            // Given
            var outputFileName = $"{Files.OutputFolder}{Path.GetFileNameWithoutExtension(Files.AgileAntechinus)}-large.jpg";

            using (var image = new MagickImage())
            {
                image.Read(Files.AgileAntechinus);

                var profile = image.GetColorProfile();
                image.Strip();
                if (profile != null)
                    image.SetProfile(profile);

                image.Format = MagickFormat.Jpeg;
                image.Quality = 86;
                image.FilterType = FilterType.Lanczos;
                image.ColorSpace = ColorSpace.sRGB;
                
                image.Resize(new MagickGeometry(3000) { Greater = true });
                image.UnsharpMask(0.5, 0.5, 0.6, 0.025);

                // When
                image.Write(outputFileName);
            }

            // Then
            File.Exists(outputFileName).ShouldBe(true);
        }
        
        [Fact]
        public void ImageMagick_Correctly_OrientsImage()
        {
            // Given
            var outputFileName = $"{Files.OutputFolder}{Path.GetFileNameWithoutExtension(Files.Brochure)}.jpg";

            using (var image = new MagickImage())
            {
                image.Read(Files.Brochure);

                var profile = image.GetColorProfile();
                image.Strip();
                if (profile != null)
                    image.SetProfile(profile);

                image.Format = MagickFormat.Jpeg;
                image.Quality = 76;
                image.FilterType = FilterType.Lanczos;
                image.ColorSpace = ColorSpace.sRGB;
                image.AutoOrient();
                image.Resize(new MagickGeometry(1500) { Greater = true });
                image.UnsharpMask(0.5, 0.5, 0.6, 0.025);

                // When
                image.Write(outputFileName);
            }

            // Then
            File.Exists(outputFileName).ShouldBe(true);
        }
    }
}