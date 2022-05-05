using SecApiReportStructureLoader.Helpers;
using SecApiReportStructureLoader.Models;
using SecApiReportStructureLoader.Repositories;
using SecApiReportStructurePoller.Models;
using SecApiReportStructurePoller.Services;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace SecApiReportStructureLoader.Services
{
    public class ReportStructurePollerService
    {
        private readonly SecApiClientService _secApiClientService;
        private readonly ReportDetailsPollerService _reportDetailsPollerService;
        private readonly ReportStructureRepository _reportStructureRepository;

        public ReportStructurePollerService()
        {
            _secApiClientService = new SecApiClientService();
            _reportDetailsPollerService = new ReportDetailsPollerService();
            _reportStructureRepository = new ReportStructureRepository();
        }

        public async Task GetReportStructure(string cikNumber, string ticker)
        {
            // Find latest 10k for the company:
            ReportDetails reportDetails = await _reportDetailsPollerService.GetLatest10kReportDetails(cikNumber);

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

            // Save results to dynamo:
            await _reportStructureRepository.SaveToDynamo(cikNumber, financialStatementPositions);
        }
    }
}
