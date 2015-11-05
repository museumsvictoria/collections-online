using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Config;
using Nancy;
using Nancy.ErrorHandling;
using Nancy.Responses;
using Nancy.ViewEngines;

namespace CollectionsOnline.WebSite.Infrastructure
{
    public class StatusCodeHandler : IStatusCodeHandler
    {
        private static IList<HttpStatusCode> _handles = new List<HttpStatusCode>();
        private IViewRenderer _viewRenderer;
        private IEnumerable<ISerializer> _serializers;

        public StatusCodeHandler(
            IViewRenderer viewRenderer,
            IEnumerable<ISerializer> serializers)
        {
            _viewRenderer = viewRenderer;
            _serializers = serializers;

            _handles.Add(HttpStatusCode.NotFound);
            _handles.Add(HttpStatusCode.InternalServerError);
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return _handles.Any(x => x == statusCode);
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            Response response;

            // Force json if request from api, otherwise return view
            if (context.Request.Path.StartsWith(Constants.ApiPathBase))
            {
                response = new JsonResponse(new {Error = string.Format("{0} - {1}", (int) statusCode, statusCode)},
                    _serializers.FirstOrDefault(s => s.CanSerialize("application/json")));
            }
            else
            {
                response = _viewRenderer.RenderView(context, string.Format("Error/{0}.cshtml", (int)statusCode));                    
            }

            response.WithStatusCode(statusCode);

            context.Response = response;
        }
    }
}