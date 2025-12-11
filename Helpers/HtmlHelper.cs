using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace TreatyXml.Helpers
{
    /// <summary>
    /// Helper class for HTML parsing and sanitization
    /// </summary>
    public static class HtmlHelper
    {
        /// <summary>
        /// Sanitizes HTML by converting self-closing tags and handling entities
        /// </summary>
        public static string SanitizeHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
                return html;

            // Replace self-closing tags with proper closing tags
            html = Regex.Replace(html, @"<br\s*/?>", "<br></br>", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"<hr\s*/?>", "<hr></hr>", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"<img([^>]*)/?>", "<img$1></img>", RegexOptions.IgnoreCase);

            // Handle common HTML entities
            html = html.Replace("&nbsp;", "&#160;");
            html = html.Replace("&", "&amp;").Replace("&amp;amp;", "&amp;").Replace("&amp;#", "&#");

            return html;
        }

        /// <summary>
        /// Parses HTML paragraphs into an array of text strings
        /// </summary>
        public static string[] ParseHtmlParagraphs(string html)
        {
            var paragraphs = new List<string>();

            try
            {
                // Sanitize HTML before parsing
                html = SanitizeHtml(html);

                var doc = new XmlDocument();
                doc.LoadXml($"<root>{html}</root>");

                foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                {
                    if (node.Name == "p")
                    {
                        paragraphs.Add(node.InnerText.Trim());
                    }
                }
            }
            catch
            {
                // Fallback: simple text extraction
                var cleaned = Regex.Replace(html, @"<[^>]+>", "\n");
                paragraphs.AddRange(cleaned.Split('\n')
                    .Select(p => p.Trim())
                    .Where(p => !string.IsNullOrEmpty(p)));
            }

            return paragraphs.ToArray();
        }

        /// <summary>
        /// Detects if text contains enumeration pattern
        /// </summary>
        public static (bool IsEnumerated, string EnumValue, string Content) DetectEnumeration(string text)
        {
            var enumMatch = Regex.Match(text, @"^(\d+\.|[a-z]\)|\([ivx]+\))\s*(.*)$", RegexOptions.Singleline);

            if (enumMatch.Success)
            {
                return (true, enumMatch.Groups[1].Value, enumMatch.Groups[2].Value);
            }

            return (false, null, text);
        }
    }
}
