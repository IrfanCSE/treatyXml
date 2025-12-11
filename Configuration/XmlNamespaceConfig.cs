using System.Xml.Linq;

namespace TreatyXml.Configuration
{
    /// <summary>
    /// Configuration class for XML namespaces used in treaty documents
    /// </summary>
    public class XmlNamespaceConfig
    {
        public XNamespace Core { get; }
        public XNamespace Di { get; }
        public XNamespace Em { get; }
        public XNamespace Fm { get; }
        public XNamespace Fn { get; }
        public XNamespace Form { get; }
        public XNamespace Glph { get; }
        public XNamespace Header { get; }
        public XNamespace In { get; }
        public XNamespace LnbCase { get; }
        public XNamespace LnbLeg { get; }
        public XNamespace LnbdigCase { get; }
        public XNamespace Lnci { get; }
        public XNamespace Ls { get; }
        public XNamespace M { get; }
        public XNamespace Nl { get; }
        public XNamespace Pnfo { get; }
        public XNamespace Ps { get; }
        public XNamespace Pu { get; }
        public XNamespace Se { get; }
        public XNamespace Su { get; }
        public XNamespace Tr { get; }

        public XmlNamespaceConfig()
        {
            Core = "http://www.lexisnexis.com/namespace/sslrp/core";
            Di = "http://www.lexisnexis.com/namespace/sslrp/di";
            Em = "http://www.lexisnexis.com/namespace/sslrp/em";
            Fm = "http://www.lexisnexis.com/namespace/sslrp/fm";
            Fn = "http://www.lexisnexis.com/namespace/sslrp/fn";
            Form = "http://www.lexisnexis.com/namespace/sslrp/form";
            Glph = "http://www.lexisnexis.com/namespace/sslrp/glph";
            Header = "http://www.lexisnexis.com/namespace/sslrp/header";
            In = "http://www.lexisnexis.com/namespace/sslrp/in";
            LnbCase = "http://www.lexisnexis.com/namespace/case/lnb-case";
            LnbLeg = "http://www.lexisnexis.com/namespace/sslrp/lnb-leg";
            LnbdigCase = "http://www.lexisnexis.com/namespace/digest/lnbdig-case";
            Lnci = "http://www.lexisnexis.com/namespace/common/lnci";
            Ls = "http://www.lexisnexis.com/namespace/sslrp/ls";
            M = "http://www.w3.org/1998/Math/MathML";
            Nl = "http://www.lexisnexis.com/namespace/sslrp/nl";
            Pnfo = "http://www.lexisnexis.com/namespace/sslrp/pnfo";
            Ps = "http://www.lexisnexis.com/namespace/sslrp/ps";
            Pu = "http://www.lexisnexis.com/namespace/sslrp/pu";
            Se = "http://www.lexisnexis.com/namespace/sslrp/se";
            Su = "http://www.lexisnexis.com/namespace/sslrp/su";
            Tr = "http://www.lexisnexis.com/namespace/sslrp/tr";
        }

        /// <summary>
        /// Creates a root element with all namespace declarations
        /// </summary>
        public XElement CreateRootElementWithNamespaces(string elementName)
        {
            return new XElement(Tr + elementName,
                new XAttribute(XNamespace.Xmlns + "core", Core.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "di", Di.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "em", Em.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "fm", Fm.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "fn", Fn.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "form", Form.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "glph", Glph.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "header", Header.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "in", In.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "lnb-case", LnbCase.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "lnb-leg", LnbLeg.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "lnbdig-case", LnbdigCase.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "lnci", Lnci.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "ls", Ls.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "m", M.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "nl", Nl.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "pnfo", Pnfo.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "ps", Ps.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "pu", Pu.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "se", Se.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "su", Su.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "tr", Tr.NamespaceName)
            );
        }
    }
}
