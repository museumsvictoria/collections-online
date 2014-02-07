namespace CollectionsOnline.WebSite.Features.Stories
{
    public interface IStoryViewModelQuery
    {
        StoryViewModel BuildStory(string storyId);
    }
}