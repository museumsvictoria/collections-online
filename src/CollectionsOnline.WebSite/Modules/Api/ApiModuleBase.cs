using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using CollectionsOnline.Core.Config;
using CollectionsOnline.WebSite.Models.Api;
using Nancy;
using Nancy.ModelBinding;

namespace CollectionsOnline.WebSite.Modules.Api
{
    public abstract class ApiModuleBase : NancyModule
    {
        protected ApiModuleBase(string modulePath = "")
            : base(string.Format("/{0}{1}{2}", Constants.ApiPathBase, Constants.CurrentApiVersionPathSegment, modulePath))
        {
            Before += context =>
                {
                    BindApiInput();

                    return null;
                };

            After += ctx =>
                {
                    // Encode response if client accepts
                    if (ctx.Request.Headers.AcceptEncoding.Any(x => x.Contains("gzip")))
                    {
                        var jsonData = new MemoryStream();
                        ctx.Response.Contents.Invoke(jsonData);
                        jsonData.Position = 0;
                        if (jsonData.Length < 4096)
                        {
                            ctx.Response.Contents = stream =>
                            {
                                jsonData.CopyTo(stream);
                                stream.Flush();
                            };
                        }
                        else
                        {
                            ctx.Response.WithHeader("Content-Encoding", "gzip");
                            ctx.Response.WithHeader("Vary", "Accept-Encoding");
                            ctx.Response.Contents = stream =>
                            {
                                using (var gzip = new GZipStream(stream, CompressionMode.Compress))
                                {
                                    jsonData.CopyTo(gzip);
                                }
                            };
                        }
                    }
                };
        }

        protected Response BuildResponse(object model, HttpStatusCode httpStatus = HttpStatusCode.OK, ApiPageInfo apiPageInfo = null)
        {
            if (ApiInputModel.Envelope)
            {
                // Check if we have pagination information to return
                if(apiPageInfo != null)
                {
                    return Response.AsJson(new
                    {
                        Headers = new
                        {
                            Link = BuildLinkHeader(apiPageInfo),
                            TotalResults = apiPageInfo.TotalResults,
                            TotalPages = apiPageInfo.TotalPages,
                        },
                        Response = model,
                        Status = httpStatus
                    });
                }
                
                return Response.AsJson(new
                    {
                        Response = model,
                        Status = httpStatus
                    });
            }

            var response = Response
                .AsJson(model)
                .WithStatusCode(httpStatus);

            // No envelope but we have pagination infomation to return
            if (apiPageInfo != null)
            {
                response
                    .WithHeader("Link", BuildLinkHeader(apiPageInfo))
                    .WithHeader("Total-Results", apiPageInfo.TotalResults.ToString())
                    .WithHeader("Total-Pages", apiPageInfo.TotalPages.ToString());
            }
            
            return response;
        }

        private void BindApiInput()
        {
            var apiInputModel = this.Bind<ApiInputModel>();

            if (apiInputModel.Page <= 0)
                apiInputModel.Page = 1;

            if (apiInputModel.PerPage < Constants.PagingPerPageDefault || apiInputModel.PerPage > Constants.PagingPerPageMax)
                apiInputModel.PerPage = Constants.PagingPerPageDefault;

            ApiInputModel = apiInputModel;
        }

        private string BuildLinkHeader(ApiPageInfo apiPageInfo)
        {
            var queryString = HttpUtility.ParseQueryString(Request.Url.Query);
            var url = Request.Url;
            var totalPages = apiPageInfo.TotalPages;
            var links = new List<string>();

            // Next
            if ((ApiInputModel.Page + 1) <= totalPages)
            {
                queryString.Set("page", (ApiInputModel.Page + 1).ToString());

                url.Query = "?" + queryString;
                links.Add(string.Format("<{0}>; rel=\"next\"", url));
            }

            // Prev
            if ((ApiInputModel.Page - 1) >= 1)
            {
                queryString.Set("page", (ApiInputModel.Page - 1).ToString());
                if ((ApiInputModel.Page - 1) == 1)
                {
                    queryString.Remove("page");
                }

                url.Query = "?" + queryString;
                links.Add(string.Format("<{0}>; rel=\"prev\"", url));
            }

            if(links.Any())
                return links.Aggregate((lp1, lp2) => lp1 + "," + lp2);

            return string.Empty;
        }        

        protected ApiInputModel ApiInputModel { get; private set; }
    }
}