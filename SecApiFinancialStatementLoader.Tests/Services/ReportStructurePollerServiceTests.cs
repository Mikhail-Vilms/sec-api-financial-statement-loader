using SecApiFinancialStatementLoader.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SecApiFinancialStatementLoader.Tests.Services
{
    public class ReportStructurePollerServiceTests
    {
        [Fact]
        public async Task GetReportStructure_Success()
        {
            void Log(string logMsg)
            {
                Console.WriteLine(logMsg);
            }

            ReportStructureLoader _loader = new ReportStructureLoader();

            await _loader.Load("CIK0000886982", "GS", Log);

            Assert.Equal(true, true);
        }
    }
}
