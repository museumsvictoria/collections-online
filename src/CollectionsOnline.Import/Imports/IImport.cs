using System;

namespace CollectionsOnline.Import.Imports
{
    public interface IImport
    {
        void Run(DateTime? previousDateRun);
    }
}