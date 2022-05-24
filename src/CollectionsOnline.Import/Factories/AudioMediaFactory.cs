using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Factories;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Imports;
using CollectionsOnline.Import.Infrastructure;
using CollectionsOnline.Import.Models;
using IMu;
using LiteDB;
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
            // TODO: provide base class that better encapsulates functionality between image/file/audio factories
            var stopwatch = Stopwatch.StartNew();
            using (var db = new LiteRepository(new ConnectionString($"Filename={ConfigurationManager.AppSettings["WebSitePath"]}\\content\\media\\media-checksum.db")))
            {
                // Fetch media checksum from db
                var mediaIrn = audioMedia.Irn;
                var mediaChecksum = db.FirstOrDefault<MediaChecksum>(x => x.Irn == mediaIrn);
                
                if (FileExists(ref audioMedia, originalFileExtension, mediaChecksum))
                {
                    stopwatch.Stop();
                    Log.Logger.Debug("Found existing audio {Irn} in {ElapsedMilliseconds} ms", audioMedia.Irn,
                        stopwatch.ElapsedMilliseconds);

                    return true;
                }

                // Fetch fresh media from emu as no existing media found or media fails checksum
                var (fetchIsSuccess, fileSize) = FetchMedia(ref audioMedia, originalFileExtension);
                if (fetchIsSuccess)
                {
                    stopwatch.Stop();
                    Log.Logger.Debug("Completed audio {Irn} ({FileSize}) creation in {ElapsedMilliseconds} ms", audioMedia.Irn,
                        fileSize, stopwatch.ElapsedMilliseconds);

                    // Update or insert image media checksum value in db
                    if (mediaChecksum == null)
                    {
                        db.Insert(new MediaChecksum()
                        {
                            Irn = audioMedia.Irn,
                            Md5Checksum = audioMedia.Md5Checksum
                        });
                    }
                    else
                    {
                        mediaChecksum.Md5Checksum = audioMedia.Md5Checksum;
                        
                        db.Update(mediaChecksum);
                    }

                    return true;
                }

                return false;
            }
        }

        private bool FileExists(ref AudioMedia audioMedia, string originalFileExtension, MediaChecksum mediaChecksum)
        {
            // First check to see if we are not simply overwriting all existing media
            if (bool.Parse(ConfigurationManager.AppSettings["OverwriteExistingMedia"]))
                return false;

            // Check media checksum to see if file has changed
            if (mediaChecksum == null)
            {
                return false;
            }
            else if (mediaChecksum.Md5Checksum != audioMedia.Md5Checksum)
            {
                Log.Logger.Debug("Existing file {Irn} checksum {ExistingChecksum} did not match current file {NewChecksum}", audioMedia.Irn, 
                    mediaChecksum.Md5Checksum, audioMedia.Md5Checksum);
                return false;
            }
            
            // then if we find an existing file, use the files on disk instead
            var destPath = PathFactory.GetDestPath(audioMedia.Irn, originalFileExtension, FileDerivativeType.None);
            if (File.Exists(destPath))
            {
                audioMedia.File = new MediaFile
                {
                    Uri = PathFactory.BuildUriPath(audioMedia.Irn, originalFileExtension, FileDerivativeType.None),
                    Size = new FileInfo(destPath).Length
                };

                return true;
            }
            else
            {
                Log.Logger.Debug("Existing file {Irn} checksum found but file not present", audioMedia.Irn);
            }

            return false;
        }
        
        private (bool, string) FetchMedia(ref AudioMedia audioMedia, string originalFileExtension)
        {
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

                    // Save file stream
                    using (var file = File.Open(destPath, FileMode.Create, FileAccess.Write))
                    {
                        var fileSize = fileStream.Length.BytesToString();
                        fileStream.CopyTo(file);
                        fileStream.Dispose();

                        audioMedia.File = new MediaFile
                        {
                            Uri = PathFactory.BuildUriPath(audioMedia.Irn, originalFileExtension, FileDerivativeType.None),
                            Size = file.Length
                        };

                        return (true, fileSize);
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

            return (false, null);
        }
    }
}