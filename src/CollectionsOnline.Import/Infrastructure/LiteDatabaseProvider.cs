using System.Configuration;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using LiteDB;
using Ninject.Activation;
using Serilog;

namespace CollectionsOnline.Import.Infrastructure
{
    public class LiteDatabaseProvider : Provider<ILiteDatabase>
    {
        protected override ILiteDatabase CreateInstance(IContext context)
        {
            // Open database (or create if doesn't exist)
            return new LiteDatabase(ConfigurationManager.ConnectionStrings["LiteDB"].ConnectionString);
        }
    }
}