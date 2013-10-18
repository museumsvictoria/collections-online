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
                    BuildResponseHeaders(context);
                };
        }

        protected Response BuildResponse(object model)
        {
            // TODO: add logic to prevent pagination data going out if not a paginated response
            if (Envelope)
                return Response.AsJson(new
                    {
                        Data = model,
                        Pagination = new
                            {
                                Offset, 
                                Limit,
                                Total = Statistics.TotalResults,
                            }
                    });
            
            return Response.AsJson(model);
        }

        protected Response BuildErrorResponse(HttpStatusCode httpStatus, string message, params object[] args)
        {            
            return Response.AsJson(new
                {
                    Error = string.Format(message, args),
                    HttpStatus = httpStatus
                }).WithStatusCode(httpStatus);
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

        private void BuildResponseHeaders(NancyContext context)
        {
            // If we are not paginating dont set headers
            if (Statistics == null) return;

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

            context.Response.Headers["Link"] = links.Aggregate((lp1, lp2) => lp1 + "," + lp2);
            context.Response.Headers["Total"] = Statistics.TotalResults.ToString();
        }

        protected int Offset { get; private set; }

        protected int Limit { get; private set; }

        protected bool Envelope { get; private set; }

        public RavenQueryStatistics Statistics;
    }
}