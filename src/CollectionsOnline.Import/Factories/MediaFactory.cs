using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Extensions;
using ImageProcessor.Imaging;
using IMu;
using Serilog;

namespace CollectionsOnline.Import.Factories
{
    public class MediaFactory : IMediaFactory
    {
        private readonly IImageMediaFactory _imageMediaFactory;
        private readonly IFileMediaFactory _fileMediaFactory;
        private readonly IAudioMediaFactory _audioMediaFactory;
        private readonly IVideoMediaFactory _videoMediaFactory;

        public MediaFactory(
            IImageMediaFactory imageMediaFactory,
            IFileMediaFactory fileMediaFactory,
            IAudioMediaFactory audioMediaFactory,
            IVideoMediaFactory videoMediaFactory)
        {
            _imageMediaFactory = imageMediaFactory;
            _fileMediaFactory = fileMediaFactory;
            _audioMediaFactory = audioMediaFactory;
            _videoMediaFactory = videoMediaFactory;
        }

        public Media Make(Map map, ResizeMode? thumbnailResizeMode)
        {
            if (map != null &&
                string.Equals(map.GetEncodedString("AdmPublishWebNoPassword"), "yes", StringComparison.OrdinalIgnoreCase) &&
                map.GetEncodedStrings("MdaDataSets_tab").Contains(Constants.ImuMultimediaQueryString))
            {
                var irn = long.Parse(map.GetEncodedString("irn"));
                
                var dateModified = DateTime.ParseExact(
                    string.Format("{0} {1}", map.GetEncodedString("AdmDateModified"), map.GetEncodedString("AdmTimeModified")), 
                    "dd/MM/yyyy HH:mm", 
                    new CultureInfo("en-AU")).ToUniversalTime();

                var mimeType = map.GetEncodedString("MulMimeType");
                var identifier = map.GetEncodedString("MulIdentifier");

                var captionMap = map.GetMaps("metadata").FirstOrDefault( x => string.Equals(x.GetEncodedString("MdaElement_tab"), "dcTitle", StringComparison.OrdinalIgnoreCase) && string.Equals(x.GetEncodedString("MdaQualifier_tab"), "Caption.COL"));
                var caption = captionMap != null ? captionMap.GetEncodedString("MdaFreeText_tab") : map.GetEncodedString("MulTitle");

                var creators = map.GetEncodedStrings("RigCreator_tab");
                var sources = map.GetEncodedStrings("RigSource_tab");
                var credit = map.GetEncodedString("RigAcknowledgementCredit");
                var rightsStatement = map.GetEncodedString("RigCopyrightStatement");
                var rightsStatus = map.GetEncodedString("RigCopyrightStatus");
                var licence = map.GetEncodedString("RigLicence");
                var licenceDetails = map.GetEncodedString("RigLicenceDetails");

                var md5Checksum = map.GetEncodedString("ChaMd5Sum");
                
                // Handle images
                if (string.Equals(mimeType, "image", StringComparison.OrdinalIgnoreCase))
                {
                    var alternativeText = map.GetEncodedString("DetAlternateText");

                    var imageMedia = new ImageMedia
                    {
                        Irn = irn,
                        DateModified = dateModified,
                        Caption = caption,
                        AlternativeText = alternativeText,
                        Creators = creators,
                        Sources = sources,
                        Credit = credit,
                        RightsStatement = rightsStatement,
                        RightsStatus = rightsStatus,
                        Licence = licence,
                        LicenceDetails = licenceDetails,
                        Md5Checksum = md5Checksum
                    };

                    if (_imageMediaFactory.Make(ref imageMedia, thumbnailResizeMode))
                    {
                        return imageMedia;
                    }
                }

                // Handle files
                if (string.Equals(mimeType, "application", StringComparison.OrdinalIgnoreCase))
                {
                    var fileMedia = new FileMedia
                    {
                        Irn = irn,
                        DateModified = dateModified,
                        Caption = caption,
                        Creators = creators,
                        Sources = sources,
                        Credit = credit,
                        RightsStatement = rightsStatement,
                        RightsStatus = rightsStatus,
                        Licence = licence,
                        LicenceDetails = licenceDetails,
                        Md5Checksum = md5Checksum
                    };

                    if (_fileMediaFactory.Make(ref fileMedia, Path.GetExtension(identifier)))
                    {
                        return fileMedia;
                    }
                }

                // Handle audio
                if (string.Equals(mimeType, "audio", StringComparison.OrdinalIgnoreCase))
                {
                    var audioMedia = new AudioMedia
                    {
                        Irn = irn,
                        DateModified = dateModified,
                        Caption = caption,
                        Creators = creators,
                        Sources = sources,
                        Credit = credit,
                        RightsStatement = rightsStatement,
                        RightsStatus = rightsStatus,
                        Licence = licence,
                        LicenceDetails = licenceDetails,
                        Md5Checksum = md5Checksum,
                        Thumbnail = new ImageMediaFile
                        {
                            Uri = "/content/img/img-audio-thumbnail.png",
                            Height = 250,
                            Width = 250,
                            Size = 2841
                        }
                    };

                    if (_audioMediaFactory.Make(ref audioMedia, Path.GetExtension(identifier)))
                    {
                        return audioMedia;
                    }
                }

                // Handle video
                if (map.GetEncodedStrings("ChaRepository_tab").Contains(Constants.ImuVideoQueryString))
                {
                    var videoMedia = new VideoMedia
                    {
                        Irn = irn,
                        DateModified = dateModified,
                        Caption = caption,
                        AlternativeText = caption,
                        Creators = creators,
                        Sources = sources,
                        Credit = credit,
                        RightsStatement = rightsStatement,
                        RightsStatus = rightsStatus,
                        Licence = licence,
                        LicenceDetails = licenceDetails,
                        Uri = identifier
                    };

                    if (_videoMediaFactory.Make(ref videoMedia))
                    {
                        return videoMedia;
                    }
                }

                // Handle uris
                if (map.GetEncodedStrings("ChaRepository_tab").Contains(Constants.ImuUriQueryString))
                {
                    var uriMedia = new UriMedia
                    {
                        Irn = irn,
                        DateModified = dateModified,
                        Caption = caption,
                        Creators = creators,
                        Sources = sources,
                        Credit = credit,
                        RightsStatement = rightsStatement,
                        RightsStatus = rightsStatus,
                        Licence = licence,
                        LicenceDetails = licenceDetails,
                        Uri = identifier
                    };

                    Log.Logger.Debug("Completed uri {Irn} creation", uriMedia.Irn);

                    return uriMedia;
                }
            }

            return null;
        }

        public IList<Media> Make(IEnumerable<Map> maps, ResizeMode? thumbnailResizeMode)
        {
            var medias = new List<Media>();

            // Group by mmr irn
            var groupedMediaMaps = maps
                .Where(x => x != null)
                .GroupBy(x => x.GetEncodedString("irn"))
                .ToList();

            // Find and log duplicate mmr irns
            var duplicateMediaIrns = groupedMediaMaps
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();
            if(duplicateMediaIrns.Any())
                Log.Logger.Warning("Duplicate MMR Irns detected {@DuplicateMediaIrns}", duplicateMediaIrns);

            // Select only distinct mmr maps
            var distinctMediaMaps = groupedMediaMaps.Select(x => x.First());

            // Create medias
            medias.AddRange(distinctMediaMaps.Select(x => Make(x, thumbnailResizeMode)).Where(x => x != null));

            return medias;
        }
    }
}