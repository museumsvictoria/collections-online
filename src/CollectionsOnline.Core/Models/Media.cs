using System;
using System.Collections.Generic;

namespace CollectionsOnline.Core.Models
{
    public abstract class Media
    {
        public long Irn { get; set; }

        public DateTime DateModified { get; set; }

        public string Title { get; set; }
    }

    public class ImageMedia : Media
    {
        public string AlternateText { get; set; }

        public ImageMediaFile Original { get; set; }

        public ImageMediaFile Thumbnail { get; set; }

        public ImageMediaFile Small { get; set; }

        public ImageMediaFile Medium { get; set; }

        public ImageMediaFile Large { get; set; }
    }
}