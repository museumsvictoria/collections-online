namespace CollectionsOnline.Core.Models
{
    public class MediaFile
    {
        public string Uri { get; set; }

        public long Size { get; set; }
    }

    public class ImageMediaFile : MediaFile
    {
        public int Width { get; set; }

        public int Height { get; set; }
    }    
}