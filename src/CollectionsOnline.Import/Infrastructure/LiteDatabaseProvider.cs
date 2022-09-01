using System.Configuration;
using LiteDB;
using Ninject.Activation;
using Serilog;

namespace CollectionsOnline.Import.Infrastructure
{
    public class LiteDatabaseProvider : Provider<ILiteDatabase>
    {
        protected override ILiteDatabase CreateInstance(IContext context)
        {
            return new LiteDatabase($"Filename={ConfigurationManager.AppSettings["WebSitePath"]}\\content\\media\\media-checksum.db");
        }
    }
}