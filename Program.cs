using System.Xml;
using System.Xml.Linq;
using TreatyXml.Converters;

// Create converter instance
var converter = new TreatyXmlConverter();

// Read JSON content
string jsonContent = System.IO.File.ReadAllText("H:\\Github\\Converter\\TreatyXml\\treaty.json");

// Convert to XML
XDocument xmlDoc = converter.ConvertToXml(jsonContent);

// Save with proper formatting
var settings = new XmlWriterSettings
{
    Indent = true,
    IndentChars = "  ",
    OmitXmlDeclaration = false,
    Encoding = System.Text.Encoding.UTF8
};

using (var writer = XmlWriter.Create("H:\\Github\\Converter\\TreatyXml\\output.xml", settings))
{
    xmlDoc.Save(writer);
}

Console.WriteLine("Conversion completed!");