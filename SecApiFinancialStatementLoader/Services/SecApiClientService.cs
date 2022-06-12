using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SecApiFinancialStatementLoader.Services
{
    public class SecApiClientService
    {
        private readonly string _submissionsUrl = "https://data.sec.gov/submissions/{0}.json";

        // XBRL TAXONOMY EXTENSION SCHEMA DOCUMENT
        // Example: https://www.sec.gov/Archives/edgar/data/320193/000032019321000105/aapl-20210925.xsd
        private readonly string _taxanomyXsdDocUrl = "https://www.sec.gov/Archives/edgar/data/{0}/{1}/{2}-{3}.xsd";

        // XBRL TAXONOMY EXTENSION CALCULATION LINKBASE DOCUMENT
        // Example: https://www.sec.gov/Archives/edgar/data/320193/000032019321000105/aapl-20210925_cal.xml
        private readonly string _taxanomyCalDocUrl = "https://www.sec.gov/Archives/edgar/data/{0}/{1}/{2}-{3}_cal.xml";

        private readonly HttpClient _httpClient;

        public SecApiClientService()
        {
            _httpClient = new HttpClient();

            _httpClient
                .DefaultRequestHeaders
                .UserAgent
                .Add(new ProductInfoHeaderValue("ScraperBot", "1.0"));
            _httpClient
                .DefaultRequestHeaders
                .UserAgent
                .Add(new ProductInfoHeaderValue("(+http://www.example.com/ScraperBot.html)"));
        }

        public async Task<string> RetrieveSubmissions(string cikNumber)
        {
            string targetUrl = string.Format(_submissionsUrl, cikNumber);
            var request = new HttpRequestMessage(HttpMethod.Get, targetUrl);
            var response = await _httpClient.SendAsync(request);
            string responseJson = await response.Content.ReadAsStringAsync();
            return responseJson;
        }

        public async Task<string> RetrieveTaxanomyXsdDoc(
            string cikNumber,
            string accessionNumber,
            string ticker,
            string filingDate,
            Action<string> logger)
        {
            string targetUrl = string
                .Format(
                    _taxanomyXsdDocUrl,
                    cikNumber.Remove(0, 3).TrimStart('0'),
                    accessionNumber.Replace("-", string.Empty),
                    ticker.ToLowerInvariant(),
                    filingDate.Replace("-", string.Empty));

            logger($"Trying to retrieve taxanomyXsdDoc from: {targetUrl}");

            var request = new HttpRequestMessage(HttpMethod.Get, targetUrl);

            var response = await _httpClient.SendAsync(request);
            string responseJson = await response.Content.ReadAsStringAsync();
            return responseJson;
        }

        public async Task<string> RetrieveTaxanomyCalDocXml(
            string cikNumber,
            string accessionNumber,
            string ticker,
            string filingDate,
            Action<string> logger)
        {
            string targetUrl = string
                .Format(
                    _taxanomyCalDocUrl,
                    cikNumber.Remove(0, 3).TrimStart('0'),
                    accessionNumber.Replace("-", string.Empty),
                    ticker.ToLowerInvariant(),
                    filingDate.Replace("-", string.Empty));

            logger($"Trying to retrieve taxanomyCalDoc from: {targetUrl}");

            var request = new HttpRequestMessage(HttpMethod.Get, targetUrl);

            var response = await _httpClient.SendAsync(request);
            string responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }
    }
}
