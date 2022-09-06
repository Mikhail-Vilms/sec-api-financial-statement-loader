namespace SecApiFinancialStatementLoader.Models
{
    /// <summary>
    /// POCO class that reflects the structure of the message received from the SNS/SQS
    /// </summary>
    public class LambdaTriggerMessage
    {
        public string CikNumber { get; set; }
        public string TickerSymbol { get; set; }
        public string FinancialStatement { get; set; }
    }
}
