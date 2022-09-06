using SecApiFinancialStatementLoader.Models;
using System;
using System.Threading.Tasks;

namespace SecApiFinancialStatementLoader.IServices
{
    /// <summary>
    /// Contract for operatoins with "XBRL TAXONOMY EXTENSION SCHEMA DOCUMENT" entities
    /// </summary>
    public interface ITaxonomyExtSchemaService
    {
        /// <summary>
        /// Method that retrieves "XBRL TAXONOMY EXTENSION SCHEMA DOCUMENT" xsd file from the SEC API and performs search for the URI-element for the given financial statement
        /// </summary>
        public Task<string> GetFinancialStatementURI(
            FinancialStatementDetails finStatementDetails,
            ReportDetails reportDetails,
            Action<string> logger);
    }
}
