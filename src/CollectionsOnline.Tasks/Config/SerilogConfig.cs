using System;
using System.Configuration;
using Serilog;

namespace CollectionsOnline.Tasks.Config
{
    public static class SerilogConfig
    {
        public static void Initialize()
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .WriteTo.ColoredConsole()
                .WriteTo.Seq(ConfigurationManager.AppSettings["SeqUrl"])
                .WriteTo.RollingFile($"{AppDomain.CurrentDomain.BaseDirectory}\\logs\\{{Date}}.txt")
                .Enrich.WithMachineName()
                .CreateLogger();
        }
    }
}
