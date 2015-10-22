using System;
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
using Raven.Client;
using Raven.Client.Linq;
using Serilog;

namespace CollectionsOnline.Import.Factories
{
    public class AudioMediaFactory : IAudioMediaFactory
    {
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
                Log.Logger.Debug("Found existing audio {Irn} in {ElapsedMilliseconds} ms", audioMedia.Irn, stopwatch.ElapsedMilliseconds);

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

                    var destPath = PathFactory.MakeDestPath(audioMedia.Irn, originalFileExtension, FileDerivativeType.None);

                    // Create destination folder
                    var directoryInfo = new DirectoryInfo(Path.GetDirectoryName(destPath));
                    if (!directoryInfo.Exists)
                        directoryInfo.Create();

                    // Save file stream
                    using (var file = File.OpenWrite(destPath))
                    {
                        fileStream.CopyTo(file);
                        fileStream.Dispose();

                        audioMedia.File = new MediaFile
                        {
                            Uri = PathFactory.MakeUriPath(audioMedia.Irn, originalFileExtension, FileDerivativeType.None),
                            Size = file.Length
                        };

                        stopwatch.Stop();
                        Log.Logger.Debug("Completed audio {Irn} creation in {ElapsedMilliseconds} ms", audioMedia.Irn, stopwatch.ElapsedMilliseconds);

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is IMuException && ((IMuException)ex).ID == "MultimediaResolutionNotFound")
                {
                    Log.Logger.Warning(ex, "Multimedia resolution not found, unable to save audio {Irn}", audioMedia.Irn);
                }
                else if (ex is IMuException && ((IMuException)ex).ID == "MultimediaResourceNotFound")
                {
                    Log.Logger.Warning(ex, "Multimedia resource not found, unable to save audio {Irn}", audioMedia.Irn);
                }
                else
                {
                    // Error is unexpected therefore we want the entire import to fail
                    Log.Logger.Fatal(ex, "Unexpected error occured creating audio {Irn}", audioMedia.Irn);
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
                    return false;
                
                // If existing media checksum does not match the one from emu we need to save file
                if (result != null && result.Md5Checksum != audioMedia.Md5Checksum)
                {
                    Log.Logger.Warning("Existing audio {Irn} found but checksum {ExistingChecksum} did not match new audio {NewChecksum}", audioMedia.Irn, result.Md5Checksum, audioMedia.Md5Checksum);
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