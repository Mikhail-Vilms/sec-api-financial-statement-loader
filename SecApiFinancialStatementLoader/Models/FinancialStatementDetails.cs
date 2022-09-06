namespace SecApiFinancialStatementLoader.Models
{
    /// <summary>
    /// POCO class that stores information about the financial statement that lambda loads
    /// </summary>
    public class FinancialStatementDetails
    {
        /// <summary>
        /// A Central Index Key or CIK number is a number given to an individual or company by the United States Securities and Exchange Commission.
        /// The number is used to identify the filings of a company, person, or entity in several online databases.
        /// Including Compustat in WRDS and in EDGAR.
        /// </summary>
        public string CikNumber { get; set; }

        /// <summary>
        /// A ticker symbol, also called a stock symbol, is a unique code that represents a company listed on a stock exchange.
        /// It's typically an abbreviation of the company's name or may have some other reference to the company.
        /// </summary>
        public string TickerSymbol { get; set; }

        /// <summary>
        /// Financial statement is reference to <see cref="FinancialStatementEnum"/> FinancialStatementType enum class.
        /// </summary>
        public FinancialStatementEnum FinancialStatement { get; set; }

        public override string ToString()
        {
            return $"[{TickerSymbol}/{CikNumber}/{FinancialStatement}]";
        }
    }
}
