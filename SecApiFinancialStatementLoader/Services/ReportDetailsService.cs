using SecApiFinancialStatementLoader.IServices;
using SecApiFinancialStatementLoader.Models;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SecApiFinancialStatementLoader.Services
{
    public class ReportDetailsService : IReportDetailsService
    {
        private readonly ISecApiClient _secApiClient;

        public ReportDetailsService(ISecApiClient secApiClient)
        {
            _secApiClient = secApiClient;
        }

        public async Task<ReportDetails> Get_LatestReportDetails_By_Company(string cikNumber, Action<string> logger)
        {
            // Send HTTP Request to SEC API
            string submissionsResponseJson = await _secApiClient.RetrieveSubmissions(cikNumber, logger);

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