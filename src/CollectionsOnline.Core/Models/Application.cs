using System;
using CollectionsOnline.Core.Config;

namespace CollectionsOnline.Core.Models
{
    public class Application : AggregateRoot
    {
        public DateTime LastDataImport { get; set; }

        public bool DataImportRunning { get; set; }

        public Application()
        {
            Id = Constants.ApplicationId;
        }

        public void RunDataImport()
        {
            if (!DataImportRunning)
            {
                DataImportRunning = true;
            }
        }

        public void DataImportFinished()
        {
            DataImportRunning = false;
        }

        public void DataImportSuccess(DateTime dateCompleted)
        {
            DataImportRunning = false;
            LastDataImport = dateCompleted;
        }
    }
}