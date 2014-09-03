using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Models;
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
                string.Equals(map.GetString("AdmPublishWebNoPassword"), "yes", StringComparison.OrdinalIgnoreCase) &&
                map.GetStrings("MdaDataSets_tab").Contains(Constants.ImuMultimediaQueryString))
            {
                var irn = long.Parse(map.GetString("irn"));

                // Handle images
                if (string.Equals(map.GetString("MulMimeType"), "image", StringComparison.OrdinalIgnoreCase))
                {
                    var imageMedia = new ImageMedia
                    {
                        Irn = irn,
                        DateModified = DateTime.ParseExact(string.Format("{0} {1}", map.GetString("AdmDateModified"), map.GetString("AdmTimeModified")), "dd/MM/yyyy HH:mm", new CultureInfo("en-AU")),
                        Title = map.GetString("MulTitle"),
                        AlternateText = map.GetString("DetAlternateText"),
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