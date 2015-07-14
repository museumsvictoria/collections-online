using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Infrastructure;
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
        private readonly IImuSessionProvider _imuSessionProvider;
        private readonly IList<ImageMediaJob> _imageMediaJobs;

        public ImageMediaFactory(
            IImuSessionProvider imuSessionProvider)
        {
            _imuSessionProvider = imuSessionProvider;            

            // Build a list the various image conversions used in the application
            _imageMediaJobs = new List<ImageMediaJob>
            {
                new ImageMediaJob
                {
                    FileDerivativeType = FileDerivativeType.Original,
                    ResizeLayer = new ResizeLayer(new Size(3000, 3000), ResizeMode.Max, upscale:false),
                    Quality = 90
                },
                new ImageMediaJob
                {
                    FileDerivativeType = FileDerivativeType.Thumbnail,
                    ResizeLayer = new ResizeLayer(new Size(250, 250), ResizeMode.Crop),
                    BackgroundColor = Color.White,
                    Quality = 80
                },
                new ImageMediaJob
                {
                    FileDerivativeType = FileDerivativeType.Medium,
                    ResizeLayer = new ResizeLayer(new Size(0, 500), ResizeMode.Max),
                    Quality = 80
                },
                new ImageMediaJob
                {
                    FileDerivativeType = FileDerivativeType.Large,
                    ResizeLayer = new ResizeLayer(new Size(1500, 1500), ResizeMode.Max, upscale:false),
                    Quality = 80
                }
            };
        }

        public bool Make(ref ImageMedia imageMedia, ResizeMode? thumbnailResizeMode)
        {
            var stopwatch = Stopwatch.StartNew();

            if (ThereAreExistingMedia(ref imageMedia))
            {
                stopwatch.Stop();
                _log.Trace("Loaded existing image media resources in {0} ms", stopwatch.ElapsedMilliseconds);

                return true;
            }

            // Fetch from Imu as we were not able to find local files
            try
            {
                using (var imuSession = _imuSessionProvider.CreateInstance("emultimedia"))
                {
                    imuSession.FindKey(imageMedia.Irn);
                    var result = imuSession.Fetch("start", 0, -1, new[] { "resource" }).Rows[0];

                    var resource = result.GetMap("resource");

                    if (resource == null)
                        throw new IMuException("MultimediaResourceNotFound");

                    var fileStream = resource["file"] as FileStream;

                    using (var imageFactory = new ImageFactory())
                    {
                        imageFactory
                            .Load(fileStream);

                        stopwatch.Stop();
                        _log.Trace("Loaded image media resource FileStream in {0} ms ({1} kbytes, {2} width, {3} height)", stopwatch.ElapsedMilliseconds, (fileStream.Length / 1024f).ToString("N"), imageFactory.Image.Width,
                            imageFactory.Image.Height);

                        stopwatch.Reset();
                        stopwatch.Start();

                        foreach (var imageMediaJob in _imageMediaJobs)
                        {
                            imageFactory.Reset();

                            // Hack for overridding padding
                            if (imageMediaJob.FileDerivativeType == FileDerivativeType.Thumbnail && thumbnailResizeMode.HasValue)
                                imageMediaJob.ResizeLayer.ResizeMode = thumbnailResizeMode.Value;

                            // Indirectly call graphics.drawimage to get around multi layer tiff images causing gdi exceptions when image is not resized.
                            if (imageMediaJob.ResizeLayer.Upscale == false && (imageFactory.Image.Width < imageMediaJob.ResizeLayer.Size.Width || imageFactory.Image.Height < imageMediaJob.ResizeLayer.Size.Height))
                                imageFactory.Brightness(0);

                            imageFactory
                                .Resize(imageMediaJob.ResizeLayer)
                                .Format(new JpegFormat())
                                .Quality(imageMediaJob.Quality);

                            if (imageMediaJob.BackgroundColor.HasValue)
                                imageFactory.BackgroundColor(imageMediaJob.BackgroundColor.Value);

                            var destPath = PathFactory.MakeDestPath(imageMedia.Irn, ".jpg", imageMediaJob.FileDerivativeType);

                            imageFactory.Save(destPath);

                            // Set property via reflection (ImageMediaFile properties are used instead of a collection due to Raven Indexing)
                            typeof(ImageMedia).GetProperties().First(x => x.PropertyType == typeof(ImageMediaFile) && x.Name == imageMediaJob.FileDerivativeType.ToString())
                                .SetValue(imageMedia, new ImageMediaFile
                                {
                                    Uri = PathFactory.MakeUriPath(imageMedia.Irn, ".jpg", imageMediaJob.FileDerivativeType),
                                    Size = new FileInfo(destPath).Length,
                                    Width = imageFactory.Image.Width,
                                    Height = imageFactory.Image.Height
                                });
                        }
                    }

                    stopwatch.Stop();
                    _log.Trace("Created all derivative image media in {0} ms", stopwatch.ElapsedMilliseconds);

                    return true;
                }
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
                    _log.Error("Error saving image media {0}, un-recoverable error", imageMedia.Irn);
                    throw;
                }
            }

            return false;
        }

        private bool ThereAreExistingMedia(ref ImageMedia imageMedia)
        {
            // First check to see if we are not overwriting existing data,            
            if (!bool.Parse(ConfigurationManager.AppSettings["OverwriteExistingMedia"]))
            {
                var imageMediaIrn = imageMedia.Irn;

                // then if we find existing files matching all of our image media jobs, use the files on disk instead
                if (_imageMediaJobs.All(x => File.Exists(PathFactory.MakeDestPath(imageMediaIrn, ".jpg", x.FileDerivativeType))))
                {
                    foreach (var imageMediaJob in _imageMediaJobs)
                    {
                        using (var imageFactory = new ImageFactory())
                        {
                            var destPath = PathFactory.MakeDestPath(imageMediaIrn, ".jpg", imageMediaJob.FileDerivativeType);
                            
                            imageFactory.Load(destPath);

                            // Set property via reflection (ImageMediaFile properties are used instead of a collection due to Raven Indexing)
                            typeof(ImageMedia).GetProperties().First(x => x.PropertyType == typeof(ImageMediaFile) && x.Name == imageMediaJob.FileDerivativeType.ToString())
                                .SetValue(imageMedia, new ImageMediaFile
                                {
                                    Uri = PathFactory.MakeUriPath(imageMedia.Irn, ".jpg", imageMediaJob.FileDerivativeType),
                                    Size = new FileInfo(destPath).Length,
                                    Width = imageFactory.Image.Width,
                                    Height = imageFactory.Image.Height
                                });
                        }
                    }
                    
                    return true;
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