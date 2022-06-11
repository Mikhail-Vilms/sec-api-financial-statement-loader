using SecApiFinancialStatementLoader.Models;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

namespace SecApiFinancialStatementLoader.Helpers
{
    public static class XblrTaxanomyDocsHelper
    {
        private static readonly Dictionary<string, List<string>> _statementTitles = new Dictionary<string, List<string>>()
        {
            {
                FinancialStatementType.IncomeStatement.ToString(),
                new List<string>()
                {
                    "StatementsOfEarnings", // used by GS
                    "StatementsOfOperations" // used by AAPL
                }
            },
            {
                FinancialStatementType.BalanceSheet.ToString(),
                new List<string>()
                {
                    "BalanceSheets"
                }
            },
            {
                FinancialStatementType.CashFlowStatement.ToString(),
                new List<string>()
                {
                    "StatementsOfCashFlows"
                }
            },
        };

        public static Dictionary<string, FinancialStatementNode> Get_FinancialStatementPositions_From_TaxanomyDocs(
            XmlSchema taxanomySchemaXsd,
            XmlDocument taxanomyCalculationLinkbaseXml, 
            FinancialStatementType financialStatement)
        {
            // Get the "roleURI" value for the Financial Statement of the specific type
            // from the "XBRL TAXONOMY EXTENSION SCHEMA DOCUMENT" xsd file:
            string financialStatementUri = 
                Get_FinancialStatementUri_From_TaxanomySchema(taxanomySchemaXsd, financialStatement);

            // Parse Xml and find all cash flow statement-related financial positions:
            Dictionary<string, FinancialStatementNode> financialStatementPositions =
                Get_FinancialStatementNodes_From_TaxanomyCalculationDoc(taxanomyCalculationLinkbaseXml, financialStatementUri);

            return financialStatementPositions;
        }

        // Method for working with "XBRL TAXONOMY EXTENSION SCHEMA DOCUMENT"
        // AAPL:
        // https://www.sec.gov/Archives/edgar/data/320193/0000320193-21-000105-index.htm
        // https://www.sec.gov/Archives/edgar/data/320193/000032019321000105/aapl-20210925.xsd
        // GS:
        // https://www.sec.gov/Archives/edgar/data/886982/0001193125-22-052682-index.htm
        public static string Get_FinancialStatementUri_From_TaxanomySchema(
            XmlSchema taxanomySchemaXsd,
            FinancialStatementType financialStatement)
        {
            // 1) Looking for <xs:annotation>:
            XmlSchemaAnnotation xsa = null;
            foreach (XmlSchemaObject xsdItem in taxanomySchemaXsd.Items)
            {
                if (xsdItem is XmlSchemaAnnotation)
                {
                    // found <xs:annotation>
                    xsa = xsdItem as XmlSchemaAnnotation;
                }
            }

            if (xsa == null)
            {
                return null;
            }

            // 2) Looking for <xs:appinfo>:
            XmlSchemaAppInfo xsai = null;
            foreach (XmlSchemaObject xsaItem in xsa.Items)
            {
                if (xsaItem is XmlSchemaAppInfo)
                {
                    // found <xs:appinfo>
                    xsai = xsaItem as XmlSchemaAppInfo;
                }
            }

            if (xsai == null)
            {
                return null;
            }

            // 3) Looking for id="ConsolidatedStatementsofCashFlows" attribute:
            foreach (XmlNode xsaiNode in xsai.Markup)
            {
                if (xsaiNode.Attributes.GetNamedItem("id") == null)
                {
                    continue;
                }

                foreach (string statementTitle in _statementTitles[financialStatement.ToString()])
                {
                    if (xsaiNode.Attributes.GetNamedItem("id").Value.ToUpper().Contains(statementTitle.ToUpper()))
                    {
                        return xsaiNode.Attributes.GetNamedItem("roleURI")?.Value;
                    }
                }
            }

            return null;
        }

        // "XBRL TAXONOMY EXTENSION CALCULATION LINKBASE DOCUMENT"
        public static Dictionary<string, FinancialStatementNode> Get_FinancialStatementNodes_From_TaxanomyCalculationDoc(
            XmlDocument taxanomyCalDocXml,
            string financialStatementUri)
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

            Dictionary<string, FinancialStatementNode> financialStatementPositions = new Dictionary<string, FinancialStatementNode>();
            // Find all cash flow statement nodes:
            foreach (XmlElement cashFlowNode in cashFlowStatementRootXmlNode.ChildNodes)
            {
                if (!cashFlowNode.Name.Contains("loc"))
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
                if (!cashFlowNode.Name.Contains("calculationArc"))
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
