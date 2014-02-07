using CollectionsOnline.Core.Models;

namespace CollectionsOnline.WebSite.Features.Stories
{
    public class StoryViewModelFactory : IStoryViewModelFactory
    {
        public StoryViewModel MakeViewModel(Story story)
        {
            var storyViewModel = new StoryViewModel
                {
                    Story = story
                };

            return storyViewModel;
        }
    }
}