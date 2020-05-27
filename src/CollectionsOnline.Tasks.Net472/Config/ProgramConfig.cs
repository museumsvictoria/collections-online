using System;
using Serilog;

namespace CollectionsOnline.Tasks.Net472.Config
{
    public static class ProgramConfig
    {
        public static void Initialize()
        {
            // Set up Ctrl+C handling 
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                Log.Logger.Warning("Canceling all tasks");

                eventArgs.Cancel = true;
                Program.TasksCanceled = true;
            };

            // Log any exceptions that are not handled
            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) => Log.Logger.Fatal((Exception)eventArgs.ExceptionObject, "Unhandled Exception occured in tasks");
        }
    }
}
