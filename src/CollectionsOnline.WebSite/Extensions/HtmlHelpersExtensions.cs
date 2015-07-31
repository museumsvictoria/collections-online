using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Models;
using Nancy.Helpers;
using Nancy.ViewEngines.Razor;
using Newtonsoft.Json;

namespace CollectionsOnline.WebSite.Extensions
{
    public static class HtmlHelpersExtensions
    {
        public static IHtmlString RenderAssociationDescription<T>(this HtmlHelpers<T> helper, Association association)
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
                parts.Add(string.Format("<a href=\"/search?date={0}\">{1}</a>", HttpUtility.UrlEncode(association.Date), association.Date));

            var result = parts.Concatenate(", ");

            if(!string.IsNullOrWhiteSpace(association.Comments))
                result = string.Format("{0}<br/>{1}<br/>", result, association.Comments);

            return new NonEncodedHtmlString(result);
        }

        public static IHtmlString RenderCitation<T>(this HtmlHelpers<T> helper, EmuAggregateRoot document)
        {
            var sb = new StringBuilder();

            if (document is Article)
            {
                var article = document as Article;

                sb.Append(BuildAuthorsCitation(article.Authors));
                sb.Append(string.Format("({0}) {1} in Museum Victoria Collections {2}{3} Accessed {4}", (!string.IsNullOrWhiteSpace(article.YearWritten)) ? article.YearWritten : article.DateModified.Year.ToString(), article.Title, helper.RenderContext.Context.Request.Url.SiteBase, helper.RenderContext.Context.Request.Path, DateTime.UtcNow.ToString("dd MMMM yyyy")));
            }
            else if(document is Species)
            {
                var species = document as Species;

                sb.Append(BuildAuthorsCitation(species.Authors));
                sb.Append(string.Format("({0}) ", species.DateModified.Year));

                if (!string.IsNullOrWhiteSpace(species.Taxonomy.TaxonName))
                    sb.Append(string.Format("<em>{0}</em> ", species.Taxonomy.TaxonName));
                if (!string.IsNullOrWhiteSpace(species.Taxonomy.CommonName))
                    sb.Append(string.Format("{0} ", species.Taxonomy.CommonName));

                sb.Append(string.Format("in Museum Victoria Collections {0}{1} Accessed {2}", helper.RenderContext.Context.Request.Url.SiteBase, helper.RenderContext.Context.Request.Path, DateTime.UtcNow.ToString("dd MMMM yyyy")));
            }
            else if(document is Item || document is Specimen)
            {
                sb.Append(string.Format("Museum Victoria Collections {0}{1} Accessed {2}", helper.RenderContext.Context.Request.Url.SiteBase, helper.RenderContext.Context.Request.Path, DateTime.UtcNow.ToString("dd MMMM yyyy")));
            }
            else if(document is Collection)
            {
                var collection = document as Collection;

                sb.Append(BuildAuthorsCitation(collection.Authors));
                sb.Append(string.Format("({0}) {1} in Museum Victoria Collections {2}{3} Accessed {4}", collection.DateModified.Year, collection.Title, helper.RenderContext.Context.Request.Url.SiteBase, helper.RenderContext.Context.Request.Path, DateTime.UtcNow.ToString("dd MMMM yyyy")));
            }

            return new NonEncodedHtmlString(sb.ToString());
        }

        public static IHtmlString RenderLocationExternalLink<T>(this HtmlHelpers<T> helper, MuseumLocation museumLocation)
        {
            var sb = new StringBuilder();

            var formatString = "<a class=\"website icon-externallink\" href=\"http://museumvictoria.com.au/{0}/\">Visit {1}</a>";

            switch (museumLocation.DisplayLocation)
            {
                case "Bunjilaka":
                    sb.Append(string.Format(formatString, "bunjilaka", museumLocation.DisplayLocation));
                    break;
                case "Discovery Centre":
                    sb.Append(string.Format(formatString, "melbournemuseum", museumLocation.Venue));
                    break;
                case "Immigration Discovery Centre":
                    sb.Append(string.Format(formatString, "immigrationmuseum", museumLocation.Venue));
                    break;
                case "Melbourne Museum":
                    sb.Append(string.Format(formatString, "melbournemuseum", museumLocation.DisplayLocation));
                    break;
                case "Immigration Museum":
                    sb.Append(string.Format(formatString, "immigrationmuseum", museumLocation.DisplayLocation));
                    break;
                case "Royal Exhibition Building":
                    sb.Append(string.Format(formatString, "reb", museumLocation.DisplayLocation));
                    break;
                case "Scienceworks":
                    sb.Append(string.Format(formatString, "scienceworks", museumLocation.DisplayLocation));
                    break;
            }

            return new NonEncodedHtmlString(sb.ToString());
        }

        public static IHtmlString ConvertNewlines<T>(this HtmlHelpers<T> helper, string content)
        {
            return new NonEncodedHtmlString(content.Replace("\n", "<br />"));
        }

        public static IHtmlString ConvertHyperLinks<T>(this HtmlHelpers<T> helper, string content)
        {
            var result = new StringBuilder(content);
            var regexResult = Regex.Matches(content, @"((ht|f)tp(s?)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z‌​0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*)");

            for (int i = 0; i < regexResult.Count; i++)
            {
                result.Replace(regexResult[i].Value, string.Format("<a href=\"{0}\">[Link {1}]</a>", regexResult[i].Value, i+1));
            }

            return new NonEncodedHtmlString(result.ToString());
        }

        public static IHtmlString ConvertToJson<T>(this HtmlHelpers<T> helper, object input)
        {
            return new NonEncodedHtmlString(JsonConvert.SerializeObject(input, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects }));
        }
         
        private static string BuildAuthorsCitation(IList<Author> authors)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < authors.Count; i++)
            {
                if (authors[i].FirstName != null && authors[i].LastName != null)
                {
                    sb.Append(string.Format("{0}, {1}.", authors[i].LastName, authors[i].FirstName.Substring(0, 1)));

                    if (i < authors.Count - 1)
                    {
                        sb.Append(", ");

                        if (i == authors.Count - 2)
                            sb.Append(" and ");
                    }
                    else
                    {
                        sb.Append(" ");
                    }
                }
            }

            return sb.ToString();
        }
    }
}