using SecApiFinancialStatementLoader.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecApiFinancialStatementLoader.IServices
{
    /// <summary>
    /// Contract for operatoins with "XBRL TAXONOMY EXTENSION CALCULATION LINKBASE DOCUMENT" entities
    /// </summary>
    public interface ITaxonomyExtLinkbaseService
    {
        /// <summary>
        /// Method that retrieves an "XBRL TAXONOMY EXTENSION CALCULATION LINKBASE DOCUMENT" xml file from the SEC API and constructs a dictionary that reflects the structure of a given financial statement
        /// </summary>
        public Task<Dictionary<string, FinancialStatementNode>> GetFinancialStatementStructure(
            FinancialStatementDetails finStatementDetails,
            ReportDetails reportDetails,
            string financialStatementURI,
            Action<string> logger);
    }
}
