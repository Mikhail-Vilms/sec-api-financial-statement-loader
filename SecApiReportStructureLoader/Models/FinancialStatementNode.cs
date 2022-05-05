using System.Collections.Generic;

namespace SecApiReportStructureLoader.Models
{
    public class FinancialStatementNode
    {
        public string FullLabel { get; set; }
        public string Name { get; set; }
        public List<string> Children { get; set; }
    }
}
