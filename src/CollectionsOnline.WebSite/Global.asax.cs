using System;
using System.Web;
using Nancy;
using NLog;

namespace CollectionsOnline.WebSite
{
    public class Global : HttpApplication
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            // Remove headers that are not needed
            var application = sender as HttpApplication;
            if (application != null && application.Context != null)
            {
                application.Context.Response.Headers.Remove("Server");
                application.Context.Response.Headers.Remove("X-MiniProfiler-Ids");
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // Capture any errors occurring before the nancy pipeline
            var error = Server.GetLastError();
            _log.Error("Application_Error occured [url:{0}]: {1}", Request.Url, error);

            Server.ClearError();

            Response.ContentType = "text/html";
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            Response.TransmitFile(@"Views\Error\500.html");
        }
    }
}