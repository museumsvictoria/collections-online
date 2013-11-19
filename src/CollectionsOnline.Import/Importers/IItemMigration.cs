using System;

namespace CollectionsOnline.Import.Importers
{
    public interface IItemMigration
    {
        void Run(DateTime dateLastRun);
    }
}