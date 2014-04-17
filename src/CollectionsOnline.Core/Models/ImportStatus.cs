using System;
using System.Collections.Generic;

namespace CollectionsOnline.Core.Models
{
    public class ImportStatus
    {
        public string ImportType { get; set; }

        public DateTime? PreviousDateRun { get; set; }

        public int CurrentOffset { get; set; }

        public bool IsFinished { get; set; }

        public DateTime? CachedResultDate { get; set; }

        public IList<long> CachedResult { get; set; }
    }
}