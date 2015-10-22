using System.Collections.Generic;
using System.Linq;
using System.Web;
using CollectionsOnline.Core.Extensions;
using Nancy.Extensions;

namespace CollectionsOnline.WebSite.Extensions
{
    public static class HttpRequestExtensions
    {
        public static IDictionary<string, string> RenderParams(this HttpRequest httpRequest)
        {
            return httpRequest.Params.ToDictionary()
                .Where(x => x.Value.Any(y => !string.IsNullOrWhiteSpace(y)))
                .ToDictionary(key => key.Key, pair => pair.Value.Concatenate(";"));
        }
    }
}