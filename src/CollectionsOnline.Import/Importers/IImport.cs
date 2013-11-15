using System;
using CollectionsOnline.Core.Models;
using IMu;

namespace CollectionsOnline.Import.Importers
{
    public interface IImport<out T> where T : EmuAggregateRoot
    {
        //TODO: push imu specific bitz into factories
        string ModuleName { get; }

        string[] Columns { get; }

        Terms Terms { get; }

        T MakeDocument(Map map);

        void Run(DateTime dateLastRun);
    }
}