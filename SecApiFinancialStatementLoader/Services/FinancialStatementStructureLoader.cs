using SecApiFinancialStatementLoader.Helpers;
using SecApiFinancialStatementLoader.Models;
using SecApiFinancialStatementLoader.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace SecApiFinancialStatementLoader.Services
{
    public class FinancialStatementStructureLoader
    {   
        private readonly ReportDetailsService _reportDetailsService;
        private readonly ReportStructureRepository _reportStructureRepository;
        private readonly SecApiClientService _secApiClientService;
        private readonly SnsService _snsService;

        public FinancialStatementStructureLoader()
        {
            _reportDetailsService = new ReportDetailsService();
            _reportStructureRepository = new ReportStructureRepository();
            _secApiClientService = new SecApiClientService();
            _snsService = new SnsService();
        }

        public async Task Load(
            string cikNumber,
            string tickerSymbol,
            Action<string> logger)
        {
            logger($"Trying to retrieve report details for {tickerSymbol}/{cikNumber}");

            // Find latest 10k for the company:
            ReportDetails reportDetails = await _reportDetailsService.GetLatest10kDetails(cikNumber);
            logger($"Details of the latest report: {reportDetails}");

            // Get "XBRL TAXONOMY EXTENSION SCHEMA DOCUMENT" for the latest 10k:
            string taxanomySchemaStr = await _secApiClientService
                .RetrieveTaxanomyXsdDoc(
                    cikNumber,
                    reportDetails.AccessionNumber,
                    tickerSymbol,
                    reportDetails.ReportDate);
            // Parse into XmlSchema:
            XmlSchema taxanomySchemaXsd = XmlSchema.Read(new StringReader(taxanomySchemaStr), null);

            // Get "XBRL TAXONOMY EXTENSION CALCULATION LINKBASE DOCUMENT" for the current report:
            string taxanomyCalculationLinkbaseStr = await _secApiClientService
                .RetrieveTaxanomyCalDocXml(
                    cikNumber,
                    reportDetails.AccessionNumber,
                    tickerSymbol,
                    reportDetails.ReportDate);
            // Parse into xml doc:
            XmlDocument taxanomyCalculationLinkbaseXml = new XmlDocument();
            taxanomyCalculationLinkbaseXml.LoadXml(taxanomyCalculationLinkbaseStr);

            foreach (FinancialStatementType financialStatementType in Enum.GetValues(typeof(FinancialStatementType)))
            {
                // Retrieve and parse titles of the financial statement positions from the latest report:
                Dictionary<string, FinancialStatementNode> financialStatementPositions = XblrTaxanomyDocsHelper
                    .Get_FinancialStatementPositions_From_TaxanomyDocs(
                        taxanomySchemaXsd,
                        taxanomyCalculationLinkbaseXml,
                        financialStatementType);

                logger($"Number of the financial potitions for {tickerSymbol}/{cikNumber}/{financialStatementType}: {financialStatementPositions.Count}");

                // Save finalized structure of the cash flow statement to dynamo:
                await _reportStructureRepository
                    .SaveToDynamo(
                        cikNumber,
                        financialStatementType,
                        financialStatementPositions);

                logger($"Financial statement's structure for {tickerSymbol}/{cikNumber}/{financialStatementType} has been saved to Dynamo");

                await _snsService.PublishFinancialPositionsToLoadAsync(
                    financialStatementPositions
                        .Values
                        .Select(finStatementNode => JsonSerializer
                            .Serialize(new
                                {
                                    CikNumber = cikNumber,
                                    TickerSymbol = tickerSymbol,
                                    FinancialStatement = financialStatementType.ToString(),
                                    FinancialPosition = finStatementNode.Name
                                }))
                        .ToList());
            }
        }
    }
}
