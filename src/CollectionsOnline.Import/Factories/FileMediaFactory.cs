using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Infrastructure;
using IMu;
using NLog;

namespace CollectionsOnline.Import.Factories
{
    public class FileMediaFactory : IFileMediaFactory
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly IImuSessionProvider _imuSessionProvider;

        public FileMediaFactory(
            IImuSessionProvider imuSessionProvider)
        {
            _imuSessionProvider = imuSessionProvider;
        }

        public bool Make(ref FileMedia fileMedia, string originalFileExtension)
        {
            var stopwatch = Stopwatch.StartNew();

            if (ThereAreExistingMedia(ref fileMedia, originalFileExtension))
            {
                stopwatch.Stop();
                _log.Trace("Loaded existing file media in {0} ms", stopwatch.ElapsedMilliseconds);

                return true;
            }

            // Fetch from Imu as we were not able to find local files
            try
            {
                using (var imuSession = _imuSessionProvider.CreateInstance("emultimedia"))
                {
                    imuSession.FindKey(fileMedia.Irn);
                    var result = imuSession.Fetch("start", 0, -1, new[] { "resource" }).Rows[0];

                    var resource = result.GetMap("resource");

                    if (resource == null)
                        throw new IMuException("MultimediaResourceNotFound");

                    // Load file stream
                    var fileStream = resource["file"] as FileStream;

                    // Save file stream
                    using (var file = File.OpenWrite(PathFactory.MakeDestPath(fileMedia.Irn, originalFileExtension, FileDerivativeType.None)))
                    {
                        fileStream.CopyTo(file);
                        fileStream.Dispose();

                        // Create media file
                        fileMedia.File = new MediaFile
                        {
                            Uri = PathFactory.MakeUriPath(fileMedia.Irn, originalFileExtension, FileDerivativeType.None),
                            Size = file.Length
                        };
                    } 

                    stopwatch.Stop();
                    _log.Trace("Created new file media in {0} ms", stopwatch.ElapsedMilliseconds);

                    return true;
                }
            }
            catch (Exception exception)
            {
                if (exception is IMuException && ((IMuException)exception).ID == "MultimediaResourceNotFound")
                {
                    // Error is a known issue that will be picked up in subsequent imports once the data is fixed. So we don't need to re-throw exception.
                    _log.Warn("Multimedia resource was not found, unable to save image at this time {0}, {1}", fileMedia.Irn, exception);
                }
                else
                {
                    // Error is unexpected therefore we want the entire import to fail, re-throw the error.
                    _log.Error("Error saving file media {0}, un-recoverable error, {1}", fileMedia.Irn, exception);
                    throw;
                }
            }

            return false;
        }

        private bool ThereAreExistingMedia(ref FileMedia fileMedia, string originalFileExtension)
        {
            // First check to see if we are not overwriting existing data,
            // then if we find existing files matching all of our image media jobs, use the files on disk instead
            if (!bool.Parse(ConfigurationManager.AppSettings["OverwriteExistingMedia"]))
            {
                var destPath = PathFactory.MakeDestPath(fileMedia.Irn, originalFileExtension, FileDerivativeType.None);
                var uriPath = PathFactory.MakeUriPath(fileMedia.Irn, originalFileExtension, FileDerivativeType.None);

                if (File.Exists(destPath))
                {
                    fileMedia.File = new MediaFile
                    {
                        Uri = uriPath,
                        Size = new FileInfo(destPath).Length
                    };

                    return true;
                }
            }

            return false;
        }
    }
}