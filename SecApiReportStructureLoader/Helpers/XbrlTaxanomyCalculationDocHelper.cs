using SecApiReportStructureLoader.Models;
using System.Collections.Generic;
using System.Xml;

namespace SecApiReportStructureLoader.Helpers
{
    public class XbrlTaxanomyCalculationDocHelper
    {
        public static Dictionary<string, FinancialStatementNode> GetCashFlowsNodes(XmlDocument taxanomyCalDocXml, string cashFlowUri)
        {
            XmlElement cashFlowStatementRootXmlNode = null;

            XmlElement root = taxanomyCalDocXml.DocumentElement;
            foreach (XmlElement currentNode in root.ChildNodes)
            {
                string nodeName = currentNode.Name;

                foreach (XmlAttribute attribute in currentNode.Attributes)
                {
                    string attributeName = attribute.Name;
                    string attributeValue = attribute.Value;

                    if (nodeName == "link:calculationLink" && attributeName == "xlink:role" && attributeValue == cashFlowUri)
                    {
                        cashFlowStatementRootXmlNode = currentNode;
                    }
                }
            }

            Dictionary<string, FinancialStatementNode> financialStatementPositions = new Dictionary<string, FinancialStatementNode>();
            // Find all cash flow statement nodes:
            foreach (XmlElement cashFlowNode in cashFlowStatementRootXmlNode.ChildNodes)
            {
                if (cashFlowNode.Name != "link:loc")
                {
                    continue;
                }

                string fullLabel = cashFlowNode.GetAttribute("xlink:label");
                string name = fullLabel.Split("_")[2];

                financialStatementPositions.Add(fullLabel, new FinancialStatementNode()
                {
                    FullLabel = fullLabel,
                    Name = name,
                    Children = new List<string>()
                });
            }

            // Find all links between cash flow statement nodes:
            foreach (XmlElement cashFlowNode in cashFlowStatementRootXmlNode.ChildNodes)
            {
                if (cashFlowNode.Name != "link:calculationArc")
                {
                    continue;
                }

                string arcFromPosition = cashFlowNode.GetAttribute("xlink:from");
                string arcToPosition = cashFlowNode.GetAttribute("xlink:to");

                financialStatementPositions[arcFromPosition]
                    .Children
                    .Add(arcToPosition);
            }

            return financialStatementPositions;
        }
    }
}
