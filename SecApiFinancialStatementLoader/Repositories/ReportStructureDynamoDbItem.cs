using Amazon.DynamoDBv2.DataModel;
using SecApiFinancialStatementLoader.Models;
using System.Collections.Generic;

namespace SecApiFinancialStatementLoader.Repositories
{
    [DynamoDBTable("Sec-Api-Financial-Data")]
    public class ReportStructureDynamoDbItem
    {
        [DynamoDBHashKey("PartitionKey")]
        public string PartitionKey { get; set; }

        [DynamoDBRangeKey("SortKey")]
        public string SortKey { get; set; }

        public Dictionary<string, FinancialStatementNode> FinancialPositions { get; set; }
    }
}
