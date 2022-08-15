using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        
        public static string TruncateHtml(this string html, int maxLength, string trailingText = "")
        {
            if (string.IsNullOrEmpty(html) || html.Length <= maxLength) return html;

            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            if (doc.DocumentNode.InnerText.Length <= maxLength) return html;

            var textNodes = new LinkedList<HtmlNode>(doc.DocumentNode.TextDescendants());
            var precedingText = 0;
            var lastNode = textNodes.First;
            while (precedingText <= maxLength && lastNode != null)
            {
                var nodeTextLength = lastNode.Value.InnerText.Length;
                if (precedingText + nodeTextLength > maxLength)
                {
                    var truncatedText = TruncateWords(lastNode.Value.InnerText, maxLength - precedingText);

                    if (string.IsNullOrWhiteSpace(truncatedText) && lastNode.Previous != null)
                    {
                        // Put the ellipsis in the previous node and remove the empty node.
                        lastNode.Previous.Value.InnerHtml = lastNode.Previous.Value.InnerText.Trim() + trailingText;
                        lastNode.Value.InnerHtml = string.Empty;
                        lastNode = lastNode.Previous;
                    }
                    else
                    {
                        lastNode.Value.InnerHtml = truncatedText + trailingText;
                    }

                    break;
                }

                precedingText += nodeTextLength;
                lastNode = lastNode.Next;
            }

            // Remove all the nodes after lastNode
            if (lastNode != null) RemoveFollowingNodes(lastNode.Value);

            return doc.DocumentNode.InnerHtml;
        }
        
        private static IEnumerable<HtmlNode> TextDescendants(this HtmlNode root)
        {
            return root.Descendants().Where(n => n.NodeType == HtmlNodeType.Text && !string.IsNullOrWhiteSpace(n.InnerText));
        }

        private static void RemoveFollowingNodes(HtmlNode lastNode)
        {
            while (lastNode.NextSibling != null) lastNode.NextSibling.Remove();
            if (lastNode.ParentNode != null) RemoveFollowingNodes(lastNode.ParentNode);
        }

        private static string TruncateWords(string value, int length)
        {
            if (string.IsNullOrWhiteSpace(value) || length <= 0) return string.Empty;
            if (length > value.Length) return value;

            var endIndex = length;
            while (char.IsLetterOrDigit(value[endIndex-1]) && char.IsLetterOrDigit(value[endIndex]) && endIndex > 1) endIndex--;

            if (endIndex == 1) return string.Empty;
            return value.Substring(0, endIndex).Trim();
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
