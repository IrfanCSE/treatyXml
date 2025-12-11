using System.Xml.Linq;

namespace TreatyXml.Converters
{
    /// <summary>
    /// Interface for converting JSON data to XML documents
    /// </summary>
    /// <typeparam name="T">The type of data to convert</typeparam>
    public interface IXmlConverter<T>
    {
        /// <summary>
        /// Converts JSON content to an XML document
        /// </summary>
        /// <param name="jsonContent">JSON string to convert</param>
        /// <returns>XML document representation</returns>
        XDocument ConvertToXml(string jsonContent);
        
        /// <summary>
        /// Converts a strongly-typed object to an XML document
        /// </summary>
        /// <param name="data">Data object to convert</param>
        /// <returns>XML document representation</returns>
        XDocument ConvertToXml(T data);
    }
}
