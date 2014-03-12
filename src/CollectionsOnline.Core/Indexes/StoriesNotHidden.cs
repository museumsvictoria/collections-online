using System.Linq;
using CollectionsOnline.Core.Models;
using Raven.Client.Indexes;

namespace CollectionsOnline.Core.Indexes
{
    public class StoriesNotHidden : AbstractIndexCreationTask<Story>
    {
        public StoriesNotHidden()
        {
            Map = stories =>
                from story in stories
                where story.IsHidden == false
                select new {};
        }
    }
}
