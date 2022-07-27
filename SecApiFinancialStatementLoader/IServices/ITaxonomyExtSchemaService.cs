using SecApiFinancialStatementLoader.Models;
using System;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace SecApiFinancialStatementLoader.IServices
{
    public interface ITaxonomyExtSchemaService
    {
        public Task<XmlSchema> RetrieveAndParse(
            string cikNumber,
            string tickerSymbol,
            string accessionNumber,
            string reportDate,
            Action<string> logger);

        public string Get_FinancialStatementURI_From_TaxanomySchema(
            XmlSchema taxanomySchemaXsd,
            FinancialStatementType financialStatement,
            Action<string> logger);
    }
}
