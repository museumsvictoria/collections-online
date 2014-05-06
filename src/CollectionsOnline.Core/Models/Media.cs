using System;

namespace CollectionsOnline.Core.Models
{
    public class Media
    {
        public long Irn { get; set; }

        public DateTime DateModified { get; set; }

        public string Title { get; set; }

        public string AlternateText { get; set; }

        public string Type  { get; set; }

        public string Url { get; set; }
    }
}