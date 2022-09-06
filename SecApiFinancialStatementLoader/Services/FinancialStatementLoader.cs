using Amazon.DynamoDBv2.DataModel;
using SecApiFinancialStatementLoader.IServices;
using SecApiFinancialStatementLoader.Models;
using SecApiFinancialStatementLoader.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

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
            FinancialStatementDetails finStatementDetails,
            Action<string> logger)
        {
            // Find latest 10K report for the company:
            logger($"Trying to retrieve report details for {finStatementDetails}");
            ReportDetails reportDetails = await _reportDetailsService
                .Get_LatestReportDetails_By_Company(finStatementDetails.CikNumber, logger);
            logger($"Details of the latest report: {reportDetails}");


            // Get "XBRL TAXONOMY EXTENSION SCHEMA DOCUMENT" xsd file from the SEC API and perform search for the "Financial Statement URI"-element:
            string financialStatementURI = await _taxonomyExtSchemaService
                .GetFinancialStatementURI(finStatementDetails, reportDetails, logger);
            logger($"Financial statement's URI for {finStatementDetails} has been successfully fetched: {financialStatementURI}");


            // Get the "XBRL TAXONOMY EXTENSION CALCULATION LINKBASE DOCUMENT" for the given financial statement;
            // Traverse all the nodes in this document and store them in a dictionary that reflects the financail statement's structure:
            Dictionary<string, FinancialStatementNode> financialStatementStructure =await _taxonomyExtLinkbaseService
                .GetFinancialStatementStructure(finStatementDetails, reportDetails, financialStatementURI, logger);
            logger($"Financial statement structure for {finStatementDetails} have been retrieved; Total number of financial positions: {financialStatementStructure.Keys.Count}");


            // Save dictionary to the dynamo table:
            var newDynamoItem = new FinStatementStructureDynamoItem()
            {
                PartitionKey = finStatementDetails.CikNumber,
                SortKey = $"StatementStructure_{finStatementDetails.FinancialStatement}",
                FinancialPositions = financialStatementStructure
            };
            await _dynamoDbContext.SaveAsync(newDynamoItem);
            logger($"Financial statement's structure for {finStatementDetails} has been saved to Dynamo; {newDynamoItem}");


            // Publish messages to the SNS topic:
            await _snsService.PublishMsgsAsync(
                financialStatementStructure
                    .Values
                    .Select(finStatementNode => JsonSerializer
                        .Serialize(new
                        {
                            CikNumber = finStatementDetails.CikNumber,
                            TickerSymbol = finStatementDetails.TickerSymbol,
                            FinancialStatement = finStatementDetails.FinancialStatement.ToString(),
                            FinancialPosition = finStatementNode.Name
                        }))
                    .ToList());
        }
    }
}
