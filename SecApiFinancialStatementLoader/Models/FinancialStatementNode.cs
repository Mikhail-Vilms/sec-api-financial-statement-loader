using System.Collections.Generic;

namespace SecApiFinancialStatementLoader.Models
{
    public class FinancialStatementNode
    {
        public string Name { get; set; }
        public HashSet<string> Children { get; set; }
    }
}
