using SecApiFinancialStatementLoader.IServices;
using SecApiFinancialStatementLoader.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

namespace SecApiFinancialStatementLoader.Services
{
    /// <summary>
    /// Class for operations with "XBRL TAXONOMY EXTENSION CALCULATION LINKBASE DOCUMENT" entities
    /// </summary>
    public class TaxonomyExtLinkbaseService : ITaxonomyExtLinkbaseService
    {
        private readonly ISecApiClient _secApiClient;

        public TaxonomyExtLinkbaseService(ISecApiClient secApiClient)
        {
            _secApiClient = secApiClient;
        }

        /// <inheritdoc />
        public async Task <Dictionary<string, FinancialStatementNode>> GetFinancialStatementStructure(
            FinancialStatementDetails finStatementDetails,
            ReportDetails reportDetails,
            string financialStatementUri,
            Action<string> logger)
        {
            // Get "XBRL TAXONOMY EXTENSION CALCULATION LINKBASE DOCUMENT" for the current report:
            string taxanomyCalculationLinkbaseStr = await _secApiClient
                .RetrieveTaxanomyCalDocXml(
                    finStatementDetails.CikNumber,
                    reportDetails.AccessionNumber,
                    finStatementDetails.TickerSymbol,
                    reportDetails.ReportDate,
                    logger);

            // Parse into XML doc:
            XmlDocument taxanomyCalculationLinkbaseXml = new XmlDocument();
            taxanomyCalculationLinkbaseXml.LoadXml(taxanomyCalculationLinkbaseStr);

            return ConstructFinStatementStructure(
                taxanomyCalculationLinkbaseXml,
                financialStatementUri);
        }

        private Dictionary<string, FinancialStatementNode> ConstructFinStatementStructure(
            XmlDocument taxanomyCalculationLinkbaseXml,
            string financialStatementUri)
        {
            // Trying to find XML node that reflects current financial statement "head node" (or "root node"):
            XmlElement finStatementRootXmlNode = null;

            XmlElement root = taxanomyCalculationLinkbaseXml.DocumentElement;
            foreach (XmlElement currentNode in root.ChildNodes)
            {
                string nodeName = currentNode.Name;

                foreach (XmlAttribute attribute in currentNode.Attributes)
                {
                    string attributeName = attribute.Name;
                    string attributeValue = attribute.Value;

                    if (nodeName.Contains("calculationLink") && attributeName == "xlink:role" && attributeValue == financialStatementUri)
                    {
                        finStatementRootXmlNode = currentNode;
                    }
                }
            }


            // Construct financial statement structure:
            Dictionary<string, FinancialStatementNode> financialStatementTree =
                new Dictionary<string, FinancialStatementNode>();

            // Find all given financial statement's nodes:
            foreach (XmlElement finStatementNode in finStatementRootXmlNode.ChildNodes)
            {
                if (!finStatementNode.Name.Contains("loc"))
                {
                    continue;
                }

                string finPositionFullLabel = finStatementNode.GetAttribute("xlink:label");
                string finPositionName = GetFinancialPositionName(finPositionFullLabel);

                // Sometimes companies file the same financial position twice - we want to exclude duplications
                if (financialStatementTree.ContainsKey(finPositionName))
                {
                    continue;
                }

                financialStatementTree.Add(finPositionName, new FinancialStatementNode()
                {
                    Name = finPositionName,
                    Children = new HashSet<string>()
                });
            }

            // Find all links between financial statement nodes:
            foreach (XmlElement finStatementNode in finStatementRootXmlNode.ChildNodes)
            {
                if (!finStatementNode.Name.Contains("calculationArc"))
                {
                    continue;
                }

                string arcFromPositionFullLabel = finStatementNode.GetAttribute("xlink:from");
                string arcToPositionFullLabel = finStatementNode.GetAttribute("xlink:to");

                string parentFinPosName = GetFinancialPositionName(arcFromPositionFullLabel);
                string childFinPosLabel = GetFinancialPositionName(arcToPositionFullLabel);

                // Just like before - trying to avoid duplicates if there are any:
                var children = financialStatementTree[parentFinPosName].Children;
                if (children.Contains(childFinPosLabel))
                {
                    continue;
                }

                children.Add(childFinPosLabel);
            }

            return financialStatementTree;
        }

        private string GetFinancialPositionName(string finPositionFullLabel)
        {
            // Taking part of the label that is right next to "us-gaap" - this is an actual name of the financial position:
            string[] finStatementNodeLabelArr = finPositionFullLabel.Split("_");
            int taxonomyLableIndex = Array.IndexOf(finStatementNodeLabelArr, "us-gaap");
            string finPositionName = finStatementNodeLabelArr[taxonomyLableIndex + 1];

            return finPositionName;
        }
    }
}
