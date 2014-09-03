namespace CollectionsOnline.Core.Models
{
    public abstract class MediaFile
    {
        public string Uri { get; set; }
    }

    public class ImageMediaFile : MediaFile
    {
        public int Width { get; set; }

        public int Height { get; set; }
    }
}