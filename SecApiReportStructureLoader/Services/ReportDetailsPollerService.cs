using SecApiReportStructureLoader.Services;
using SecApiReportStructurePoller.Models;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SecApiReportStructurePoller.Services
{
    public class ReportDetailsPollerService
    {
        private readonly SecApiClientService _secApiClientService;

        public ReportDetailsPollerService()
        {
            _secApiClientService = new SecApiClientService();
        }

        public async Task<ReportDetails> GetLatest10kReportDetails(string cikNumber)
        {
            // Send HTTP Request to SEC API
            string submissionsResponseJson = await _secApiClientService.RetrieveSubmissions(cikNumber);

            JsonElement recentFilings = JsonDocument
                .Parse(submissionsResponseJson)
                .RootElement
                .GetProperty("filings")
                .GetProperty("recent");

            int targetIndexOfFirst10kReport = recentFilings
                .GetProperty("form")
                .EnumerateArray()
                .ToList()
                .Select(el => el.GetString())
                .ToList()
                .IndexOf("10-K");

            string targetAccessionNumber = recentFilings
                .GetProperty("accessionNumber")
                .EnumerateArray()
                .ElementAt(targetIndexOfFirst10kReport)
                .GetString();

            string targetReportDate = recentFilings
                .GetProperty("reportDate")
                .EnumerateArray()
                .ElementAt(targetIndexOfFirst10kReport)
                .GetString();

            return new ReportDetails()
            {
                AccessionNumber = targetAccessionNumber,
                ReportDate = targetReportDate
            };
        }
    }
}