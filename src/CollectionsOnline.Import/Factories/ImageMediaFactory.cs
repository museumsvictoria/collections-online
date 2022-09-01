using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Extensions;
using CollectionsOnline.Import.Infrastructure;
using ImageMagick;
using IMu;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Factories;
using CollectionsOnline.Import.Models;
using LiteDB;
using Serilog;

namespace CollectionsOnline.Import.Factories
{
    public class ImageMediaFactory : IImageMediaFactory
    {
        private readonly IImuSessionProvider _imuSessionProvider;
        private readonly IList<ImageMediaJob> _imageMediaJobs;
        private readonly ILiteDatabase _liteDatabase;

        public ImageMediaFactory(
            IImuSessionProvider imuSessionProvider,
            ILiteDatabase liteDatabase)
        {
            _imuSessionProvider = imuSessionProvider;
            _liteDatabase = liteDatabase;

            // Build a list the various image conversions used in the application
            _imageMediaJobs = new List<ImageMediaJob>
            {
                new ImageMediaJob
                {                    
                    FileDerivativeType = FileDerivativeType.Large,
                    Transform = (imageMedia, magickImage, map) =>
                    {
                        AddImageProfile(imageMedia, magickImage, true);
                        
                        magickImage.Format = MagickFormat.Jpeg;
                        magickImage.Quality = 86;
                        magickImage.FilterType = FilterType.Lanczos;
                        magickImage.ColorSpace = ColorSpace.sRGB;
                        magickImage.Resize(new MagickGeometry(3000) { Greater = true });
                        magickImage.UnsharpMask(0.5, 0.5, 0.6, 0.025);

                        return magickImage;
                    }
                },
                new ImageMediaJob
                {
                    FileDerivativeType = FileDerivativeType.Thumbnail,
                    Transform = (imageMedia, magickImage, map) =>
                    {
                        AddImageProfile(imageMedia, magickImage);
                        
                        magickImage.Format = MagickFormat.Jpeg;
                        magickImage.Quality = 70;
                        magickImage.FilterType = FilterType.Lanczos;
                        magickImage.ColorSpace = ColorSpace.sRGB;
                        
                        if (NeedsPadding(map))
                        {
                            magickImage.Resize(new MagickGeometry(250));
                            magickImage.Extent(new MagickGeometry(250), Gravity.Center, MagickColors.White);
                        }
                        else
                        {
                            magickImage.Resize(new MagickGeometry(250) { FillArea = true });
                            magickImage.Crop(250, 250, Gravity.Center);
                        }
                        
                        magickImage.UnsharpMask(0.5, 0.5, 0.5, 0.025);

                        return magickImage;
                    }
                },
                new ImageMediaJob
                {
                    FileDerivativeType = FileDerivativeType.Small,
                    Transform = (imageMedia, magickImage, map) =>
                    {
                        AddImageProfile(imageMedia, magickImage, true);

                        magickImage.Format = MagickFormat.Jpeg;
                        magickImage.Quality = 76;
                        magickImage.FilterType = FilterType.Lanczos;
                        magickImage.ColorSpace = ColorSpace.sRGB;
                        magickImage.Resize(new MagickGeometry(0, 500));
                        magickImage.UnsharpMask(0.5, 0.5, 0.6, 0.025);

                        return magickImage;
                    }
                },
                new ImageMediaJob
                {
                    FileDerivativeType = FileDerivativeType.Medium,
                    Transform = (imageMedia, magickImage, map) =>
                    {
                        AddImageProfile(imageMedia, magickImage, true);

                        magickImage.Format = MagickFormat.Jpeg;
                        magickImage.Quality = 76;
                        magickImage.FilterType = FilterType.Lanczos;
                        magickImage.ColorSpace = ColorSpace.sRGB;
                        magickImage.Resize(new MagickGeometry(1500) { Greater = true });
                        magickImage.UnsharpMask(0.5, 0.5, 0.6, 0.025);

                        return magickImage;
                    }
                }
            };
        }

        public bool Make(ref ImageMedia imageMedia)
        {
            // TODO: provide base class that better encapsulates functionality between image/file/audio factories
            var mediaIrn = imageMedia.Irn;
            
            // Fetch media checksum from db
            var stopwatch = Stopwatch.StartNew();
            
            var mediaChecksums = _liteDatabase.GetCollection<MediaChecksum>();
            mediaChecksums.EnsureIndex(x => x.Irn, true);
            var mediaChecksum = mediaChecksums.FindOne(x => x.Irn == mediaIrn); 
            
            // Try to find existing media in media checksum collection to compare checksum then check files on filesystem 
            if (FileExists(ref imageMedia, mediaChecksum))
            {
                stopwatch.Stop();
                Log.Logger.Debug("Found existing image {Irn} in {ElapsedMilliseconds} ms", mediaIrn, stopwatch.ElapsedMilliseconds);
                return true;
            }
            
            // Fetch fresh media from emu as no existing media found or media fails checksum
            var (fetchIsSuccess, fileSize) = FetchMedia(ref imageMedia);
            if (fetchIsSuccess)
            {
                // Update or insert image media checksum value in db
                if (mediaChecksum == null)
                {
                    mediaChecksums.Insert(new MediaChecksum()
                    {
                        Irn = imageMedia.Irn,
                        Md5Checksum = imageMedia.Md5Checksum
                    });
                }
                else
                {
                    mediaChecksum.Md5Checksum = imageMedia.Md5Checksum;
                    
                    mediaChecksums.Update(mediaChecksum);
                }
                
                stopwatch.Stop();
                Log.Logger.Debug("Completed image {Irn} ({FileSize}) creation in {ElapsedMilliseconds} ms", mediaIrn,
                    fileSize, stopwatch.ElapsedMilliseconds);
                return true;
            }

            return false;
        }
        
        private bool FileExists(ref ImageMedia imageMedia, MediaChecksum mediaChecksum)
        {
            var mediaIrn = imageMedia.Irn;

            // First check to see if we are not simply overwriting all existing media
            if (bool.Parse(ConfigurationManager.AppSettings["OverwriteExistingMedia"])) 
                return false;
            
            // Check media checksum to see if file has changed
            if (mediaChecksum == null)
            {
                return false;
            }
            else if (mediaChecksum.Md5Checksum != imageMedia.Md5Checksum)
            {
                Log.Logger.Debug("Existing image {Irn} checksum {ExistingChecksum} did not match current image {NewChecksum}", imageMedia.Irn, 
                    mediaChecksum.Md5Checksum, imageMedia.Md5Checksum);
                return false;
            }
            
            // then check whether media exists on disk for all media jobs
            if (!_imageMediaJobs.All(x => File.Exists(PathFactory.GetDestPath(mediaIrn, ".jpg", x.FileDerivativeType))))
            {
                Log.Logger.Debug("Existing image {Irn} checksum matches but files not present, creating new images", imageMedia.Irn);
                return false;
            }

            // Recreate ImageMedia by loading all file derivatives
            foreach (var imageMediaJob in _imageMediaJobs)
            {
                var destPath = PathFactory.GetDestPath(mediaIrn, ".jpg", imageMediaJob.FileDerivativeType);

                using (var image = new MagickImage(destPath))
                {
                    // Set property via reflection (ImageMediaFile properties are used instead of a collection due to Raven Indexing)
                    typeof(ImageMedia).GetProperties().First(x =>
                            x.PropertyType == typeof(ImageMediaFile) &&
                            x.Name == imageMediaJob.FileDerivativeType.ToString())
                        .SetValue(imageMedia, new ImageMediaFile
                        {
                            Uri = PathFactory.BuildUriPath(imageMedia.Irn, ".jpg",
                                imageMediaJob.FileDerivativeType),
                            Size = new FileInfo(destPath).Length,
                            Width = image.Width,
                            Height = image.Height
                        });
                }
            }
            
            Log.Logger.Debug("Existing image {Irn} checksum matches, files present, ImageMedia re-created", imageMedia.Irn);
            return true;
        }
        
        private (bool, string) FetchMedia(ref ImageMedia imageMedia)
        {
            try
            {
                using (var imuSession = _imuSessionProvider.CreateInstance("emultimedia"))
                {
                    imuSession.FindKey(imageMedia.Irn);
                    var result = imuSession.Fetch("start", 0, -1, Columns).Rows[0];

                    var resource = result.GetMap("resource");
                    
                    if (resource == null)
                        throw new IMuException("MultimediaResourceNotFound");
                    
                    // Load file stream
                    var fileStream = resource["file"] as FileStream;

                    using (var originalImage = new MagickImage(fileStream))
                    {
                        foreach (var imageMediaJob in _imageMediaJobs)
                        {
                            var destPath = PathFactory.MakeDestPath(imageMedia.Irn, ".jpg", imageMediaJob.FileDerivativeType);
                            var uriPath = PathFactory.BuildUriPath(imageMedia.Irn, ".jpg", imageMediaJob.FileDerivativeType);

                            using (var image =
                                   imageMediaJob.Transform(imageMedia, originalImage.Clone() as MagickImage, result))
                            {
                                // Write image to disk
                                image.Write(destPath);

                                // Set property via reflection (ImageMediaFile properties are used instead of a collection due to Raven Indexing)
                                typeof(ImageMedia)
                                    .GetProperties()
                                    .First(x => x.PropertyType == typeof(ImageMediaFile) &&
                                                x.Name == imageMediaJob.FileDerivativeType.ToString())
                                    .SetValue(imageMedia, new ImageMediaFile
                                    {
                                        Uri = uriPath,
                                        Size = new FileInfo(destPath).Length,
                                        Width = image.Width,
                                        Height = image.Height
                                    });
                            }
                        }
                    }

                    // Image media successfully saved to disk
                    return (true, fileStream.Length.BytesToString());
                }
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case IMuException exception when exception.ID == "MultimediaResolutionNotFound":
                        Log.Logger.Warning(exception, "Multimedia resolution not found, unable to save image {Irn}", imageMedia.Irn);
                        break;
                    case IMuException exception when exception.ID == "MultimediaResourceNotFound":
                        Log.Logger.Warning(exception, "Multimedia resource not found, unable to save image {Irn}", imageMedia.Irn);
                        break;
                    case MagickCoderErrorException _:
                    case MagickCorruptImageErrorException _:
                        Log.Logger.Warning(ex, "Multimedia resource appears to be corrupt {Irn}", imageMedia.Irn);
                        break;
                    default:
                        // Error is unexpected therefore we want the entire import to fail
                        Log.Logger.Fatal(ex, "Unexpected error occured creating image {Irn}", imageMedia.Irn);
                        throw;
                }
            }
            
            return (false, null);
        }

        private string[] Columns
        {
            get
            {
                return new[]
                {
                    "resource",
                    "catmedia=<ecatalogue:MulMultiMediaRef_tab>.(irn,MdaDataSets_tab,ColScientificGroup)",
                    "narmedia=<enarratives:MulMultiMediaRef_tab>.(irn,DetPurpose_tab)",
                    "parmedia=<eparties:MulMultiMediaRef_tab>.(narmedia=<enarratives:NarAuthorsRef_tab>.(irn,DetPurpose_tab))"
                };
            }
        }

        private bool NeedsPadding(Map map)
        {
            // Pad media if we find any catalogue records that are marked specimen and are zoology records or we find media is used on a species record
            if (map.GetMaps("catmedia").Any(x => x != null &&
                x.GetEncodedStrings("MdaDataSets_tab").Contains(Constants.ImuSpecimenQueryString) &&
                x.GetEncodedString("ColScientificGroup").Contains("zoology", StringComparison.OrdinalIgnoreCase)) ||
                map.GetMaps("narmedia").Any(x => x != null &&
                x.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuSpeciesQueryString)))
                return true;

            return false;
        }

        private void AddImageProfile(ImageMedia imageMedia, MagickImage magickImage, bool addIptcProfile = false)
        {
            // Save profiles if there are any
            var profile = magickImage.GetColorProfile();

            // Strip exif and any profiles
            magickImage.Strip();

            // Add original profile back
            if (profile != null)
                magickImage.SetProfile(profile);

            if (addIptcProfile)
            {
                var iptcProfile = new IptcProfile();

                iptcProfile.SetValue(IptcTag.CopyrightNotice, imageMedia.Licence.Name);
                
                if (!string.IsNullOrWhiteSpace(imageMedia.Credit))
                    iptcProfile.SetValue(IptcTag.Credit, imageMedia.Credit);

                if(imageMedia.Sources.Any())
                    iptcProfile.SetValue(IptcTag.Source, imageMedia.Sources.Concatenate(", "));

                magickImage.SetProfile(iptcProfile);
            }            
        }

        private class ImageMediaJob
        {
            public FileDerivativeType FileDerivativeType { get; set; }

            public Func<ImageMedia, MagickImage, Map, MagickImage> Transform { get; set; }
        }
    }
}