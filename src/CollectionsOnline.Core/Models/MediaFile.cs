using System;
using System.IO;

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
                string[] unit = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
                if (this.Size == 0)
                    return "0" + unit[0];
                this.Size = Math.Abs(this.Size);
                int place = Convert.ToInt32(Math.Floor(Math.Log(this.Size, 1024)));
                double num = Math.Round(this.Size / Math.Pow(1024, place), 1);

                return (Math.Sign(this.Size) * num) + unit[place];
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