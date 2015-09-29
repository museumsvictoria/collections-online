using System.Configuration;
using Nancy.ViewEngines.Razor;

namespace CollectionsOnline.WebSite.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string CurrentUrl<T>(this UrlHelpers<T> helper)
        {
            return string.Format("{0}{1}", ConfigurationManager.AppSettings["CanonicalSiteBase"], helper.RenderContext.Context.Request.Path);
        }

        public static string CurrentBasePath<T>(this UrlHelpers<T> helper)
        {
            return ConfigurationManager.AppSettings["CanonicalSiteBase"];
        }
    }
}