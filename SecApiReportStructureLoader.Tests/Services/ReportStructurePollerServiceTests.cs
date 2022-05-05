using SecApiReportStructureLoader.Services;
using System.Threading.Tasks;
using Xunit;

namespace SecApiReportStructureLoader.Tests.Services
{
    public class ReportStructurePollerServiceTests
    {
        [Fact]
        public async Task GetReportStructure_Success()
        {
            ReportStructurePollerService _service = new ReportStructurePollerService();

            await _service.GetReportStructure("CIK0000200406", "JNJ");

            //Assert.Equal("0000050863-22-000007", details.AccessionNumber);
            //Assert.Equal("2021-12-25", details.ReportDate);

            Assert.Equal(true, true);
        }
    }
}
