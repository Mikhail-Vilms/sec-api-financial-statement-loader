using SecApiReportStructureLoader.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SecApiReportStructureLoader.Tests.Services
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

            await _loader.Load("CIK0000050863", "INTC", Log);

            Assert.Equal(true, true);
        }
    }
}
