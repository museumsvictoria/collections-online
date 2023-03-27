using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Models;
using Nancy.Helpers;
using Nancy.ViewEngines.Razor;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
            var currentUrl = string.Format("{0}{1}", ConfigurationManager.AppSettings["CanonicalSiteBase"], helper.RenderContext.Context.Request.Path);

            var sb = new StringBuilder();

            if (document is Article)
            {
                var article = document as Article;

                sb.Append(BuildAuthorsCitation(article.Authors));
                sb.Append(string.Format("({0}) {1} in Museums Victoria Collections {2}<br/>Accessed {3}", !string.IsNullOrWhiteSpace(article.YearWritten) ? article.YearWritten : article.DateModified.Year.ToString(), article.Title, currentUrl, DateTime.Now.ToString("dd MMMM yyyy")));
            }
            else if(document is Species)
            {
                var species = document as Species;

                sb.Append(BuildAuthorsCitation(species.Authors));
                sb.Append(string.Format("({0}) ", (!string.IsNullOrWhiteSpace(species.YearWritten)) ? species.YearWritten : species.DateModified.Year.ToString()));

                if (species.Taxonomy != null)
                {
                    if (!string.IsNullOrWhiteSpace(species.Taxonomy.TaxonName))
                        sb.Append(string.Format("<em>{0}</em> ", species.Taxonomy.TaxonName));
                    if (!string.IsNullOrWhiteSpace(species.Taxonomy.CommonName))
                        sb.Append(string.Format("{0} ", species.Taxonomy.CommonName));
                }

                sb.Append(string.Format("in Museums Victoria Collections {0}<br/>Accessed {1}", currentUrl, DateTime.Now.ToString("dd MMMM yyyy")));
            }
            else if(document is Item || document is Specimen)
            {
                sb.Append(string.Format("Museums Victoria Collections {0}<br/>Accessed {1}", currentUrl, DateTime.Now.ToString("dd MMMM yyyy")));
            }
            else if(document is Collection)
            {
                var collection = document as Collection;

                sb.Append(BuildAuthorsCitation(collection.Authors));
                sb.Append(string.Format("({0}) {1} in Museums Victoria Collections {2}<br/>Accessed {3}", !string.IsNullOrWhiteSpace(collection.YearWritten) ? collection.YearWritten : collection.DateModified.Year.ToString(), collection.Title, currentUrl, DateTime.Now.ToString("dd MMMM yyyy")));
            }

            return new NonEncodedHtmlString(sb.ToString());
        }

        public static IHtmlString RenderCurrentPath<T>(this HtmlHelpers<T> helper)
        {
            return new NonEncodedHtmlString(string.Format("{0}{1}", ConfigurationManager.AppSettings["CanonicalSiteBase"], helper.RenderContext.Context.Request.Path));
        }

        public static IHtmlString RenderLocationExternalLink<T>(this HtmlHelpers<T> helper, MuseumLocation museumLocation)
        {
            var sb = new StringBuilder();

            var formatString = "<a class=\"website icon-externallink\" href=\"https://museumsvictoria.com.au/{0}/\">Visit {1}</a>";

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
            return new NonEncodedHtmlString(content?.Replace("\n", "<br />"));
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
            var jsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
            jsonSerializerSettings.Converters.Add(new StringEnumConverter());

            return new NonEncodedHtmlString(JsonConvert.SerializeObject(input, jsonSerializerSettings));
        }

        public static IHtmlString RenderStorages<T>(this HtmlHelpers<T> helper, IList<Storage> storages)
        {
            var sb = new StringBuilder();

            var storagesString = storages.Select(x => new[]
            {
                string.IsNullOrWhiteSpace(x.Nature) ? null : $"Nature: {x.Nature}",
                string.IsNullOrWhiteSpace(x.Form) ? null : $"Form: {x.Form}",
                string.IsNullOrWhiteSpace(x.FixativeTreatment) ? null : $"Fixative treatment: {x.FixativeTreatment}",
                string.IsNullOrWhiteSpace(x.Medium) ? null : $"Medium: {x.Medium}",
            }.Concatenate(", ")).Concatenate("; ");

            sb.Append(storagesString);

            return new NonEncodedHtmlString(sb.ToString());
        }
        
        public static IHtmlString RenderTissues<T>(this HtmlHelpers<T> helper, IList<Tissue> tissues)
        {
            var sb = new StringBuilder();

            var tissuesString = tissues.Select(x => new[]
            {
                string.IsNullOrWhiteSpace(x.TissueType) ? null : $"Type: {x.TissueType}",
                string.IsNullOrWhiteSpace(x.Preservative) ? null : $"Preservative: {x.Preservative}",
                string.IsNullOrWhiteSpace(x.StorageTemperature) ? null : $"Storage temperature: {x.StorageTemperature}",
            }.Concatenate(", ")).Concatenate("; ");

            sb.Append(tissuesString);

            return new NonEncodedHtmlString(sb.ToString());
        }

        private static string BuildAuthorsCitation(IList<Author> authors)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < authors.Count; i++)
            {
                var author = new[]
                {
                    authors[i].LastName,
                    (!string.IsNullOrWhiteSpace(authors[i].FirstName)) ? authors[i].FirstName.Substring(0, 1) : null
                }.Concatenate(", ");
                
                author = !string.IsNullOrWhiteSpace(author) ? string.Format("{0}.", author) : authors[i].FullName;

                if (!string.IsNullOrWhiteSpace(author))
                {
                    sb.Append(author);

                    if (i < authors.Count - 1)
                        sb.Append(i == authors.Count - 2 ? " and " : ", ");
                    else
                        sb.Append(" ");
                }
            }

            return sb.ToString();
        }
    }
}