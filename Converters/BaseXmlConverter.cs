using System;
using System.Text.Json;
using System.Xml.Linq;

namespace TreatyXml.Converters
{
    /// <summary>
    /// Base class for XML converters providing common functionality
    /// </summary>
    /// <typeparam name="T">The type of data to convert</typeparam>
    public abstract class BaseXmlConverter<T> : IXmlConverter<T>
    {
        /// <summary>
        /// JSON serializer options for deserialization
        /// </summary>
        protected virtual JsonSerializerOptions JsonOptions => new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Converts JSON content to an XML document
        /// </summary>
        public XDocument ConvertToXml(string jsonContent)
        {
            if (string.IsNullOrWhiteSpace(jsonContent))
                throw new ArgumentException("JSON content cannot be null or empty", nameof(jsonContent));

            var data = JsonSerializer.Deserialize<T>(jsonContent, JsonOptions);
            return ConvertToXml(data);
        }

        /// <summary>
        /// Converts a strongly-typed object to an XML document
        /// </summary>
        public abstract XDocument ConvertToXml(T data);

        /// <summary>
        /// Creates an XDocument with DOCTYPE declaration
        /// </summary>
        protected XDocument CreateDocumentWithDocType(XElement root, string rootElementName, string publicId, string systemId)
        {
            return new XDocument(
                new XDocumentType(rootElementName, publicId, systemId, null),
                root
            );
        }
    }
}
