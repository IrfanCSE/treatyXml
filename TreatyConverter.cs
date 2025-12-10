using System;
using System.Xml;
using System.Xml.Linq;
using System.Text.Json;
using System.Linq;
using System.Text.RegularExpressions;

public class TreatyConverter
{
    public static XDocument ConvertTreatyJsonToXml(string jsonContent)
    {
        var treaty = JsonSerializer.Deserialize<TreatyData>(jsonContent);
        
        // Create XML namespaces
        XNamespace core = "http://www.lexisnexis.com/namespace/sslrp/core";
        XNamespace lnbLeg = "http://www.lexisnexis.com/namespace/sslrp/lnb-leg";
        XNamespace tr = "http://www.lexisnexis.com/namespace/sslrp/tr";
        
        // Create root element with all namespaces
        var root = new XElement(tr + "ch",
            new XAttribute(XNamespace.Xmlns + "core", core.NamespaceName),
            new XAttribute(XNamespace.Xmlns + "di", "http://www.lexisnexis.com/namespace/sslrp/di"),
            new XAttribute(XNamespace.Xmlns + "em", "http://www.lexisnexis.com/namespace/sslrp/em"),
            new XAttribute(XNamespace.Xmlns + "fm", "http://www.lexisnexis.com/namespace/sslrp/fm"),
            new XAttribute(XNamespace.Xmlns + "fn", "http://www.lexisnexis.com/namespace/sslrp/fn"),
            new XAttribute(XNamespace.Xmlns + "form", "http://www.lexisnexis.com/namespace/sslrp/form"),
            new XAttribute(XNamespace.Xmlns + "glph", "http://www.lexisnexis.com/namespace/sslrp/glph"),
            new XAttribute(XNamespace.Xmlns + "header", "http://www.lexisnexis.com/namespace/sslrp/header"),
            new XAttribute(XNamespace.Xmlns + "in", "http://www.lexisnexis.com/namespace/sslrp/in"),
            new XAttribute(XNamespace.Xmlns + "lnb-case", "http://www.lexisnexis.com/namespace/case/lnb-case"),
            new XAttribute(XNamespace.Xmlns + "lnb-leg", lnbLeg.NamespaceName),
            new XAttribute(XNamespace.Xmlns + "lnbdig-case", "http://www.lexisnexis.com/namespace/digest/lnbdig-case"),
            new XAttribute(XNamespace.Xmlns + "lnci", "http://www.lexisnexis.com/namespace/common/lnci"),
            new XAttribute(XNamespace.Xmlns + "ls", "http://www.lexisnexis.com/namespace/sslrp/ls"),
            new XAttribute(XNamespace.Xmlns + "m", "http://www.w3.org/1998/Math/MathML"),
            new XAttribute(XNamespace.Xmlns + "nl", "http://www.lexisnexis.com/namespace/sslrp/nl"),
            new XAttribute(XNamespace.Xmlns + "pnfo", "http://www.lexisnexis.com/namespace/sslrp/pnfo"),
            new XAttribute(XNamespace.Xmlns + "ps", "http://www.lexisnexis.com/namespace/sslrp/ps"),
            new XAttribute(XNamespace.Xmlns + "pu", "http://www.lexisnexis.com/namespace/sslrp/pu"),
            new XAttribute(XNamespace.Xmlns + "se", "http://www.lexisnexis.com/namespace/sslrp/se"),
            new XAttribute(XNamespace.Xmlns + "su", "http://www.lexisnexis.com/namespace/sslrp/su"),
            new XAttribute(XNamespace.Xmlns + "tr", tr.NamespaceName)
        );
        
        // Add no-title element
        root.Add(new XElement(core + "no-title"));
        
        // Create legislation structure
        var legislation = new XElement(lnbLeg + "legislation",
            new XElement(lnbLeg + "international-legislation",
                new XElement(lnbLeg + "international-agreement")));
        
        var agreement = legislation.Descendants(lnbLeg + "international-agreement").First();
        
        // Add prelims section
        var prelims = CreatePrelims(treaty, core, lnbLeg);
        agreement.Add(prelims);
        
        // Add preamble section
        var preamble = CreatePreamble(treaty.Initials, lnbLeg);
        agreement.Add(preamble);
        
        // Add main section with articles
        var main = CreateMain(treaty.Articles, core, lnbLeg);
        agreement.Add(main);
        
        root.Add(legislation);
        
        // Create document with DOCTYPE
        var doc = new XDocument(
            new XDocumentType("tr:ch", "-//LEXISNEXIS//DTD PLUTO v018//EN//XML", 
                "C:\\Neptune\\NeptuneEditor\\doctypes\\plutoV018-0000\\plutoV018-0000.dtd", null),
            root
        );
        
        return doc;
    }
    
    private static XElement CreatePrelims(TreatyData treaty, XNamespace core, XNamespace lnbLeg)
    {
        var prelims = new XElement(lnbLeg + "prelims");
        
        // Official name
        prelims.Add(new XElement(lnbLeg + "officialname",
            new XElement(core + "title", treaty.Title)));
        
        // Status
        prelims.Add(new XElement(lnbLeg + "approval", $"Status: {treaty.Status}"));
        
        // Signature date
        if (!string.IsNullOrEmpty(treaty.SignatureDate))
        {
            var sigDate = ParseDate(treaty.SignatureDate);
            prelims.Add(new XElement(lnbLeg + "laid",
                "Signature Date: ",
                new XElement(core + "date",
                    new XAttribute("day", sigDate.Day),
                    new XAttribute("month", sigDate.Month),
                    new XAttribute("year", sigDate.Year),
                    treaty.SignatureDate)));
        }
        
        // Entry into force
        if (!string.IsNullOrEmpty(treaty.EntryIntoForce))
        {
            var entryDate = ParseDate(treaty.EntryIntoForce);
            prelims.Add(new XElement(lnbLeg + "made",
                "Entry into Force: ",
                new XElement(core + "date",
                    new XAttribute("day", entryDate.Day),
                    new XAttribute("month", entryDate.Month),
                    new XAttribute("year", entryDate.Year),
                    treaty.EntryIntoForce)));
        }
        
        // Effective date
        if (!string.IsNullOrEmpty(treaty.EffectiveDate))
        {
            prelims.Add(new XElement(lnbLeg + "operation",
                CreateEffectiveDateElements(treaty.EffectiveDate, core)));
        }
        
        return prelims;
    }
    
    private static object[] CreateEffectiveDateElements(string effectiveDate, XNamespace core)
    {
        var elements = new System.Collections.Generic.List<object>();
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
                var year = match.Groups[3].Value;
                
                var dateElement = new XElement(core + "date",
                    new XAttribute("day", day),
                    new XAttribute("month", month),
                    new XAttribute("year", year));
                
                // Add the date text
                var dateText = $"{day} {month} {year}";
                var remainingText = trimmed.Substring(trimmed.IndexOf(year) + 4).Trim();
                
                if (!string.IsNullOrEmpty(remainingText))
                {
                    elements.Add(dateElement);
                    elements.Add($" {remainingText}");
                }
                else
                {
                    elements.Add(dateElement);
                }
            }
        }
        
        return elements.ToArray();
    }
    
    private static XElement CreatePreamble(string initials, XNamespace lnbLeg)
    {
        var preamble = new XElement(lnbLeg + "preamble");
        
        if (!string.IsNullOrEmpty(initials))
        {
            // Parse HTML paragraphs
            var paragraphs = ParseHtmlParagraphs(initials);
            
            foreach (var paragraph in paragraphs)
            {
                preamble.Add(new XElement(lnbLeg + "para1", paragraph));
            }
        }
        
        return preamble;
    }
    
    private static XElement CreateMain(Article[] articles, XNamespace core, XNamespace lnbLeg)
    {
        var main = new XElement(lnbLeg + "main");
        
        foreach (var article in articles)
        {
            var provision = new XElement(lnbLeg + "provision");
            
            // Add article number
            provision.Add(new XElement(core + "desig",
                new XAttribute("value", article.ArticleNumber),
                article.ArticleNumber));
            
            // Add article title
            provision.Add(new XElement(core + "title", article.ArticleTitle));
            
            // Parse and add article content
            if (!string.IsNullOrEmpty(article.ArticleDescription))
            {
                var contentElements = ParseArticleContent(article.ArticleDescription, core, lnbLeg);
                provision.Add(contentElements);
            }
            
            main.Add(provision);
        }
        
        return main;
    }
    
    private static XElement[] ParseArticleContent(string html, XNamespace core, XNamespace lnbLeg)
    {
        var elements = new System.Collections.Generic.List<XElement>();
        
        // Sanitize HTML before parsing
        html = SanitizeHtml(html);
        
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
                var enumMatch = Regex.Match(text, @"^(\d+\.|[a-z]\)|\([ivx]+\))\s*(.*)$", RegexOptions.Singleline);
                
                if (enumMatch.Success)
                {
                    var enumValue = enumMatch.Groups[1].Value;
                    var content = enumMatch.Groups[2].Value;
                    
                    XElement element;
                    
                    if (className.Contains("indent-3"))
                    {
                        element = new XElement(lnbLeg + "para3",
                            new XElement(core + "enum", enumValue),
                            content);
                    }
                    else if (className.Contains("indent-2"))
                    {
                        element = new XElement(lnbLeg + "para2",
                            new XElement(core + "enum", enumValue),
                            content);
                    }
                    else if (className.Contains("indent-1"))
                    {
                        element = new XElement(lnbLeg + "para2",
                            new XElement(core + "enum", enumValue),
                            content);
                    }
                    else
                    {
                        element = new XElement(lnbLeg + "para1",
                            new XElement(core + "enum", enumValue),
                            content);
                    }
                    
                    elements.Add(element);
                }
                else
                {
                    // No enumeration - just add as para1
                    elements.Add(new XElement(lnbLeg + "para1", text));
                }
            }
        }
        
        return elements.ToArray();
    }
    
    private static string[] ParseHtmlParagraphs(string html)
    {
        var paragraphs = new System.Collections.Generic.List<string>();
        
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
    
    private static string SanitizeHtml(string html)
    {
        if (string.IsNullOrEmpty(html))
            return html;
        
        // Replace self-closing tags with proper closing tags
        html = Regex.Replace(html, @"<br\s*/?>" , "<br></br>", RegexOptions.IgnoreCase);
        html = Regex.Replace(html, @"<hr\s*/?>" , "<hr></hr>", RegexOptions.IgnoreCase);
        html = Regex.Replace(html, @"<img([^>]*)/?>", "<img$1></img>", RegexOptions.IgnoreCase);
        
        // Handle common HTML entities
        html = html.Replace("&nbsp;", "&#160;");
        html = html.Replace("&", "&amp;").Replace("&amp;amp;", "&amp;").Replace("&amp;#", "&#");
        
        return html;
    }
    
    private static (int Day, string Month, int Year) ParseDate(string dateStr)
    {
        var match = Regex.Match(dateStr, @"(\d+)\s+(\w+)\s+(\d{4})");
        
        if (match.Success)
        {
            return (
                int.Parse(match.Groups[1].Value),
                match.Groups[2].Value,
                int.Parse(match.Groups[3].Value)
            );
        }
        
        return (1, "Jan", 2000); // Default
    }
}

// Data models
public class TreatyData
{
    public int HostCountryId { get; set; }
    public string HostCountryCode { get; set; }
    public string HostCountryName { get; set; }
    public int PartnerCountryId { get; set; }
    public string PartnerCountryCode { get; set; }
    public string PartnerCountryName { get; set; }
    public string Title { get; set; }
    public string Status { get; set; }
    public string SignatureDate { get; set; }
    public string EffectiveDate { get; set; }
    public string Initials { get; set; }
    public string EntryIntoForce { get; set; }
    public Article[] Articles { get; set; }
    public Protocol[] Protocols { get; set; }
    public int Id { get; set; }
}

public class Article
{
    public string ChapterNumber { get; set; }
    public string ChapterTitle { get; set; }
    public string ArticleNumber { get; set; }
    public string ArticleTitle { get; set; }
    public string ArticleDescription { get; set; }
    public int TreatyDtaMetadataId { get; set; }
    public int TreatyArticleCategoryId { get; set; }
    public string CategoryName { get; set; }
    public int Id { get; set; }
}

public class Protocol
{
    public string ProtocolNumber { get; set; }
    public string ProtocolTitle { get; set; }
    public string ProtocolDescription { get; set; }
    public int TreatyDtaMetadataId { get; set; }
    public int Id { get; set; }
}