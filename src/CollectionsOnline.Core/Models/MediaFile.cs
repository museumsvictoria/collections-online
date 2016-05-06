using System.IO;
using CollectionsOnline.Core.Extensions;

namespace CollectionsOnline.Core.Models
{
    public class MediaFile
    {
        public string Uri { get; set; }

        public long Size { get; set; }

        public string SizeShortened
        {
            get
            {
                return this.Size.BytesToString();
            }
        }
        
        public string Extension
        {
            get
            {
                var extension = Path.GetExtension(Uri);
                
                if (extension != null) 
                    return extension.Replace(".", string.Empty);
                
                return string.Empty;
            }
        }
    }

    public class ImageMediaFile : MediaFile
    {
        public int Width { get; set; }

        public int Height { get; set; }
    }
}