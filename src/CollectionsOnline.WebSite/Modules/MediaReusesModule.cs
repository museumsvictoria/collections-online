using CollectionsOnline.WebSite.Commands;
using CollectionsOnline.WebSite.Commands.Handlers;
using Nancy;
using Nancy.ModelBinding;

namespace CollectionsOnline.WebSite.Modules
{
    public class MediaReusesModule : NancyModule
    {
        public MediaReusesModule(IMediaReuseCommandHandler mediaReuseCommandHandler)
        {
            Post["/mediareuses"] = parameters =>
            {
                mediaReuseCommandHandler.Handle(this.Bind<MediaReuseCommand>());

                return HttpStatusCode.OK;
            };
        }
    }
}