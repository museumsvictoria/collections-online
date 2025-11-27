using System;
using System.Web;
using CollectionsOnline.WebSite.Extensions;
using Nancy;
using Serilog;

namespace CollectionsOnline.WebSite
{
    public class Global : HttpApplication
    {
        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-MiniProfiler-Ids");
        }
        
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current.Request.IsSecureConnection)
            {
                HttpContext.Current.Response.AddHeader("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");
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