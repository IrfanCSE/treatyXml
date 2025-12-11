namespace TreatyXml.Models
{
    /// <summary>
    /// Represents treaty data including metadata and articles
    /// </summary>
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

    /// <summary>
    /// Represents an article within a treaty
    /// </summary>
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

    /// <summary>
    /// Represents a protocol within a treaty
    /// </summary>
    public class Protocol
    {
        public string ProtocolNumber { get; set; }
        public string ProtocolTitle { get; set; }
        public string ProtocolDescription { get; set; }
        public int TreatyDtaMetadataId { get; set; }
        public int Id { get; set; }
    }
}
