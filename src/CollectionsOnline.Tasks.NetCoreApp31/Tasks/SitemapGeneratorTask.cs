using CollectionsOnline.Tasks.NetCoreApp31.Infrastructure;
using Serilog;

namespace CollectionsOnline.Tasks.NetCoreApp31.Tasks
{
    public class SitemapGeneratorTask : ITask
    {
        public void Run()
        {
            Log.Logger.Error("This has not been implemented yet");
        }

        public int Order => 100;
    }
}
