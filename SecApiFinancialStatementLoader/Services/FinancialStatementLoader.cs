using Amazon.DynamoDBv2.DataModel;
using SecApiFinancialStatementLoader.IServices;
using SecApiFinancialStatementLoader.Models;
using SecApiFinancialStatementLoader.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace SecApiFinancialStatementLoader.Services
{
    public class FinancialStatementLoader: IFinancialStatementLoader
    {
        private readonly IReportDetailsService _reportDetailsService;
        private readonly ITaxonomyExtSchemaService _taxonomyExtSchemaService;
        private readonly ITaxonomyExtLinkbaseService _taxonomyExtLinkbaseService;
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly ISnsService _snsService;

        public FinancialStatementLoader(
            IReportDetailsService reportDetailsService,
            ITaxonomyExtSchemaService taxonomyExtSchemaService,
            ITaxonomyExtLinkbaseService taxonomyExtLinkbaseService,
            IDynamoDBContext dynamoDbContext,
            ISnsService snsService)
        {
            _reportDetailsService = reportDetailsService;
            _taxonomyExtSchemaService = taxonomyExtSchemaService;
            _taxonomyExtLinkbaseService = taxonomyExtLinkbaseService;
            _dynamoDbContext = dynamoDbContext;
            _snsService = snsService;
        }

        public async Task Load(
            LambdaTriggerMessage triggerMessage,
            Action<string> logger)
        {
            if (triggerMessage == null || triggerMessage.CikNumber == null || triggerMessage.TickerSymbol == null)
            {
                logger($"Error occured: both cik number and ticker symbol values have to be provided in the trigger message");
            }
            string cikNumber = triggerMessage.CikNumber;
            string tickerSymbol = triggerMessage.TickerSymbol;


            // Find latest 10K report for the company:
            logger($"Trying to retrieve report details for {tickerSymbol}/{cikNumber}");
            ReportDetails reportDetails = await _reportDetailsService.Get_LatestReportDetails_By_Company(cikNumber, logger);
            logger($"Details of the latest report: {reportDetails}");

            XmlSchema taxanomySchemaXsd = 
                await _taxonomyExtSchemaService.RetrieveAndParse(
                    cikNumber,
                    tickerSymbol,
                    reportDetails.AccessionNumber,
                    reportDetails.ReportDate,
                    logger);

            XmlDocument taxanomyCalculationLinkbaseXml = 
                await _taxonomyExtLinkbaseService.RetrieveAndParse(
                    cikNumber,
                    tickerSymbol,
                    reportDetails.AccessionNumber,
                    reportDetails.ReportDate,
                    logger);

            foreach(FinancialStatementType financialStatementType in Enum.GetValues(typeof(FinancialStatementType)))
            {
                await LoadFinancialStatement(
                    cikNumber,
                    tickerSymbol,
                    financialStatementType,
                    taxanomySchemaXsd,
                    taxanomyCalculationLinkbaseXml,
                    logger);
            }
        }

        private async Task LoadFinancialStatement(
            string cikNumber,
            string tickerSymbol,
            FinancialStatementType financialStatementType,
            XmlSchema taxanomySchemaXsd,
            XmlDocument taxanomyCalculationLinkbaseXml,
            Action<string> logger)
        {
            string financialStatementURI = _taxonomyExtSchemaService
                .Get_FinancialStatementURI_From_TaxanomySchema(
                    taxanomySchemaXsd,
                    financialStatementType,
                    logger);

            Dictionary<string, FinancialStatementNode> financialStatementTree = _taxonomyExtLinkbaseService
                .Get_FinancialStatementTree_From_TaxanomyLinkbaseDoc(
                    taxanomyCalculationLinkbaseXml,
                    financialStatementURI,
                    logger);

            await _dynamoDbContext.SaveAsync(new ReportStructureDynamoDbItem()
            {
                PartitionKey = cikNumber,
                SortKey = $"StatementStructure_{financialStatementType}",
                FinancialPositions = financialStatementTree
            });

            logger($"Financial statement's structure for {tickerSymbol}/{cikNumber}/{financialStatementType} has been saved to Dynamo");

            await _snsService.PublishMsgsAsync(
                financialStatementTree
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
