using System;

namespace CollectionsOnline.WebSite.Models
{
    public class EmuAggregateRootViewModel
    {
        public string Id { get; set; }

        public string DisplayTitle { get; set; }

        public string SubDisplayTitle { get; set; }

        public string Summary { get; set; }

        public string ThumbnailUri { get; set; }

        public string RecordType { get; set; }

        public double Quality { get; set; }

        public DateTime DateModified { get; set; }
    }
}   