using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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
        private readonly IList<ImageMediaJob> _imageMediaJobs;

        public ImageMediaFactory(Session session)
        {
            _module = new Module("emultimedia", session);

            // Build a list the various image conversions used in the application
            _imageMediaJobs = new List<ImageMediaJob>
            {
                new ImageMediaJob
                {
                    FileDerivativeType = FileDerivativeType.Original,
                    ResizeLayer = new ResizeLayer(new Size(4000, 4000), ResizeMode.Max, upscale:false),
                    Quality = 90
                },
                new ImageMediaJob
                {
                    FileDerivativeType = FileDerivativeType.Thumbnail,
                    ResizeLayer = new ResizeLayer(new Size(350, 350)),
                    BackgroundColor = Color.White,
                    Quality = 60
                },
                //new ImageMediaJob
                //{
                //    FileDerivativeType = FileDerivativeType.Small,
                //    ResizeLayer = new ResizeLayer(new Size(500, 500), ResizeMode.Max),
                //    Quality = 70
                //},
                //new ImageMediaJob
                //{
                //    FileDerivativeType = FileDerivativeType.Medium,
                //    ResizeLayer = new ResizeLayer(new Size(800, 800), ResizeMode.Max),
                //    Quality = 60
                //},
                new ImageMediaJob
                {
                    FileDerivativeType = FileDerivativeType.Large,
                    ResizeLayer = new ResizeLayer(new Size(1040, 1040), ResizeMode.Max),
                    Quality = 80
                }
            };
        }

        public bool Make(ref ImageMedia imageMedia)
        {
            var stopwatch = Stopwatch.StartNew();
            var imageMediaIrn = imageMedia.Irn;

            try
            {
                // First check to see if we are not overwriting existing data, 
                // then if we find existing files matching all of our image media jobs, use the files on disk instead
                if (!bool.Parse(ConfigurationManager.AppSettings["OverwriteExistingMedia"]))
                {
                    if (_imageMediaJobs.All(x => File.Exists(PathFactory.MakeDestPath(imageMediaIrn, FileFormatType.Jpg, x.FileDerivativeType))))
                    {
                        foreach (var imageMediaJob in _imageMediaJobs)
                        {
                            using (var imageFactory = new ImageFactory())
                            {
                                var destPath = PathFactory.MakeDestPath(imageMediaIrn, FileFormatType.Jpg, imageMediaJob.FileDerivativeType);
                                var uriPath = PathFactory.MakeUriPath(imageMedia.Irn, FileFormatType.Jpg, imageMediaJob.FileDerivativeType);

                                imageFactory.Load(destPath);

                                // Set property via reflection (ImageMediaFile properties are used instead of a collection due to Raven Indexing)
                                typeof(ImageMedia).GetProperties().First(x => x.PropertyType == typeof(ImageMediaFile) && x.Name == imageMediaJob.FileDerivativeType.ToString())
                                    .SetValue(imageMedia, new ImageMediaFile
                                    {
                                        Uri = uriPath,
                                        Width = imageFactory.Image.Width,
                                        Height = imageFactory.Image.Height
                                    });
                            }
                        }

                        stopwatch.Stop();
                        _log.Trace("Loaded existing media resources in {0} ms", stopwatch.ElapsedMilliseconds);

                        return true;
                    }
                }

                // Fetch MMR as we were not able to find local files
                _module.FindKey(imageMedia.Irn);
                var result = _module.Fetch("start", 0, -1, new[] { "resource" }).Rows[0];

                var resource = result.GetMap("resource");

                if (resource == null)
                    throw new IMuException("MultimediaResourceNotFound");

                var fileStream = resource["file"] as FileStream;

                using (var imageFactory = new ImageFactory())
                {
                    imageFactory
                        .Load(fileStream);

                    stopwatch.Stop();
                    _log.Trace("Loaded media resource FileStream in {0} ms ({1} kbytes, {2} width, {3} height)", stopwatch.ElapsedMilliseconds, (fileStream.Length / 1024f).ToString("N"), imageFactory.Image.Width,
                        imageFactory.Image.Height);

                    stopwatch.Reset();
                    stopwatch.Start();

                    foreach (var imageMediaJob in _imageMediaJobs)
                    {
                        imageFactory.Reset();

                        var destinationPath = PathFactory.MakeDestPath(imageMediaIrn, FileFormatType.Jpg, imageMediaJob.FileDerivativeType);
                        var uriPath = PathFactory.MakeUriPath(imageMediaIrn, FileFormatType.Jpg, imageMediaJob.FileDerivativeType);

                        // Indirectly call graphics.drawimage to get around multi layer tiff images causing gdi exceptions when image is not resized.
                        if (imageMediaJob.ResizeLayer.Upscale == false && (imageFactory.Image.Width < imageMediaJob.ResizeLayer.Size.Width || imageFactory.Image.Height < imageMediaJob.ResizeLayer.Size.Height))
                            imageFactory.Brightness(0);                        

                        imageFactory
                            .Resize(imageMediaJob.ResizeLayer)
                            .Format(new JpegFormat())
                            .Quality(imageMediaJob.Quality);

                        if (imageMediaJob.BackgroundColor.HasValue)
                            imageFactory.BackgroundColor(imageMediaJob.BackgroundColor.Value);

                        imageFactory.Save(destinationPath);

                        // Set property via reflection (ImageMediaFile properties are used instead of a collection due to Raven Indexing)
                        typeof(ImageMedia).GetProperties().First(x => x.PropertyType == typeof(ImageMediaFile) && x.Name == imageMediaJob.FileDerivativeType.ToString())
                            .SetValue(imageMedia, new ImageMediaFile
                            {
                                Uri = uriPath,
                                Width = imageFactory.Image.Width,
                                Height = imageFactory.Image.Height
                            });
                    }
                }

                stopwatch.Stop();
                _log.Trace("Created all derivative image media in {0} ms", stopwatch.ElapsedMilliseconds);

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

        private class ImageMediaJob
        {
            public FileDerivativeType FileDerivativeType { get; set; }

            public ResizeLayer ResizeLayer { get; set; }

            public int Quality { get; set; }

            public Color? BackgroundColor { get; set; }
        }
    }
}