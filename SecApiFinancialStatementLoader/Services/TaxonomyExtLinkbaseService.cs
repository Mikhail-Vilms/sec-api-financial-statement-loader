using SecApiFinancialStatementLoader.IServices;
using SecApiFinancialStatementLoader.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

namespace SecApiFinancialStatementLoader.Services
{
    public class TaxonomyExtLinkbaseService : ITaxonomyExtLinkbaseService
    {
        private readonly ISecApiClient _secApiClient;

        public TaxonomyExtLinkbaseService(ISecApiClient secApiClient)
        {
            _secApiClient = secApiClient;
        }

        public async Task<XmlDocument> RetrieveAndParse(
            string cikNumber,
            string tickerSymbol,
            string accessionNumber,
            string reportDate,
            Action<string> logger)
        {
            // Get "XBRL TAXONOMY EXTENSION CALCULATION LINKBASE DOCUMENT" for the current report:
            string taxanomyCalculationLinkbaseStr = await _secApiClient
                .RetrieveTaxanomyCalDocXml(
                    cikNumber,
                    accessionNumber,
                    tickerSymbol,
                    reportDate,
                    logger);

            // Parse into xml doc:
            XmlDocument taxanomyCalculationLinkbaseXml = new XmlDocument();
            taxanomyCalculationLinkbaseXml.LoadXml(taxanomyCalculationLinkbaseStr);

            return taxanomyCalculationLinkbaseXml;
        }

        public Dictionary<string, FinancialStatementNode> Get_FinancialStatementTree_From_TaxanomyLinkbaseDoc(
            XmlDocument taxanomyCalDocXml,
            string financialStatementUri,
            Action<string> logger)
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

                    if (nodeName.Contains("calculationLink") && attributeName == "xlink:role" && attributeValue == financialStatementUri)
                    {
                        cashFlowStatementRootXmlNode = currentNode;
                    }
                }
            }

            Dictionary<string, FinancialStatementNode> financialStatementTree =
                new Dictionary<string, FinancialStatementNode>();
            // Find all cash flow statement nodes:
            foreach (XmlElement cashFlowNode in cashFlowStatementRootXmlNode.ChildNodes)
            {
                if (!cashFlowNode.Name.Contains("loc"))
                {
                    continue;
                }

                string fullLabel = cashFlowNode.GetAttribute("xlink:label");
                string finPosLabel = fullLabel.Split("_")[2];

                if (financialStatementTree.ContainsKey(finPosLabel))
                {
                    continue;
                }

                financialStatementTree.Add(finPosLabel, new FinancialStatementNode()
                {
                    Name = finPosLabel,
                    Children = new HashSet<string>()
                });
            }

            // Find all links between cash flow statement nodes:
            foreach (XmlElement cashFlowNode in cashFlowStatementRootXmlNode.ChildNodes)
            {
                if (!cashFlowNode.Name.Contains("calculationArc"))
                {
                    continue;
                }

                string arcFromPosition = cashFlowNode.GetAttribute("xlink:from");
                string arcToPosition = cashFlowNode.GetAttribute("xlink:to");

                string parentFinPosLabel = arcFromPosition.Split("_")[2];
                string childFinPosLabel = arcToPosition.Split("_")[2];

                var children = financialStatementTree[parentFinPosLabel].Children;
                if (children.Contains(childFinPosLabel))
                {
                    continue;
                }

                children.Add(childFinPosLabel);
            }

            return financialStatementTree;
        }
    }
}
