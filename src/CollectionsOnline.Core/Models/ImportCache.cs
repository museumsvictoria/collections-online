using System;
using System.Collections.Generic;

namespace CollectionsOnline.Core.Models
{
    public class ImportCache : AggregateRoot
    {
        public ImportCache()
        {
            DateCreated = DateTime.Now;

            InitializeCollections();
        }

        public IList<long> Irns { get; set; }

        public DateTime DateCreated { get; set; }

        private void InitializeCollections()
        {
            Irns = new List<long>();
        }
    }
}