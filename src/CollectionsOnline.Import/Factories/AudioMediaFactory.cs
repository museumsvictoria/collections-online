﻿using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Imports;
using CollectionsOnline.Import.Infrastructure;
using IMu;
using NLog;
using Raven.Client;
using Raven.Client.Linq;

namespace CollectionsOnline.Import.Factories
{
    public class AudioMediaFactory : IAudioMediaFactory
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly IDocumentStore _documentStore;
        private readonly IImuSessionProvider _imuSessionProvider;

        public AudioMediaFactory(
            IDocumentStore documentStore,
            IImuSessionProvider imuSessionProvider)
        {
            _documentStore = documentStore;
            _imuSessionProvider = imuSessionProvider;
        }

        public bool Make(ref AudioMedia audioMedia, string originalFileExtension)
        {
            var stopwatch = Stopwatch.StartNew();

            if (FileExists(ref audioMedia, originalFileExtension))
            {
                stopwatch.Stop();
                _log.Trace("Loaded existing audio media in {0} ms", stopwatch.ElapsedMilliseconds);

                return true;
            }

            // Fetch from Imu as we were not able to find local files
            try
            {
                using (var imuSession = _imuSessionProvider.CreateInstance("emultimedia"))
                {
                    imuSession.FindKey(audioMedia.Irn);
                    var result = imuSession.Fetch("start", 0, -1, new[] { "resource" }).Rows[0];

                    var resource = result.GetMap("resource");

                    if (resource == null)
                        throw new IMuException("MultimediaResourceNotFound");

                    // Load file stream
                    var fileStream = resource["file"] as FileStream;

                    // Save file stream
                    using (var file = File.OpenWrite(PathFactory.MakeDestPath(audioMedia.Irn, originalFileExtension, FileDerivativeType.None)))
                    {
                        fileStream.CopyTo(file);
                        fileStream.Dispose();

                        audioMedia.File = new MediaFile
                        {
                            Uri = PathFactory.MakeUriPath(audioMedia.Irn, originalFileExtension, FileDerivativeType.None),
                            Size = file.Length
                        };

                        stopwatch.Stop();
                        _log.Trace("Created new audio media in {0} ms", stopwatch.ElapsedMilliseconds);

                        return true;
                    }
                }
            }
            catch (Exception exception)
            {
                if (exception is IMuException && ((IMuException)exception).ID == "MultimediaResourceNotFound")
                {
                    // Error is a known issue that will be picked up in subsequent imports once the data is fixed. So we don't need to re-throw exception.
                    _log.Warn("Multimedia resource was not found, unable to save image at this time {0}, {1}", audioMedia.Irn, exception);
                }
                else
                {
                    // Error is unexpected therefore we want the entire import to fail, re-throw the error.
                    _log.Error("Error saving file media {0}, un-recoverable error", audioMedia.Irn);
                    throw;
                }
            }           

            return false;
        }

        private bool FileExists(ref AudioMedia audioMedia, string originalFileExtension)
        {
            // First check to see if we are not simply overwriting all existing media
            if (bool.Parse(ConfigurationManager.AppSettings["OverwriteExistingMedia"]))
                return false;

            var mediaIrn = audioMedia.Irn;

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
                {
                    _log.Trace("No existing audio media found matching mmr irn... fetching from EMu");
                    return false;
                }
                // If existing media checksum does not match the one from emu we need to save file
                if (result != null && result.Md5Checksum != audioMedia.Md5Checksum)
                {
                    _log.Trace("Existing audio media found however checksum doesnt match... fetching from EMu (existing:{0}, new:{1})", result.Md5Checksum, audioMedia.Md5Checksum);
                    return false;
                }
            }

            var destPath = PathFactory.MakeDestPath(audioMedia.Irn, originalFileExtension, FileDerivativeType.None);

            // then if we find an existing file, use the files on disk instead
            if (File.Exists(destPath))
            {
                audioMedia.File = new MediaFile
                {
                    Uri = PathFactory.MakeUriPath(audioMedia.Irn, originalFileExtension, FileDerivativeType.None),
                    Size = new FileInfo(destPath).Length
                };

                return true;
            }

            return false;
        }
    }
}