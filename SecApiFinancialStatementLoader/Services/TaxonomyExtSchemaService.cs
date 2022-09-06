using SecApiFinancialStatementLoader.IServices;
using SecApiFinancialStatementLoader.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace SecApiFinancialStatementLoader.Services
{
    /// <summary>
    /// Class for operations with "XBRL TAXONOMY EXTENSION SCHEMA DOCUMENT" entities
    /// </summary>
    public class TaxonomyExtSchemaService : ITaxonomyExtSchemaService
    {
        // Hardcoding statement titles that we need to fetch
        private readonly Dictionary<string, List<string>> _statementTitles = new Dictionary<string, List<string>>()
        {
            {
                FinancialStatementEnum.IncomeStatement.ToString(),
                new List<string>()
                {
                    "StatementsOfEarnings", // used by GS
                    "StatementsOfOperations", // used by AAPL
                    "StatementsofIncome", // used by INTC
                    "ResultsofOperations", // used by CAT
                    "IncomeStatement", // used by IBM
                    "IncomeStatements" // used by MSFT
                }
            },
            {
                FinancialStatementEnum.BalanceSheet.ToString(),
                new List<string>()
                {
                    "BalanceSheets",
                    "BalanceSheet", // used by IBM
                    "FinancialPosition" // used by CAT
                }
            },
            {
                FinancialStatementEnum.CashFlowStatement.ToString(),
                new List<string>()
                {
                    "StatementsOfCashFlows",
                    "StatementofCashFlow", // used by CAT, IBM
                    "CashFlowStatements" // used by MSFT
                }
            },
        };

        private readonly ISecApiClient _secApiClient;

        public TaxonomyExtSchemaService(ISecApiClient secApiClient)
        {
            _secApiClient = secApiClient;
        }

        /// <inheritdoc />
        public async Task<string> GetFinancialStatementURI(
            FinancialStatementDetails finStatementDetails,
            ReportDetails reportDetails,
            Action<string> logger)
        {
            // Get "XBRL TAXONOMY EXTENSION SCHEMA DOCUMENT" for the latest 10k:
            string taxanomySchemaResponseStr = await _secApiClient.RetrieveTaxanomyXsdDoc(
                finStatementDetails.CikNumber,
                reportDetails.AccessionNumber,
                finStatementDetails.TickerSymbol,
                reportDetails.ReportDate,
                logger);

            // Parse response into XmlSchema:
            XmlSchema taxanomySchemaXsd = XmlSchema
                .Read(new StringReader(taxanomySchemaResponseStr), null);

            // Fetch the URI of the given financial statement from the "Taxonomy Extension Schema Document":
            return LookupForFinancialStatementURI(taxanomySchemaXsd, finStatementDetails.FinancialStatement);
        }

        private string LookupForFinancialStatementURI(XmlSchema taxanomySchemaXsd, FinancialStatementEnum financialStatement)
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
    }
}
