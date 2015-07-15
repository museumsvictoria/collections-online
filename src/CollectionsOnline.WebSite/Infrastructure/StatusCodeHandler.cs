using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.ErrorHandling;
using Nancy.ViewEngines;

namespace CollectionsOnline.WebSite.Infrastructure
{
    public class StatusCodeHandler : IStatusCodeHandler
    {
        private static IList<HttpStatusCode> _handles = new List<HttpStatusCode>();
        private IViewRenderer _viewRenderer;

        public StatusCodeHandler(IViewRenderer viewRenderer)
        {
            _viewRenderer = viewRenderer;

            _handles.Add(HttpStatusCode.NotFound);
            _handles.Add(HttpStatusCode.InternalServerError);
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return _handles.Any(x => x == statusCode);
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            try
            {
                var response = _viewRenderer.RenderView(context, string.Format("Error/{0}.cshtml", (int)statusCode));
                response.StatusCode = statusCode;
                context.Response = response;
            }
            catch (Exception)
            {
                context.Response.StatusCode = statusCode;
            }
        }
    }
}