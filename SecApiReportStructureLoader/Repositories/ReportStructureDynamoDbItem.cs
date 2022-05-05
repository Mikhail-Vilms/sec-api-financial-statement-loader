using Amazon.DynamoDBv2.DataModel;
using SecApiReportStructureLoader.Models;
using System.Collections.Generic;

namespace SecApiReportStructureLoader.Repositories
{
    [DynamoDBTable("sec-api-company-concepts")]
    public class ReportStructureDynamoDbItem
    {
        [DynamoDBHashKey("cik")]
        public string cik { get; set; }

        [DynamoDBRangeKey("tag")]
        public string tag { get; set; }

        public List<FinancialStatementNode> FinancialPositions { get; set; }
    }
}
