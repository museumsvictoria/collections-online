using System.Linq;
using CollectionsOnline.Core.Models;
using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebApi.Modules
{
    public class StoryModule : BaseModule
    {
        public StoryModule(IDocumentSession documentSession)
            : base("/stories")
        {
            Get["/"] = parameters =>
                {
                    var stories = documentSession
                        .Query<Story>()
                        .Statistics(out Statistics)
                        .Skip(Offset)
                        .Take(Limit)
                        .ToList();

                    return BuildResponse(stories);
                };

            Get["/{storyId}"] = parameters =>
                {
                    string storyId = parameters.storyId;
                    var story = documentSession
                        .Load<Item>("specimens/" + storyId);

                    return story == null ? BuildErrorResponse(HttpStatusCode.NotFound, "Story {0} not found", storyId) : BuildResponse(story);
                };
        }
    }
}