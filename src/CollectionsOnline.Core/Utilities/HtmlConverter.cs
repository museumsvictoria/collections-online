using System;
using System.Collections.Generic;
using System.IO;
using CollectionsOnline.Import.Utilities;
using Ganss.XSS;
using HtmlAgilityPack;

namespace CollectionsOnline.Core.Utilities
{
    public static class HtmlConverter
    {
        public static string HtmlToText(string html)
        {
            if (html == null)
                return null;

            var doc = new HtmlDocument();

            doc.LoadHtml(html);

            var sw = new StringWriter();

            ConvertTo(doc.DocumentNode, sw);

            return sw.ToString();
        }

        public static HtmlSanitizerResult HtmlSanitizer(string html)
        {
            var sanitizer = new HtmlSanitizer(DefaultAllowedTags, allowedAttributes:DefaultAllowedAttributes);

            sanitizer.KeepChildNodes = true;

            var result = new HtmlSanitizerResult();

            sanitizer.RemovingTag += (s, e) => { result.HasRemovedTag = true; };
            sanitizer.RemovingStyle += (s, e) => { result.HasRemovedStyle = true; };
            sanitizer.RemovingAttribute += (s, e) => { result.HasRemovedAttribute = true; };

            result.Html = sanitizer.Sanitize(html);

            return result;
        }

        private static void ConvertTo(HtmlNode node, StringWriter outText)
        {
            switch (node.NodeType)
            {
                case HtmlNodeType.Comment:
                    // don't output comments
                    break;

                case HtmlNodeType.Document:
                    ConvertContentTo(node, outText);
                    break;

                case HtmlNodeType.Text:
                    // get text
                    var html = ((HtmlTextNode)node).Text;

                    // is it in fact a special closing node output as text?
                    if (HtmlNode.IsOverlappedClosingElement(html))
                        break;

                    // check the text is meaningful and not a bunch of whitespaces
                    if (html.Trim().Length > 0)
                    {
                        outText.Write(HtmlEntity.DeEntitize(html));                            
                    }
                    break;

                case HtmlNodeType.Element:
                    if (node.HasChildNodes)
                    {
                        ConvertContentTo(node, outText);
                    }
                    switch (node.Name)
                    {
                        case "p":
                            // treat paragraphs as crlf
                            outText.Write(Environment.NewLine + Environment.NewLine);
                            break;
                    }

                    break;
            }
        }

        private static void ConvertContentTo(HtmlNode node, StringWriter outText)
        {
            foreach (var subnode in node.ChildNodes)
            {
                ConvertTo(subnode, outText);
            }
        }

        private static readonly ISet<string> DefaultAllowedTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "a",
            "abbr",
            "address",
            "area",
            "b",
            "blockquote",
            "br",
            "caption",
            "cite",
            "code",
            "col",
            "colgroup",
            "dd",
            "del",
            "dfn",
            "div",
            "dl",
            "dt",
            "em",
            "h1",
            "h2",
            "h3",
            "h4",
            "h5",
            "h6",
            "hr",
            "i",
            "ins",
            "kbd",
            "li",
            "menu",
            "ol",
            "p",
            "pre",
            "q",
            "s",
            "samp",
            "small",
            "span",
            "strong",
            "sub",
            "sup",
            "table",
            "tbody",
            "td",
            "textarea",
            "tfoot",
            "th",
            "thead",
            "tr",
            "u",
            "ul",
            "var",
            "section",
            "nav",
            "article",
            "aside",
            "header",
            "footer",
            "main",
            "figure",
            "figcaption",
            "data",
            "time",
            "mark",
            "ruby",
            "rt",
            "rp",
            "bdi",
            "wbr",
            "details",
            "summary",
            "menuitem"
        };

        private static readonly ISet<string> DefaultAllowedAttributes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "abbr",
            "accept",
            "accept-charset",
            "accesskey",
            "action",
            "align",
            "alt",
            "axis",
            "char",
            "charoff",
            "charset",
            "checked",
            "cite",
            "clear",
            "cols",
            "colspan",
            "compact",
            "coords",
            "datetime",
            "dir",
            "disabled",
            "enctype",
            "for",
            "headers",
            "href",
            "hreflang",
            "hspace",
            "ismap",
            "label",
            "lang",
            "longdesc",
            "maxlength",
            "media",
            "method",
            "multiple",
            "name",
            "nohref",
            "noshade",
            "nowrap",
            "prompt",
            "readonly",
            "rel",
            "rev",
            "rows",
            "rowspan",
            "rules",
            "scope",
            "selected",
            "shape",
            "span",
            "start",
            "summary",
            "tabindex",
            "target",
            "title",
            "type",
        };
    }
}
