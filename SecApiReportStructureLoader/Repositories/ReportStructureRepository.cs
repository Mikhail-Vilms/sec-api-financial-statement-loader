using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using SecApiReportStructureLoader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SecApiReportStructureLoader.Repositories
{
    public class ReportStructureRepository
    {
        private string _tableName = "sec-api-company-concepts";

        public async Task SaveToDynamo(
            string cikNumber,
            Dictionary<string, FinancialStatementNode> financialStatementPositions)
        {
            using var ddbClient = new AmazonDynamoDBClient(RegionEndpoint.USEast1);
            var dynamoTable = Table.LoadTable(ddbClient, _tableName, true);

            ReportStructureDynamoDbItem newItem = new ReportStructureDynamoDbItem()
            {
                cik = cikNumber,
                tag = "CashFlowStatementStructure",
                FinancialPositions = financialStatementPositions.Values.ToList()
            };

            var documentJson = JsonSerializer.Serialize(newItem);
            var document = Document.FromJson(documentJson);

            await dynamoTable.PutItemAsync(document);
        }
    }
}
