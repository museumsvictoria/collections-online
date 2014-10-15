using System.Collections.Generic;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Models;
using Nancy.Helpers;
using Nancy.ViewEngines.Razor;

namespace CollectionsOnline.WebSite.Extensions
{
    public static class HtmlHelpersExtensions
    {
        public static IHtmlString RenderAssociationDescription<T>(this HtmlHelpers<T> helpers, Association association)
        {
            var parts = new List<string>();

            if(!string.IsNullOrWhiteSpace(association.Name))
                parts.Add(string.Format("<a href=\"/search?name={0}\">{1}</a>", HttpUtility.UrlEncode(association.Name), association.Name));
            if(!string.IsNullOrWhiteSpace(association.StreetAddress))
                parts.Add(association.StreetAddress);
            if(!string.IsNullOrWhiteSpace(association.Locality))
                parts.Add(string.Format("<a href=\"/search?locality={0}\">{1}</a>", HttpUtility.UrlEncode(association.Locality), association.Locality));
            if(!string.IsNullOrWhiteSpace(association.Region))
                parts.Add(string.Format("<a href=\"/search?locality={0}\">{1}</a>", HttpUtility.UrlEncode(association.Region), association.Region));
            if(!string.IsNullOrWhiteSpace(association.State))
                parts.Add(string.Format("<a href=\"/search?locality={0}\">{1}</a>", HttpUtility.UrlEncode(association.State), association.State));
            if(!string.IsNullOrWhiteSpace(association.Country))
                parts.Add(string.Format("<a href=\"/search?locality={0}\">{1}</a>", HttpUtility.UrlEncode(association.Country), association.Country));
            if(!string.IsNullOrWhiteSpace(association.Date))
                parts.Add(association.Date);

            var result = parts.Concatenate(", ");

            if(!string.IsNullOrWhiteSpace(association.Comments))
                result = string.Format("{0}<br/>{1}<br/>", result, association.Comments);

            return new NonEncodedHtmlString(result);
        }
    }
}