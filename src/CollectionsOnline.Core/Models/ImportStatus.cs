using System;

namespace CollectionsOnline.Core.Models
{
    public class ImportStatus
    {
        public string ImportType { get; set; }

        public DateTime? PreviousDateRun { get; set; }

        public int CurrentImportCacheOffset { get; set; }

        public bool IsFinished { get; set; }
    }
}