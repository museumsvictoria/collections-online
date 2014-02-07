using CollectionsOnline.Core.Models;

namespace CollectionsOnline.WebSite.Features.Stories
{
    public interface IStoryViewModelFactory
    {
        StoryViewModel MakeViewModel(Story story);
    }
}