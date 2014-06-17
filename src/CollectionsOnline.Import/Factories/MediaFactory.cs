using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Utilities;
using ImageResizer;
using IMu;

namespace CollectionsOnline.Import.Factories
{
    public class MediaFactory : IMediaFactory
    {
        private readonly IMediaHelper _mediaHelper;

        public MediaFactory(IMediaHelper mediaHelper)
        {
            _mediaHelper = mediaHelper;
        }

        public Media Make(Map map)
        {
            if (map != null &&
                string.Equals(map.GetString("AdmPublishWebNoPassword"), "yes", StringComparison.OrdinalIgnoreCase) &&
                map.GetStrings("MdaDataSets_tab").Contains(Constants.ImuMultimediaQueryString) &&
                string.Equals(map.GetString("MulMimeType"), "image", StringComparison.OrdinalIgnoreCase))
            {
                var irn = long.Parse(map.GetString("irn"));

                var url = PathFactory.MakeUrlPath(irn, FileFormatType.Jpg, "thumb");
                var thumbResizeSettings = new ResizeSettings
                {
                    Format = FileFormatType.Jpg.ToString(),
                    Height = 365,
                    Width = 365,
                    Mode = FitMode.Crop,
                    PaddingColor = Color.White,
                    Quality = 65
                };

                if (_mediaHelper.Save(irn, FileFormatType.Jpg, thumbResizeSettings, "thumb"))
                {
                    return new Media
                    {
                        Irn = irn,
                        DateModified =
                            DateTime.ParseExact(
                                string.Format("{0} {1}", map.GetString("AdmDateModified"),
                                    map.GetString("AdmTimeModified")), "dd/MM/yyyy HH:mm",
                                new CultureInfo("en-AU")),
                        Title = map.GetString("MulTitle"),
                        AlternateText = map.GetString("DetAlternateText"),
                        Type = map.GetString("MulMimeType"),
                        Url = url
                    };
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