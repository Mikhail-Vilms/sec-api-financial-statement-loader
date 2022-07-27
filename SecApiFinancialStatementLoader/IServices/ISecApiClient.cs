using System;
using System.Threading.Tasks;

namespace SecApiFinancialStatementLoader.IServices
{
    /// <summary>
    /// Contract that describes direct interactions with the SEC API
    /// </summary>
    public interface ISecApiClient
    {
        /// <summary>
        /// Method that retrieves the list of copmany's submissions(including 10K/10Q reports) from the SEC API
        /// </summary>
        public Task<string> RetrieveSubmissions(string cikNumber, Action<string> logger);

        /// <summary>
        /// Method that retrieves "xblr taxonomy extension schema document" from the SEC API
        /// </summary>
        public Task<string> RetrieveTaxanomyXsdDoc(
            string cikNumber,
            string accessionNumber,
            string ticker,
            string filingDate,
            Action<string> logger);

        /// <summary>
        /// Method that retrieves "xblr taxonomy extension calculation linkbase document" from the SEC API
        /// </summary>
        public Task<string> RetrieveTaxanomyCalDocXml(
            string cikNumber,
            string accessionNumber,
            string ticker,
            string filingDate,
            Action<string> logger);
    }
}
