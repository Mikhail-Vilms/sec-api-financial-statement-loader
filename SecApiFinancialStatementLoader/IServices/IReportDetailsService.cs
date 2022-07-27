using SecApiFinancialStatementLoader.Models;
using System;
using System.Threading.Tasks;

namespace SecApiFinancialStatementLoader.IServices
{
    public interface IReportDetailsService
    {
        public Task<ReportDetails> Get_LatestReportDetails_By_Company(string cikNumber, Action<string> logger);
    }
}
