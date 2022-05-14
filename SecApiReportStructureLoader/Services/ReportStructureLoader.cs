using SecApiReportStructureLoader.Models;
using SecApiReportStructureLoader.Repositories;
using SecApiReportStructurePoller.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecApiReportStructureLoader.Services
{
    public class ReportStructureLoader
    {
        private readonly ReportDetailsService _reportDetailsService;
        private readonly ReportStructureService _reportStructureService;
        private readonly ReportStructureRepository _reportStructureRepository;

        public ReportStructureLoader()
        {
            _reportDetailsService = new ReportDetailsService();
            _reportStructureRepository = new ReportStructureRepository();
            _reportStructureService = new ReportStructureService();
        }

        public async Task Load(string cikNumber, string tickerSymbol, Action<string> logger)
        {
            logger($"Trying to retrieve report details for {tickerSymbol}/{cikNumber}");

            // Find latest 10k for the company:
            ReportDetails reportDetails = await _reportDetailsService.GetLatest10kDetails(cikNumber);
            logger($"Details of the latest report: {reportDetails.ToString()}");

            // Retrieve and parse titles of the financial statements positions from the latest report:
            Dictionary<string, FinancialStatementNode> financialStatementPositions = 
                await _reportStructureService.GetReportStructure(cikNumber, tickerSymbol, reportDetails);
            logger($"Number of the financial potitions for {tickerSymbol}/{cikNumber}: {financialStatementPositions.Count}");

            // Save finalized structure of the cash flow statement to dynamo:
            await _reportStructureRepository.SaveToDynamo(cikNumber, financialStatementPositions);
            logger($"Report structure for {tickerSymbol}/{cikNumber} has been saved to dynamo");
        }
    }
}
