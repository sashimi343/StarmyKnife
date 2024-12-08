using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using StarmyKnife.Core.Contracts.Models;

namespace StarmyKnife.Core.Models
{
    public class XPathSearcher : IPathSearcher
    {
        private readonly XmlDocument _xml;

        public XPathSearcher()
        {
            _xml = new XmlDocument();
        }

        public XPathSearcher(string xml)
        {
            _xml = new XmlDocument();
            _xml.LoadXml(RemoveAllNamespaces(xml));
        }

        public bool TryLoadInput(string xml, out string error)
        {
            try
            {
                _xml.LoadXml(RemoveAllNamespaces(xml));
                System.Diagnostics.Debug.WriteLine(_xml.OuterXml);
                error = null;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public List<string> FindAllNodes(string xpath)
        {
            var nodes = _xml.SelectNodes(xpath);
            var nodeTexts = new List<string>();

            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                if (node is XmlElement element)
                {
                    nodeTexts.Add(element.OuterXml);
                }
                else if (node is XmlText text)
                {
                    nodeTexts.Add(text.Value);
                }
                else if (node != null)
                {
                    nodeTexts.Add(node.Value ?? node.OuterXml);
                }
            }

            return nodeTexts;
        }

        private static string RemoveAllNamespaces(string xmlDocument)
        {
            XElement xmlDocumentWithoutNs = RemoveAllNamespaces(XElement.Parse(xmlDocument));

            return xmlDocumentWithoutNs.ToString();
        }

        private static XElement RemoveAllNamespaces(XElement xmlDocument)
        {
            if (!xmlDocument.HasElements)
            {
                XElement xElement = new XElement(xmlDocument.Name.LocalName);
                xElement.Value = xmlDocument.Value;

                foreach (XAttribute attribute in xmlDocument.Attributes())
                    xElement.Add(attribute);

                return xElement;
            }
            return new XElement(xmlDocument.Name.LocalName, xmlDocument.Elements().Select(el => RemoveAllNamespaces(el)));
        }
    }
}
