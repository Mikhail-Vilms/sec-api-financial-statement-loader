using SecApiFinancialStatementLoader.Models;
using System;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace SecApiFinancialStatementLoader.IServices
{
    /// <summary>
    /// Contract for operatoins with "XBRL TAXONOMY EXTENSION SCHEMA DOCUMENT" files
    /// </summary>
    public interface ITaxonomyExtSchemaService
    {
        /// <summary>
        /// Method that retrieves "XBRL TAXONOMY EXTENSION SCHEMA DOCUMENT" xsd file from the SEC API
        /// </summary>
        public Task<XmlSchema> RetrieveAndParse(
            string cikNumber,
            string tickerSymbol,
            string accessionNumber,
            string reportDate,
            Action<string> logger);

        /// <summary>
        /// Method that performs search for the URI-element for the given financial statement
        /// </summary>
        public string Get_FinancialStatementURI_From_TaxanomySchema(
            XmlSchema taxanomySchemaXsd,
            FinancialStatementType financialStatement,
            Action<string> logger);
    }
}
