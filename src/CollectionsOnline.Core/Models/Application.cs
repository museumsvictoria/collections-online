using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Config;

namespace CollectionsOnline.Core.Models
{
    public class Application : AggregateRoot
    {
        public DateTime? PreviousDateRun { get; set; }

        public bool ImportsRunning { get; set; }

        public IList<ImportProgress> ImportProgresses { get; set; }

        public Application()
        {
            Id = Constants.ApplicationId;

            InitializeCollections();
        }

        public void RunAllImports()
        {
            if (!ImportsRunning)
            {
                ImportsRunning = true;
            }
        }

        public void AllImportsFinished()
        {
            ImportsRunning = false;
        }

        public void AllImportsFinishedSuccessfully(DateTime dateRun)
        {
            ImportsRunning = false;
            PreviousDateRun = dateRun;
            ImportProgresses.Clear();
        }

        public void ImportFinished(string importType)
        {
            GetImportProgress(importType).IsFinished = true;
        }

        public ImportProgress GetImportProgress(string importType)
        {
            var importProgress = ImportProgresses.FirstOrDefault(x => x.ImportType == importType);

            if (importProgress == null)
            {
                importProgress = new ImportProgress
                {
                    ImportType = importType,
                    CurrentOffset = 0,
                    IsFinished = false
                };

                ImportProgresses.Add(importProgress);
            }
            
            return importProgress;
        }

        public void UpdateImportCurrentOffset(string importType, int currentOffset)
        {
            GetImportProgress(importType).CurrentOffset = currentOffset;
        }

        private void InitializeCollections()
        {
            ImportProgresses = new List<ImportProgress>();
        }
    }
}