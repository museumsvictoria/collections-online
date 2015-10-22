using System;
using System.Configuration;
using Serilog;

namespace CollectionsOnline.Core.Infrastructure
{
    public static class SerilogConfig
    {
        public static void Initialize()
        {           
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .WriteTo.ColoredConsole()
                .WriteTo.Seq(ConfigurationManager.AppSettings["SeqUrl"])
                .WriteTo.RollingFile(string.Format("{0}\\logs\\{{Date}}.txt", AppDomain.CurrentDomain.BaseDirectory))
                .Enrich.WithMachineName()
                .CreateLogger();
        }
    }
}