using CollectionsOnline.Core.Models;
using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebSite.Features.Stories
{
    public class StoriesModule : NancyModule
    {
        public StoriesModule(
            IStoryViewModelQuery storyViewModelQuery,
            IDocumentSession documentSession)            
        {
            Get["/stories/{id}"] = parameters =>
            {
                var story = documentSession
                    .Load<Story>("stories/" + parameters.id as string);

                return (story == null || story.IsHidden) ? HttpStatusCode.NotFound : View["stories", storyViewModelQuery.BuildStory("stories/" + parameters.id)];
            };
        }
    }
}