using SecApiFinancialStatementLoader.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

namespace SecApiFinancialStatementLoader.IServices
{
    public interface ITaxonomyExtLinkbaseService
    {
        public Task<XmlDocument> RetrieveAndParse(
            string cikNumber,
            string tickerSymbol,
            string accessionNumber,
            string reportDate,
            Action<string> logger);

        public Dictionary<string, FinancialStatementNode> Get_FinancialStatementTree_From_TaxanomyLinkbaseDoc(
            XmlDocument taxanomyCalDocXml,
            string financialStatementURI,
            Action<string> logger);
    }
}
