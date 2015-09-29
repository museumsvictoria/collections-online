namespace CollectionsOnline.WebSite.Models.Api
{
    public class MediaFileApiViewModel
    {
        public string Uri { get; set; }

        public long Size { get; set; }
    }

    public class ImageMediaFileApiViewModel : MediaFileApiViewModel
    {
        public int Width { get; set; }

        public int Height { get; set; }
    }
}