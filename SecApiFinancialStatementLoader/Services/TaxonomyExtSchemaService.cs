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
    public class TaxonomyExtSchemaService : ITaxonomyExtSchemaService
    {
        private readonly ISecApiClient _secApiClient;

        private readonly Dictionary<string, List<string>> _statementTitles = new Dictionary<string, List<string>>()
        {
            {
                FinancialStatementType.IncomeStatement.ToString(),
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
                FinancialStatementType.BalanceSheet.ToString(),
                new List<string>()
                {
                    "BalanceSheets",
                    "BalanceSheet", // used by IBM
                    "FinancialPosition" // used by CAT
                }
            },
            {
                FinancialStatementType.CashFlowStatement.ToString(),
                new List<string>()
                {
                    "StatementsOfCashFlows",
                    "StatementofCashFlow", // used by CAT, IBM
                    "CashFlowStatements" // used by MSFT
                }
            },
        };

        public TaxonomyExtSchemaService(ISecApiClient secApiClient)
        {
            _secApiClient = secApiClient;
        }

        public async Task<XmlSchema> RetrieveAndParse(
            string cikNumber,
            string tickerSymbol,
            string accessionNumber,
            string reportDate,
            Action<string> logger)
        {
            // Get "XBRL TAXONOMY EXTENSION SCHEMA DOCUMENT" for the latest 10k:
            string taxanomySchemaResponseStr = await _secApiClient.RetrieveTaxanomyXsdDoc(
                cikNumber,
                accessionNumber,
                tickerSymbol,
                reportDate,
                logger);

            // Parse into XmlSchema:
            XmlSchema taxanomySchemaXsd = XmlSchema
                .Read(new StringReader(taxanomySchemaResponseStr), null);

            return taxanomySchemaXsd;
        }

        public string Get_FinancialStatementURI_From_TaxanomySchema(
            XmlSchema taxanomySchemaXsd,
            FinancialStatementType financialStatement,
            Action<string> logger)
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
