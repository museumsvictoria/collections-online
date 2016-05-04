using System;
using System.Collections.Generic;

namespace CollectionsOnline.Core.Models
{
    public abstract class Media
    {
        public long Irn { get; set; }

        public DateTime DateModified { get; set; }

        public string Caption { get; set; }

        public IList<string> Creators { get; set; }

        public IList<string> Sources { get; set; }

        public string Credit { get; set; }

        public string RightsStatement { get; set; }

        public Licence Licence { get; set; }
    }

    public class ImageMedia : Media, IHasThumbnail, IHasChecksum
    {
        public string AlternativeText { get; set; }

        public ImageMediaFile Original { get; set; }

        public ImageMediaFile Thumbnail { get; set; }

        public ImageMediaFile Medium { get; set; }

        public ImageMediaFile Large { get; set; }

        public string Md5Checksum { get; set; }
    }

    public class VideoMedia : Media, IHasThumbnail
    {
        public string AlternativeText { get; set; }

        public string Uri { get; set; }

        public string VideoId { get; set; }

        public ImageMediaFile Thumbnail { get; set; }

        public ImageMediaFile Medium { get; set; }
    }

    public class AudioMedia : Media, IHasThumbnail, IHasChecksum
    {
        public MediaFile File { get; set; }

        public ImageMediaFile Thumbnail { get; set; }

        public string Md5Checksum { get; set; }
    }

    public class FileMedia : Media, IHasChecksum
    {
        public MediaFile File { get; set; }

        public string Md5Checksum { get; set; }
    }

    public class UriMedia : Media
    {
        public string Uri { get; set; }
    }

    public interface IHasThumbnail
    {
        ImageMediaFile Thumbnail { get; set; }
    }

    public interface IHasChecksum
    {
        string Md5Checksum { get; set; }
    }
}