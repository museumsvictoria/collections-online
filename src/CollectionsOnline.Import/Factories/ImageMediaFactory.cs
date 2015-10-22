using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Imports;
using CollectionsOnline.Import.Infrastructure;
using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Formats;
using IMu;
using Raven.Client;
using CollectionsOnline.Core.Extensions;
using Serilog;

namespace CollectionsOnline.Import.Factories
{
    public class ImageMediaFactory : IImageMediaFactory
    {
        private readonly IDocumentStore _documentStore;
        private readonly IImuSessionProvider _imuSessionProvider;
        private readonly IList<ImageMediaJob> _imageMediaJobs;

        public ImageMediaFactory(
            IDocumentStore documentStore,
            IImuSessionProvider imuSessionProvider)
        {
            _documentStore = documentStore;
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

            if (FileExists(ref imageMedia))
            {
                stopwatch.Stop();
                Log.Logger.Debug("Found existing image {Irn} in {ElapsedMilliseconds} ms", imageMedia.Irn, stopwatch.ElapsedMilliseconds);

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
                    Log.Logger.Debug("Completed image {Irn} creation in {ElapsedMilliseconds} ms", imageMedia.Irn, stopwatch.ElapsedMilliseconds);

                    return true;
                }
            }
            catch (Exception ex)
            {
                if (ex is IMuException && ((IMuException)ex).ID == "MultimediaResolutionNotFound")
                {
                    Log.Logger.Warning(ex, "Multimedia resolution not found, unable to save image {Irn}", imageMedia.Irn);
                }
                else if (ex is IMuException && ((IMuException)ex).ID == "MultimediaResourceNotFound")
                {
                    Log.Logger.Warning(ex, "Multimedia resource not found, unable to save image {Irn}", imageMedia.Irn);
                }
                else
                {
                    // Error is unexpected therefore we want the entire import to fail
                    Log.Logger.Fatal(ex, "Unexpected error occured creating image {Irn}", imageMedia.Irn);
                    throw;
                }
            }

            return false;
        }

        private bool FileExists(ref ImageMedia imageMedia)
        {
            // First check to see if we are not simply overwriting all existing media
            if (bool.Parse(ConfigurationManager.AppSettings["OverwriteExistingMedia"])) 
                return false;

            var mediaIrn = imageMedia.Irn;

            // Check to see whether the file has changed in emu first
            using (var documentSession = _documentStore.OpenSession())
            {
                // Find the latest document who's media contains the media we are checking
                var result = documentSession
                    .Query<MediaByIrnWithChecksumResult, MediaByIrnWithChecksum>()
                    .OrderByDescending(x => x.DateModified)
                    .FirstOrDefault(x => x.Irn == mediaIrn);

                // If all imu imports have run before (not simply loading images from disk) and there are no results that use this media and we need to save file
                var allImportsComplete = documentSession
                    .Load<Application>(Constants.ApplicationId)
                    .ImportStatuses.Where(x => x.ImportType.Contains(typeof(ImuImport<>).Name, StringComparison.OrdinalIgnoreCase))
                    .Select(x => x.PreviousDateRun)
                    .Any(x => x.HasValue);

                if (allImportsComplete && result == null)
                    return false;

                // If existing media checksum does not match the one from emu we need to save file
                if (result != null && result.Md5Checksum != imageMedia.Md5Checksum)
                {
                    Log.Logger.Warning("Existing image {Irn} found but checksum {ExistingChecksum} did not match new image {NewChecksum}", imageMedia.Irn, result.Md5Checksum, imageMedia.Md5Checksum);
                    return false;
                }
            }

            // then check whether media exists on disk for all media jobs
            if (_imageMediaJobs.All(x => File.Exists(PathFactory.MakeDestPath(mediaIrn, ".jpg", x.FileDerivativeType))))
            {
                foreach (var imageMediaJob in _imageMediaJobs)
                {
                    using (var imageFactory = new ImageFactory())
                    {
                        var destPath = PathFactory.MakeDestPath(mediaIrn, ".jpg", imageMediaJob.FileDerivativeType);
                            
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