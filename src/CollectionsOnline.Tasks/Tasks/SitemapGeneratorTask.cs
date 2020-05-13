using System;
using CollectionsOnline.Tasks.Infrastructure;

namespace CollectionsOnline.Tasks.Tasks
{
    public class SitemapGeneratorTask : ITask
    {
        public void Run()
        {
            throw new NotImplementedException();
        }

        public int Order => 100;
    }
}
