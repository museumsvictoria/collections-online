using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Extensions;
using IMu;

namespace CollectionsOnline.Import.Factories
{
    public class MediaFactory : IMediaFactory
    {
        private readonly IImageMediaFactory _imageMediaFactory;

        public MediaFactory(IImageMediaFactory imageMediaFactory)
        {
            _imageMediaFactory = imageMediaFactory;
        }

        public Media Make(Map map)
        {
            if (map != null &&
                string.Equals(map.GetEncodedString("AdmPublishWebNoPassword"), "yes", StringComparison.OrdinalIgnoreCase) &&
                map.GetEncodedStrings("MdaDataSets_tab").Contains(Constants.ImuMultimediaQueryString))
            {
                var irn = long.Parse(map.GetEncodedString("irn"));
                var dateModified = DateTime.ParseExact(string.Format("{0} {1}", map.GetEncodedString("AdmDateModified"), map.GetEncodedString("AdmTimeModified")), "dd/MM/yyyy HH:mm", new CultureInfo("en-AU"));

                var captionMap = map.GetMaps("metadata").FirstOrDefault( x => string.Equals(x.GetEncodedString("MdaElement_tab"), "dcTitle", StringComparison.OrdinalIgnoreCase) && string.Equals(x.GetEncodedString("MdaQualifier_tab"), "Caption.COL"));
                var caption = captionMap != null ? captionMap.GetEncodedString("MdaFreeText_tab") : map.GetEncodedString("MulTitle");

                var creators = map.GetEncodedStrings("RigCreator_tab");
                var sources = map.GetEncodedStrings("RigSource_tab");
                var credit = map.GetEncodedString("RigAcknowledgementCredit");
                var rightsStatement = map.GetEncodedString("RigCopyrightStatement");
                var rightsStatus = map.GetEncodedString("RigCopyrightStatus");
                var licence = map.GetEncodedString("RigLicence");
                var licenceDetails = map.GetEncodedString("RigLicenceDetails");
                
                // Handle images
                if (string.Equals(map.GetEncodedString("MulMimeType"), "image", StringComparison.OrdinalIgnoreCase))
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
                        LicenceDetails = licenceDetails
                    };

                    if (_imageMediaFactory.Make(ref imageMedia))
                    {
                        return imageMedia;
                    }
                }
            }

            return null;
        }

        public IList<Media> Make(IEnumerable<Map> maps)
        {
            var medias = new List<Media>();

            medias.AddRange(maps.Select(Make).Where(x => x != null));

            return medias;
        }
    }
}