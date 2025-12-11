using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using TreatyXml.Configuration;
using TreatyXml.Converters;
using TreatyXml.Helpers;
using TreatyXml.Models;

namespace TreatyXml.Converters
{
    /// <summary>
    /// Converter for transforming TreatyData JSON to XML format
    /// </summary>
    public class TreatyXmlConverter : BaseXmlConverter<TreatyData>
    {
        private readonly XmlNamespaceConfig _namespaces;

        public TreatyXmlConverter()
        {
            _namespaces = new XmlNamespaceConfig();
        }

        public override XDocument ConvertToXml(TreatyData treaty)
        {
            if (treaty == null)
                throw new ArgumentNullException(nameof(treaty));

            // Create root element with all namespaces
            var root = _namespaces.CreateRootElementWithNamespaces("ch");

            // Add no-title element
            root.Add(new XElement(_namespaces.Core + "no-title"));

            // Create legislation structure
            var legislation = new XElement(_namespaces.LnbLeg + "legislation",
                new XElement(_namespaces.LnbLeg + "international-legislation",
                    new XElement(_namespaces.LnbLeg + "international-agreement")));

            var agreement = legislation.Descendants(_namespaces.LnbLeg + "international-agreement").First();

            // Add prelims section
            var prelims = CreatePrelims(treaty);
            agreement.Add(prelims);

            // Add preamble section
            var preamble = CreatePreamble(treaty.Initials);
            agreement.Add(preamble);

            // Add main section with articles
            var main = CreateMain(treaty.Articles);
            agreement.Add(main);

            root.Add(legislation);

            // Create document with DOCTYPE
            return CreateDocumentWithDocType(
                root,
                "tr:ch",
                "-//LEXISNEXIS//DTD PLUTO v018//EN//XML",
                "C:\\Neptune\\NeptuneEditor\\doctypes\\plutoV018-0000\\plutoV018-0000.dtd"
            );
        }

        private XElement CreatePrelims(TreatyData treaty)
        {
            var prelims = new XElement(_namespaces.LnbLeg + "prelims");

            // Official name
            prelims.Add(new XElement(_namespaces.LnbLeg + "officialname",
                new XElement(_namespaces.Core + "title", TextHelper.NormalizeText(treaty.Title))));

            // Status
            prelims.Add(new XElement(_namespaces.LnbLeg + "approval", $"Status: {treaty.Status}"));

            // Signature date
            if (!string.IsNullOrEmpty(treaty.SignatureDate))
            {
                var sigDate = DateHelper.ParseDate(treaty.SignatureDate);
                prelims.Add(new XElement(_namespaces.LnbLeg + "laid",
                    "Signature Date: ",
                    new XElement(_namespaces.Core + "date",
                        new XAttribute("day", sigDate.Day),
                        new XAttribute("month", sigDate.Month),
                        new XAttribute("year", sigDate.Year),
                        treaty.SignatureDate)));
            }

            // Entry into force
            if (!string.IsNullOrEmpty(treaty.EntryIntoForce))
            {
                var entryDate = DateHelper.ParseDate(treaty.EntryIntoForce);
                prelims.Add(new XElement(_namespaces.LnbLeg + "made",
                    "Entry into Force: ",
                    new XElement(_namespaces.Core + "date",
                        new XAttribute("day", entryDate.Day),
                        new XAttribute("month", entryDate.Month),
                        new XAttribute("year", entryDate.Year),
                        treaty.EntryIntoForce)));
            }

            // Effective date
            if (!string.IsNullOrEmpty(treaty.EffectiveDate))
            {
                prelims.Add(new XElement(_namespaces.LnbLeg + "operation",
                    CreateEffectiveDateElements(treaty.EffectiveDate)));
            }

            return prelims;
        }

        private object[] CreateEffectiveDateElements(string effectiveDate)
        {
            var elements = new List<object>();
            elements.Add("Effective Date: ");

            // Parse multiple dates if separated by semicolon
            var parts = effectiveDate.Split(';');

            foreach (var part in parts)
            {
                var trimmed = part.Trim();
                var match = Regex.Match(trimmed, @"(\d+)\s+(\w+)\s+(\d{4})");

                if (match.Success)
                {
                    var day = match.Groups[1].Value;
                    var month = match.Groups[2].Value;
                    var shortMonth = DateHelper.ConvertToShortMonth(match.Groups[2].Value);
                    var year = match.Groups[3].Value;

                    // Create the date text content
                    var dateText = $"{day} {month} {year}";

                    var dateElement = new XElement(_namespaces.Core + "date",
                        new XAttribute("day", day),
                        new XAttribute("month", shortMonth),
                        new XAttribute("year", year),
                        dateText);

                    var remainingText = trimmed.Substring(trimmed.IndexOf(year) + 4).Trim();

                    elements.Add(dateElement);

                    if (!string.IsNullOrEmpty(remainingText))
                    {
                        elements.Add($" {remainingText}");
                    }
                }
            }

            return elements.ToArray();
        }

        private XElement CreatePreamble(string initials)
        {
            var preamble = new XElement(_namespaces.LnbLeg + "preamble");

            if (!string.IsNullOrEmpty(initials))
            {
                var paragraphs = HtmlHelper.ParseHtmlParagraphs(initials);

                foreach (var paragraph in paragraphs)
                {
                    preamble.Add(new XElement(_namespaces.LnbLeg + "para1", paragraph));
                }
            }

            return preamble;
        }

        private XElement CreateMain(Article[] articles)
        {
            var main = new XElement(_namespaces.LnbLeg + "main");

            if (articles == null || articles.Length == 0)
                return main;

            foreach (var article in articles)
            {
                var provision = new XElement(_namespaces.LnbLeg + "provision");

                // Add article number
                provision.Add(new XElement(_namespaces.Core + "desig",
                    new XAttribute("value", article.ArticleNumber),
                    article.ArticleNumber));

                // Add article title
                provision.Add(new XElement(_namespaces.Core + "title", article.ArticleTitle));

                // Parse and add article content
                if (!string.IsNullOrEmpty(article.ArticleDescription))
                {
                    var contentElements = ParseArticleContent(article.ArticleDescription);
                    provision.Add(contentElements);
                }

                main.Add(provision);
            }

            return main;
        }

        private XElement[] ParseArticleContent(string html)
        {
            var elements = new List<XElement>();

            // Sanitize HTML before parsing
            html = HtmlHelper.SanitizeHtml(html);

            // Remove outer <p> tags and split by paragraph
            var doc = new XmlDocument();
            doc.LoadXml($"<root>{html}</root>");

            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                if (node.Name == "p")
                {
                    var className = node.Attributes?["class"]?.Value ?? "";
                    var text = node.InnerText;

                    // Detect enumeration pattern
                    var (isEnumerated, enumValue, content) = HtmlHelper.DetectEnumeration(text);

                    if (isEnumerated)
                    {
                        XElement element = DetermineParaLevel(className, enumValue, content);
                        elements.Add(element);
                    }
                    else
                    {
                        // No enumeration - just add as para1
                        elements.Add(new XElement(_namespaces.LnbLeg + "para1", text));
                    }
                }
            }

            return elements.ToArray();
        }

        private XElement DetermineParaLevel(string className, string enumValue, string content)
        {
            if (className.Contains("indent-3"))
            {
                return new XElement(_namespaces.LnbLeg + "para4",
                    new XElement(_namespaces.Core + "enum", enumValue),
                    content);
            }
            else if (className.Contains("indent-2"))
            {
                return new XElement(_namespaces.LnbLeg + "para3",
                    new XElement(_namespaces.Core + "enum", enumValue),
                    content);
            }
            else if (className.Contains("indent-1"))
            {
                return new XElement(_namespaces.LnbLeg + "para2",
                    new XElement(_namespaces.Core + "enum", enumValue),
                    content);
            }
            else
            {
                return new XElement(_namespaces.LnbLeg + "para1",
                    new XElement(_namespaces.Core + "enum", enumValue),
                    content);
            }
        }
    }
}
