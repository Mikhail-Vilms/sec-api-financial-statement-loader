using Moq;
using SecApiFinancialStatementLoader.IServices;
using SecApiFinancialStatementLoader.Models;
using SecApiFinancialStatementLoader.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace SecApiFinancialStatementLoader.Tests.Services
{
    public class TaxonomyExtSchemaServiceTests
    {
        [Fact]
        public async Task GetFinancialStatementURI_AllThreeStatements_ORCL_Sample_Success()
        {
            // Arrange
            string currentDirectory = Directory.GetCurrentDirectory();
            string pathToSampleXsdFile = Path.GetFullPath(currentDirectory + "\\" + ".\\SecApiResponseSamples\\orcl-20220531.xsd");
            string secApiMockResponse = File.ReadAllText(pathToSampleXsdFile);

            var mockSecApiClient = new Mock<ISecApiClient>();
            mockSecApiClient
                .Setup(expr => expr.RetrieveTaxanomyXsdDoc(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Action<string>>()))
                .ReturnsAsync(secApiMockResponse);

            TaxonomyExtSchemaService taxonomyExtSchemaService = new TaxonomyExtSchemaService(mockSecApiClient.Object);

            FinancialStatementDetails finStatementDetails = new FinancialStatementDetails()
            {
                CikNumber = "CIK0001341439",
                TickerSymbol = "ORCL",
                FinancialStatement = FinancialStatementEnum.BalanceSheet
            };

            ReportDetails reportDetails = new ReportDetails()
            {
                AccessionNumber = "0001564590-22-023675",
                ReportDate = "2022-05-31"
            };

            void Log(string logMessage) => Console.WriteLine($"{logMessage}");

            // Act
            string resultFinStatementUri = await taxonomyExtSchemaService
                .GetFinancialStatementURI(finStatementDetails, reportDetails, Log);

            // Assert
            Assert.Equal(
                "http://www.oracle.com/20220531/taxonomy/role/StatementCONSOLIDATEDBALANCESHEETS",
                resultFinStatementUri);
        }
    }
}
