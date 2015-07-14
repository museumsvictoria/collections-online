using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
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

        public bool Make(ref AudioMedia audioMedia, string originalFileExtension)
        {
            var stopwatch = Stopwatch.StartNew();

            var uriPath = PathFactory.MakeUriPath(audioMedia.Irn, originalFileExtension, FileDerivativeType.None);
            var destPath = PathFactory.MakeDestPath(audioMedia.Irn, originalFileExtension, FileDerivativeType.None);

            if (ThereAreExistingMedia(destPath))
            {
                audioMedia.File = new MediaFile
                {
                    Uri = uriPath,
                    Size = new FileInfo(destPath).Length
                };

                stopwatch.Stop();
                _log.Trace("Loaded existing audio media in {0} ms", stopwatch.ElapsedMilliseconds);

                return true;
            }

            // Fetch from Imu as we were not able to find local files
            var fileSize = FetchFile(audioMedia.Irn, destPath);
            if (fileSize > 0)
            {
                audioMedia.File = new MediaFile
                {
                    Uri = uriPath,
                    Size = fileSize
                };

                stopwatch.Stop();
                _log.Trace("Created new udio media in {0} ms", stopwatch.ElapsedMilliseconds);

                return true;
            }

            return false;
        }

        public bool Make(ref FileMedia fileMedia, string originalFileExtension)
        {
            var stopwatch = Stopwatch.StartNew();

            var uriPath = PathFactory.MakeUriPath(fileMedia.Irn, originalFileExtension, FileDerivativeType.None);
            var destPath = PathFactory.MakeDestPath(fileMedia.Irn, originalFileExtension, FileDerivativeType.None);

            if (ThereAreExistingMedia(destPath))
            {
                fileMedia.File = new MediaFile
                {
                    Uri = uriPath,
                    Size = new FileInfo(destPath).Length
                };

                stopwatch.Stop();
                _log.Trace("Loaded existing file media in {0} ms", stopwatch.ElapsedMilliseconds);

                return true;
            }

            // Fetch from Imu as we were not able to find local files
            var fileSize = FetchFile(fileMedia.Irn, destPath);
            if (fileSize > 0)
            {
                fileMedia.File = new MediaFile
                {
                    Uri = uriPath,
                    Size = fileSize
                };

                stopwatch.Stop();
                _log.Trace("Created new file media in {0} ms", stopwatch.ElapsedMilliseconds);

                return true;
            }

            return false;
        }

        private long FetchFile(long irn, string destPath)
        {
            try
            {
                using (var imuSession = _imuSessionProvider.CreateInstance("emultimedia"))
                {
                    imuSession.FindKey(irn);
                    var result = imuSession.Fetch("start", 0, -1, new[] { "resource" }).Rows[0];

                    var resource = result.GetMap("resource");

                    if (resource == null)
                        throw new IMuException("MultimediaResourceNotFound");

                    // Load file stream
                    var fileStream = resource["file"] as FileStream;

                    // Save file stream
                    using (var file = File.OpenWrite(destPath))
                    {
                        fileStream.CopyTo(file);
                        fileStream.Dispose();

                        return file.Length;
                    }
                }
            }
            catch (Exception exception)
            {
                if (exception is IMuException && ((IMuException)exception).ID == "MultimediaResourceNotFound")
                {
                    // Error is a known issue that will be picked up in subsequent imports once the data is fixed. So we don't need to re-throw exception.
                    _log.Warn("Multimedia resource was not found, unable to save image at this time {0}, {1}", irn, exception);
                }
                else
                {
                    // Error is unexpected therefore we want the entire import to fail, re-throw the error.
                    _log.Error("Error saving file media {0}, un-recoverable error", irn);
                    throw;
                }
            }

            return 0;
        }

        private bool ThereAreExistingMedia(string destPath)
        {
            // First check to see if we are not overwriting existing data,
            // then if we find existing files matching all of our image media jobs, use the files on disk instead
            if (!bool.Parse(ConfigurationManager.AppSettings["OverwriteExistingMedia"]))
            {
                if (File.Exists(destPath))
                {                    
                    return true;
                }
            }

            return false;
        }
    }
}