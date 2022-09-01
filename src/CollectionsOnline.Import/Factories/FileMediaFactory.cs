using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Factories;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Infrastructure;
using CollectionsOnline.Import.Models;
using IMu;
using LiteDB;
using Serilog;

namespace CollectionsOnline.Import.Factories
{
    public class FileMediaFactory : IFileMediaFactory
    {
        private readonly IImuSessionProvider _imuSessionProvider;
        private readonly ILiteDatabase _liteDatabase;

        public FileMediaFactory(
            IImuSessionProvider imuSessionProvider,
            ILiteDatabase liteDatabase)
        {
            _imuSessionProvider = imuSessionProvider;
            _liteDatabase = liteDatabase;
        }

        public bool Make(ref FileMedia fileMedia, string originalFileExtension)
        {
            // TODO: provide base class that better encapsulates functionality between image/file/audio factories
            var mediaIrn = fileMedia.Irn;
            
            // Fetch media checksum from db
            var stopwatch = Stopwatch.StartNew();
            
            var mediaChecksums = _liteDatabase.GetCollection<MediaChecksum>();
            mediaChecksums.EnsureIndex(x => x.Irn, true);
            var mediaChecksum = mediaChecksums.FindOne(x => x.Irn == mediaIrn);
            
            // Compare checksum then check files on filesystem 
            if (FileExists(ref fileMedia, originalFileExtension, mediaChecksum))
            {
                stopwatch.Stop();
                Log.Logger.Debug("Found existing file {Irn} in {ElapsedMilliseconds} ms", mediaIrn, stopwatch.ElapsedMilliseconds);
                return true;
            }

            // Fetch fresh media from emu as no existing media found or media fails checksum
            var (fetchIsSuccess, fileSize) = FetchMedia(ref fileMedia, originalFileExtension);
            if (fetchIsSuccess)
            {
                // Update or insert image media checksum value in db
                if (mediaChecksum == null)
                {
                    mediaChecksums.Insert(new MediaChecksum()
                    {
                        Irn = fileMedia.Irn,
                        Md5Checksum = fileMedia.Md5Checksum
                    });
                }
                else
                {
                    mediaChecksum.Md5Checksum = fileMedia.Md5Checksum;
                    
                    mediaChecksums.Update(mediaChecksum);
                }

                stopwatch.Stop();
                Log.Logger.Debug("Completed file {Irn} ({FileSize}) creation in {ElapsedMilliseconds} ms", fileMedia.Irn,
                    fileSize, stopwatch.ElapsedMilliseconds);
                return true;
            }

            return false;
        }

        private bool FileExists(ref FileMedia fileMedia, string originalFileExtension, MediaChecksum mediaChecksum)
        {
            // First check to see if we are not simply overwriting all existing media
            if (bool.Parse(ConfigurationManager.AppSettings["OverwriteExistingMedia"]))
                return false;
            
            // Check media checksum to see if file has changed
            if (mediaChecksum == null)
            {
                return false;
            }
            else if (mediaChecksum.Md5Checksum != fileMedia.Md5Checksum)
            {
                Log.Logger.Debug("Existing file {Irn} checksum {ExistingChecksum} did not match current file {NewChecksum}", fileMedia.Irn, 
                    mediaChecksum.Md5Checksum, fileMedia.Md5Checksum);
                return false;
            }

            // then if we find an existing file, use the files on disk instead
            var destPath = PathFactory.GetDestPath(fileMedia.Irn, originalFileExtension, FileDerivativeType.None);
            if (File.Exists(destPath))
            {
                fileMedia.File = new MediaFile
                {
                    Uri = PathFactory.BuildUriPath(fileMedia.Irn, originalFileExtension, FileDerivativeType.None),
                    Size = new FileInfo(destPath).Length
                };
                
                Log.Logger.Debug("Existing file {Irn} checksum matches, file present, FileMedia re-created", fileMedia.Irn);
                return true;
            }
            
            Log.Logger.Debug("Existing file {Irn} checksum matches but file not present", fileMedia.Irn);
            return false;
        }
        
        private (bool, string) FetchMedia(ref FileMedia fileMedia, string originalFileExtension)
        {
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

                    var destPath = PathFactory.MakeDestPath(fileMedia.Irn, originalFileExtension, FileDerivativeType.None);

                    // Save file stream
                    using (var file = File.Open(destPath, FileMode.Create, FileAccess.Write))
                    {
                        var fileSize = fileStream.Length.BytesToString();
                        fileStream.CopyTo(file);
                        fileStream.Dispose();

                        fileMedia.File = new MediaFile
                        {
                            Uri = PathFactory.BuildUriPath(fileMedia.Irn, originalFileExtension, FileDerivativeType.None),
                            Size = file.Length
                        };

                        return (true, fileSize);
                    }
                }
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case IMuException exception when exception.ID == "MultimediaResolutionNotFound":
                        Log.Logger.Warning(exception, "Multimedia resolution not found, unable to save file {Irn}", fileMedia.Irn);
                        break;
                    case IMuException exception when exception.ID == "MultimediaResourceNotFound":
                        Log.Logger.Warning(exception, "Multimedia resource not found, unable to save file {Irn}", fileMedia.Irn);
                        break;
                    default:
                        // Error is unexpected therefore we want the entire import to fail
                        Log.Logger.Fatal(ex, "Unexpected error occured creating file {Irn}", fileMedia.Irn);
                        throw;
                }
            }

            return (false, null);
        }
    }
}