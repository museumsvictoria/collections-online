using System.Collections.Generic;
using System.Linq;
using System.Web;
using CollectionsOnline.Core.Config;
using CollectionsOnline.WebApi.Models;
using Nancy;
using Nancy.ModelBinding;
using Raven.Client;

namespace CollectionsOnline.WebApi.Modules
{
    public abstract class BaseModule : NancyModule
    {
        protected BaseModule(string modulePath = "")
            : base("/v1" + modulePath)
        {
            Before += context =>
                {
                    BindPagination();

                    return null;
                };

            After += context =>
                {
                    if (Statistics != null && !Envelope)
                    {
                        Context.Response.Headers["Link"] = BuildLinkHeader();
                        Context.Response.Headers["Total-Results"] = Statistics.TotalResults.ToString();
                    }
                };
        }

        protected Response BuildResponse(object model, HttpStatusCode httpStatus = HttpStatusCode.OK)
        {
            if (Envelope)
            {
                if (Statistics != null)
                {
                    return Response.AsJson(new
                    {
                        Headers = new
                        {
                            Link = BuildLinkHeader(),
                            TotalResults = Statistics.TotalResults
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

            return Response.AsJson(model).WithStatusCode(httpStatus);
        }

        protected Response BuildErrorResponse(HttpStatusCode httpStatus, string message, params object[] args)
        {
            return BuildResponse(new { Error = string.Format(message, args) }, httpStatus);
        }

        private void BindPagination()
        {
            var paginationInputModel = this.Bind<PaginationInputModel>();

            if (paginationInputModel.Offset < 0)
                paginationInputModel.Offset = 0;

            if (paginationInputModel.Limit <= 0 || paginationInputModel.Limit > Constants.WebApiPagingPageSizeMax)
                paginationInputModel.Limit = Constants.WebApiPagingPageSizeDefault;

            Offset = paginationInputModel.Offset;
            Limit = paginationInputModel.Limit;
            Envelope = paginationInputModel.Envelope;
        }

        private string BuildLinkHeader()
        {
            var queryString = HttpUtility.ParseQueryString(Request.Url.Query);
            var url = Request.Url;
            var links = new List<string>();

            // Next
            if ((Offset + Limit) < Statistics.TotalResults)
            {
                queryString.Set("offset", (Offset + Limit).ToString());

                url.Query = "?" + queryString;
                links.Add(string.Format("<{0}>; rel=\"next\"", url));
            }

            // Prev
            if ((Offset - Limit) >= 0)
            {
                queryString.Set("offset", (Offset - Limit).ToString());
                if ((Offset - Limit) == 0)
                {
                    queryString.Remove("offset");
                }

                url.Query = "?" + queryString;
                links.Add(string.Format("<{0}>; rel=\"prev\"", url));
            }

            if(links.Any())
                return links.Aggregate((lp1, lp2) => lp1 + "," + lp2);

            return string.Empty;
        }

        protected int Offset { get; private set; }

        protected int Limit { get; private set; }

        protected bool Envelope { get; private set; }

        public RavenQueryStatistics Statistics;
    }
}