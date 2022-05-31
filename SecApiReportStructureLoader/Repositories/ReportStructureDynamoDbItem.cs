using Amazon.DynamoDBv2.DataModel;
using SecApiReportStructureLoader.Models;
using System.Collections.Generic;

namespace SecApiReportStructureLoader.Repositories
{
    [DynamoDBTable("Sec-Api-Data")]
    public class ReportStructureDynamoDbItem
    {
        [DynamoDBHashKey("PartitionKey")]
        public string PartitionKey { get; set; }

        [DynamoDBRangeKey("SortKey")]
        public string SortKey { get; set; }

        public List<FinancialStatementNode> FinancialPositions { get; set; }
    }
}
