using System;
using CollectionsOnline.Core.Config;

namespace CollectionsOnline.Core.DomainModels
{
    public class Application : DomainModel
    {
        public DateTime LastDataImport { get; private set; }

        public bool DataImportRunning { get; private set; }

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