using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using SecApiFinancialStatementLoader.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SecApiFinancialStatementLoader.Repositories
{
    public class ReportStructureRepository
    {
        private readonly string _tableName = "Sec-Api-Financial-Data";

        public async Task SaveToDynamo(
            string cikNumber,
            FinancialStatementType financialStatement,
            Dictionary<string, FinancialStatementNode> financialStatementPositions)
        {
            using var ddbClient = new AmazonDynamoDBClient(RegionEndpoint.USWest2);
            var dynamoTable = Table.LoadTable(ddbClient, _tableName, true);

            ReportStructureDynamoDbItem newItem = new ReportStructureDynamoDbItem()
            {
                PartitionKey = cikNumber,
                SortKey = $"StatementStructure_{financialStatement}",
                FinancialPositions = financialStatementPositions.Values.ToList()
            };

            var documentJson = JsonSerializer.Serialize(newItem);
            var document = Document.FromJson(documentJson);

            await dynamoTable.PutItemAsync(document);
        }
    }
}
