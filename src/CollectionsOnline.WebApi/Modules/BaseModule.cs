using System;
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
                    BindPagination(context);

                    return null;
                };

            After += context =>
                {
                    BuildPaginationResponseHeaders(context);
                };
        }        

        private void BindPagination(NancyContext context)
        {
            var baseInputModel = this.Bind<BaseInputModel>();

            if (baseInputModel.Offset < 0)
                baseInputModel.Offset = 0;

            if (baseInputModel.Size <= 0 || baseInputModel.Size > Constants.WebApiPagingPageSizeMax)
                baseInputModel.Size = Constants.WebApiPagingPageSizeDefault;

            Offset = baseInputModel.Offset;
            Size = baseInputModel.Size;
        }

        private void BuildPaginationResponseHeaders(NancyContext context)
        {
            // If we are not paginating dont set headers
            if (Statistics == null) return;

            var queryString = HttpUtility.ParseQueryString(Request.Url.Query);
            var url = Request.Url;
            var links = new List<string>();

            // Next
            if ((Offset + Size) < Statistics.TotalResults)
            {
                queryString.Set("offset", (Offset + Size).ToString());

                url.Query = "?" + queryString;
                links.Add(string.Format("<{0}>; rel=\"next\"", url));
            }

            // Prev
            if ((Offset - Size) >= 0)
            {
                queryString.Set("offset", (Offset - Size).ToString());
                if ((Offset - Size) == 0)
                {
                    queryString.Remove("offset");
                }

                url.Query = "?" + queryString;
                links.Add(string.Format("<{0}>; rel=\"prev\"", url));
            }

            context.Response.Headers["Link"] = links.Aggregate((lp1, lp2) => lp1 + "," + lp2);
            context.Response.Headers["Total-Results"] = Statistics.TotalResults.ToString();
        }

        protected int Offset { get; private set; }

        protected int Size { get; private set; }

        public RavenQueryStatistics Statistics;
    }
}