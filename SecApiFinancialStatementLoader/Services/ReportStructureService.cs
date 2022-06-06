using SecApiFinancialStatementLoader.Helpers;
using SecApiFinancialStatementLoader.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace SecApiFinancialStatementLoader.Services
{
    public class ReportStructureService
    {
        private readonly SecApiClientService _secApiClientService;

        public ReportStructureService()
        {
            _secApiClientService = new SecApiClientService();
        }

        public async Task<Dictionary<string, FinancialStatementNode>> GetReportStructure(string cikNumber, string ticker, ReportDetails reportDetails)
        {
            // Get Taxanomy Xsd Document for the latest 10k:
            string taxanomyXsdDoc = await _secApiClientService.RetrieveTaxanomyXsdDoc(
                cikNumber,
                reportDetails.AccessionNumber,
                ticker,
                reportDetails.ReportDate);

            // Parse into XmlSchema
            XmlSchema taxanomyXsdSchema = XmlSchema.Read(new StringReader(taxanomyXsdDoc), null);

            // Find "GetCashFlowsUri" in "Taxanomy" Xsd file:
            string cashFlowUri = TaxanomyXsdDocHelper.GetCashFlowsUriFromXsdSchema(taxanomyXsdSchema);

            // Get XBRL TAXONOMY EXTENSION CALCULATION LINKBASE DOCUMENT for current report:
            string taxanomyCalDoc = await _secApiClientService.RetrieveTaxanomyCalDocXml(
                cikNumber,
                reportDetails.AccessionNumber,
                ticker,
                reportDetails.ReportDate);
            XmlDocument taxanomyCalDocXml = new XmlDocument();
            taxanomyCalDocXml.LoadXml(taxanomyCalDoc);

            // Parse Xml and find all cash flow statement-related financial positions:
            Dictionary<string, FinancialStatementNode> financialStatementPositions = XbrlTaxanomyCalculationDocHelper.GetCashFlowsNodes(taxanomyCalDocXml, cashFlowUri);

            return financialStatementPositions;
        }
    }
}
