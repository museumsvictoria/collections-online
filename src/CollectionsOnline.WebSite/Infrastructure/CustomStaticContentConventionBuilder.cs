using System;
using Nancy;
using Nancy.Conventions;
using System.Collections.Generic;

namespace CollectionsOnline.WebSite.Infrastructure
{
    public static class CustomStaticContentConventionBuilder
    {
        public static Func<NancyContext, string, Response> AddDirectory(
            string requestedPath,            
            IDictionary<string, string> customHeaders = null,
            string contentPath = null,
            params string[] allowedExtensions)
        {
            var responseBuilder = StaticContentConventionBuilder.AddDirectory(requestedPath, contentPath, allowedExtensions);

            return (context, root) =>
            {
                var response = responseBuilder(context, root);
                if (response != null && customHeaders != null)
                {
                    foreach (var customHeader in customHeaders)
                    {
                        response.Headers.Add(customHeader.Key, customHeader.Value);
                    }
                }
                return response;
            };
        }

        public static Func<NancyContext, string, Response> AddFile(
            string requestedFile,
            string contentFile,
            IDictionary<string, string> customHeaders = null)
        {
            var responseBuilder = StaticContentConventionBuilder.AddFile(requestedFile, contentFile);

            return (context, root) =>
            {
                var response = responseBuilder(context, root);
                if (response != null && customHeaders != null)
                {
                    foreach (var customHeader in customHeaders)
                    {
                        response.Headers.Add(customHeader.Key, customHeader.Value);
                    }
                }
                return response;
            };
        }
    }
}