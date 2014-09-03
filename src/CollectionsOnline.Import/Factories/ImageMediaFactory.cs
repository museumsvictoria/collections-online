using System;
using System.Drawing;
using System.IO;
using CollectionsOnline.Core.Models;
using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Formats;
using IMu;
using NLog;

namespace CollectionsOnline.Import.Factories
{
    public class ImageMediaFactory : IImageMediaFactory
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly Module _module;

        public ImageMediaFactory(Session session)
        {
            _module = new Module("emultimedia", session);
        }

        public bool Make(ref ImageMedia imageMedia)
        {
            try
            {
                _module.FindKey(imageMedia.Irn);
                var result = _module.Fetch("start", 0, -1, new[] { "resource" }).Rows[0];
                var resource = result.GetMap("resource");

                if (resource == null)
                    throw new IMuException("MultimediaResourceNotFound");

                var fileStream = resource["file"] as FileStream;

                using (var imageFactory = new ImageFactory())
                {
                    // Original
                    imageFactory
                        .Load(fileStream)
                        .Format(new JpegFormat())
                        .Quality(90)
                        .Save(PathFactory.MakeDestPath(imageMedia.Irn, FileFormatType.Jpg, "original"));

                    imageMedia.Original = new ImageMediaFile
                    {
                        Uri = PathFactory.MakeUriPath(imageMedia.Irn, FileFormatType.Jpg, "original"),
                        Width = imageFactory.Image.Width,
                        Height = imageFactory.Image.Height
                    };

                    // Thumbnail
                    imageFactory
                        .Reset()
                        .Resize(new ResizeLayer(new Size(350, 350)))
                        .Format(new JpegFormat())
                        .Quality(60)
                        .BackgroundColor(Color.White)
                        .Save(PathFactory.MakeDestPath(imageMedia.Irn, FileFormatType.Jpg, "thumbnail"));

                    imageMedia.Thumbnail = new ImageMediaFile
                    {
                        Uri = PathFactory.MakeUriPath(imageMedia.Irn, FileFormatType.Jpg, "thumbnail"),
                        Width = imageFactory.Image.Width,
                        Height = imageFactory.Image.Height
                    };

                    // Small
                    imageFactory
                        .Reset()
                        .Resize(new ResizeLayer(new Size(500, 500), ResizeMode.Max))
                        .Format(new JpegFormat())
                        .Quality(70)
                        .Save(PathFactory.MakeDestPath(imageMedia.Irn, FileFormatType.Jpg, "small"));

                    imageMedia.Small = new ImageMediaFile
                    {
                        Uri = PathFactory.MakeUriPath(imageMedia.Irn, FileFormatType.Jpg, "small"),
                        Width = imageFactory.Image.Width,
                        Height = imageFactory.Image.Height
                    };

                    // Medium
                    imageFactory
                        .Reset()
                        .Resize(new ResizeLayer(new Size(800, 800), ResizeMode.Max))
                        .Format(new JpegFormat())
                        .Quality(70)
                        .Save(PathFactory.MakeDestPath(imageMedia.Irn, FileFormatType.Jpg, "medium"));

                    imageMedia.Medium = new ImageMediaFile
                    {
                        Uri = PathFactory.MakeUriPath(imageMedia.Irn, FileFormatType.Jpg, "medium"),
                        Width = imageFactory.Image.Width,
                        Height = imageFactory.Image.Height
                    };

                    // Large
                    imageFactory
                        .Reset()
                        .Resize(new ResizeLayer(new Size(1200, 1200), ResizeMode.Max))
                        .Format(new JpegFormat())
                        .Quality(70)
                        .Save(PathFactory.MakeDestPath(imageMedia.Irn, FileFormatType.Jpg, "large"));

                    imageMedia.Large = new ImageMediaFile
                    {
                        Uri = PathFactory.MakeUriPath(imageMedia.Irn, FileFormatType.Jpg, "large"),
                        Width = imageFactory.Image.Width,
                        Height = imageFactory.Image.Height
                    };
                }
              
                return true;
            }
            catch (Exception exception)
            {
                if (exception is IMuException && ((IMuException)exception).ID == "MultimediaResolutionNotFound")
                {
                    // Error is a known issue that will be picked up in subsequent imports once the data is fixed. So we don't need to re-throw exception.
                    _log.Warn("Multimedia resolution was not found, Unable to save image at this time {0}, {1}", imageMedia.Irn, exception);
                }
                else if (exception is IMuException && ((IMuException)exception).ID == "MultimediaResourceNotFound")
                {
                    // Error is a known issue that will be picked up in subsequent imports once the data is fixed. So we don't need to re-throw exception.
                    _log.Warn("Multimedia resource was not found, unable to save image at this time {0}, {1}", imageMedia.Irn, exception);
                }
                else
                {
                    // Error is unexpected therefore we want the entire import to fail, re-throw the error.
                    _log.Error("Error saving image {0}, un-recoverable error, {1}", imageMedia.Irn, exception);
                    throw;
                }
            }

            return false;
        }
    }
}