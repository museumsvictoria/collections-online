using CollectionsOnline.Core.Models;
using Raven.Client;

namespace CollectionsOnline.WebSite.Features.Stories
{
    public class StoryViewModelQuery : IStoryViewModelQuery
    {
        private readonly IDocumentSession _documentSession;
        private readonly IStoryViewModelFactory _storyViewModelFactory;

        public StoryViewModelQuery(
            IDocumentSession documentSession,
            IStoryViewModelFactory storyViewModelFactory)
        {
            _documentSession = documentSession;
            _storyViewModelFactory = storyViewModelFactory;
        }

        public StoryViewModel BuildStory(string storyId)
        {
            var story = _documentSession
                .Load<Story>(storyId);

            return _storyViewModelFactory.MakeViewModel(story);
        }
    }
}