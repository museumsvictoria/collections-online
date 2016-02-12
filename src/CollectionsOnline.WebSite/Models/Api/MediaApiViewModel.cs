using System;
using System.Collections.Generic;

namespace CollectionsOnline.WebSite.Models.Api
{
    public class MediaApiViewModel
    {
        public DateTime DateModified { get; set; }

        public string Caption { get; set; }

        public IList<string> Creators { get; set; }

        public IList<string> Sources { get; set; }

        public string Credit { get; set; }

        public string RightsStatement { get; set; }

        public string RightsStatus { get; set; }

        public string Licence { get; set; }

        public string LicenceDetails { get; set; }
    }

    public class ImageMediaApiViewModel : MediaApiViewModel
    {
        public string AlternativeText { get; set; }

        public ImageMediaFileApiViewModel Original { get; set; }

        public ImageMediaFileApiViewModel Thumbnail { get; set; }

        public ImageMediaFileApiViewModel Medium { get; set; }

        public ImageMediaFileApiViewModel Large { get; set; }
    }

    public class VideoMediaApiViewModel : MediaApiViewModel
    {
        public string AlternativeText { get; set; }

        public string Uri { get; set; }

        public string VideoId { get; set; }

        public ImageMediaFileApiViewModel Thumbnail { get; set; }

        public ImageMediaFileApiViewModel Medium { get; set; }
    }

    public class AudioMediaApiViewModel : MediaApiViewModel
    {
        public MediaFileApiViewModel File { get; set; }
    }

    public class FileMediaApiViewModel : MediaApiViewModel
    {
        public MediaFileApiViewModel File { get; set; }
    }

    public class UriMediaApiViewModel : MediaApiViewModel
    {
        public string Uri { get; set; }
    }
}