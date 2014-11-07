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

        public string RightsStatus { get; set; }

        public string Licence { get; set; }

        public string LicenceDetails { get; set; }
    }

    public class ImageMedia : Media
    {
        public string AlternativeText { get; set; }

        public ImageMediaFile Original { get; set; }

        public ImageMediaFile Thumbnail { get; set; }

        public ImageMediaFile Small { get; set; }

        public ImageMediaFile Medium { get; set; }

        public ImageMediaFile Large { get; set; }
    }
}