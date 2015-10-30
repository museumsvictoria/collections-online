using System;

namespace CollectionsOnline.Core.Models
{
    public class CollectionEvent
    {
        public long Irn { get; set; }

        public string ExpeditionName { get; set; }

        public string CollectionEventCode { get; set; }

        public string SamplingMethod { get; set; }

        public DateTime? DateVisitedFrom { get; set; }

        public DateTime? DateVisitedTo { get; set; }

        public string DepthTo { get; set; }

        public string DepthFrom { get; set; }

        public string CollectedBy { get; set; }
    }
}