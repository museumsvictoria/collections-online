using System;
using CollectionsOnline.Core.Config;
using Serilog;

namespace CollectionsOnline.Import.Imports
{
    public abstract class ImuImportBase : IImport
    {
        public abstract void Run();

        public abstract int Order { get; }

        protected bool ImportCanceled()
        {
            if (DateTime.Now.TimeOfDay > Constants.ImuOfflineTimeSpanStart && DateTime.Now.TimeOfDay < Constants.ImuOfflineTimeSpanEnd)
            {
                Log.Logger.Warning("Imu about to go offline, canceling all imports");
                Program.ImportCanceled = true;
            }

            return Program.ImportCanceled;
        }
    }
}