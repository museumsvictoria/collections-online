using System.Collections.Specialized;
using System.Linq;
using Nancy.Helpers;

namespace CollectionsOnline.WebSite.Extensions
{
    public static class NameValueCollectionExtensions
    {
        public static string[] GetQueryStringValues(this NameValueCollection self, string name)
        {
            var values = self.GetValues(name);

            return values?.SelectMany(x => x.Split(',')).ToArray();
        }

        public static string RenderQueryString(this NameValueCollection self)
        {
            var keys = self.AllKeys;
            for (var i = 0; i < self.Count; i++)
            {
                self[keys[i]] = HttpUtility.UrlEncode(self[keys[i]]);
            }

            return self.ToString();
        }
    }
}