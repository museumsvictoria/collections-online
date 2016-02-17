using System;
using System.Web;
using CollectionsOnline.WebSite.Extensions;
using Nancy;
using Serilog;

namespace CollectionsOnline.WebSite
{
    public class Global : HttpApplication
    {
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
            var ex = Server.GetLastError();

            Log.Logger.Fatal(ex, "Unhandled Exception occured in System.Web pipeline {Url} {@HttpParams}", Request.Url, Request.RenderParams());

            Server.ClearError();

            Response.ContentType = "text/html";
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            Response.TransmitFile(@"Views\Error\500.html");
        }
    }
}