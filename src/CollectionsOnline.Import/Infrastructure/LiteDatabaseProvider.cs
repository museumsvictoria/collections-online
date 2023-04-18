using System.Configuration;
using CollectionsOnline.Core.Factories;
using LiteDB;
using Ninject.Activation;

namespace CollectionsOnline.Import.Infrastructure
{
    public class LiteDatabaseProvider : Provider<ILiteDatabase>
    {
        protected override ILiteDatabase CreateInstance(IContext context)
        {
            // Try to create destination path first as LiteDB needs an existing directory when creating a DB
            PathFactory.CreateDestPath($"{ConfigurationManager.AppSettings["WebSitePath"]}\\content\\media\\");
            
            return new LiteDatabase($"Filename={ConfigurationManager.AppSettings["WebSitePath"]}\\content\\media\\media-checksum.db");
        }
    }
}