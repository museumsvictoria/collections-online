using System;
using System.Collections;
using System.Collections.Generic;

namespace CollectionsOnline.Core.Models
{
    public class ImportProgress
    {
        public string ImportType { get; set; }

        public DateTime DateRun { get; set; }

        public int CurrentOffset { get; set; }

        public bool IsFinished { get; set; }

        public IList<long> CachedResult { get; set; }

        public ImportProgress()
        {
            InitializeCollections();
        }

        private void InitializeCollections()
        {
            CachedResult = new List<long>();
        }
    }
}