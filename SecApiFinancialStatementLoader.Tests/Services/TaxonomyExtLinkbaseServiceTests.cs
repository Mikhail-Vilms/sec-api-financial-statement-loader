using Moq;
using SecApiFinancialStatementLoader.IServices;
using SecApiFinancialStatementLoader.Models;
using SecApiFinancialStatementLoader.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace SecApiFinancialStatementLoader.Tests.Services
{
    public class TaxonomyExtLinkbaseServiceTests
    {
        [Fact]
        public async Task GetFinancialStatementStructure_All3Statements_ORCL_Sample_Success()
        {
            // Arrange
            var mockSecApiClient = GetMockSecApiClient();

            TaxonomyExtLinkbaseService taxonomyExtLinkbaseService = new TaxonomyExtLinkbaseService(mockSecApiClient.Object);

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
            Dictionary<string, FinancialStatementNode> resultFinancialStatementStructure = 
                await taxonomyExtLinkbaseService.GetFinancialStatementStructure(
                    finStatementDetails,
                    reportDetails,
                    "http://www.oracle.com/20220531/taxonomy/role/StatementCONSOLIDATEDBALANCESHEETS",
                    Log);

            // Act - test income statement
            resultFinancialStatementStructure =
                await taxonomyExtLinkbaseService.GetFinancialStatementStructure(
                    finStatementDetails,
                    reportDetails,
                    "http://www.oracle.com/20220531/taxonomy/role/StatementCONSOLIDATEDSTATEMENTSOFOPERATIONS",
                    Log);

            // Assert
            Assert.Contains("OperatingIncomeLoss", resultFinancialStatementStructure.Keys);
            Assert.Contains("NetIncomeLoss", resultFinancialStatementStructure.Keys);

            // Act - test cashflow statement
            resultFinancialStatementStructure =
                await taxonomyExtLinkbaseService.GetFinancialStatementStructure(
                    finStatementDetails,
                    reportDetails,
                    "http://www.oracle.com/20220531/taxonomy/role/StatementCONSOLIDATEDSTATEMENTSOFCASHFLOWS",
                    Log);

            // Assert
            Assert.Contains("NetCashProvidedByUsedInOperatingActivities", resultFinancialStatementStructure.Keys);
            Assert.Contains("NetCashProvidedByUsedInInvestingActivities", resultFinancialStatementStructure.Keys);
            Assert.Contains("NetCashProvidedByUsedInFinancingActivities", resultFinancialStatementStructure.Keys);
        }

        [Fact]
        public async Task GetFinancialStatementStructure_All3Statemnets_IBM_Sample_Success()
        {
            // Arrange
            var mockSecApiClient = GetMockSecApiClient();

            TaxonomyExtLinkbaseService taxonomyExtLinkbaseService = new TaxonomyExtLinkbaseService(mockSecApiClient.Object);

            FinancialStatementDetails finStatementDetails = new FinancialStatementDetails()
            {
                CikNumber = "CIK0000051143",
                TickerSymbol = "IBM",
                FinancialStatement = FinancialStatementEnum.BalanceSheet
            };
            ReportDetails reportDetails = new ReportDetails()
            {
                AccessionNumber = "0001564590-22-023675",
                ReportDate = "2022-05-31"
            };
            void Log(string logMessage) => Console.WriteLine($"{logMessage}");

            // Act
            Dictionary<string, FinancialStatementNode> resultFinancialStatementStructure =
                await taxonomyExtLinkbaseService.GetFinancialStatementStructure(
                    finStatementDetails,
                    reportDetails,
                    "http://www.ibm.com/role/StatementConsolidatedBalanceSheet",
                    Log);

            // Act - test income statement
            resultFinancialStatementStructure =
                await taxonomyExtLinkbaseService.GetFinancialStatementStructure(
                    finStatementDetails,
                    reportDetails,
                    "http://www.ibm.com/role/StatementConsolidatedIncomeStatement",
                    Log);

            // Assert
            Assert.Contains("NetIncomeLoss", resultFinancialStatementStructure.Keys);

            // Act - test cashflow statement
            resultFinancialStatementStructure =
                await taxonomyExtLinkbaseService.GetFinancialStatementStructure(
                    finStatementDetails,
                    reportDetails,
                    "http://www.ibm.com/role/StatementConsolidatedStatementOfCashFlows",
                    Log);

            // Assert
            Assert.Contains("NetCashProvidedByUsedInOperatingActivities", resultFinancialStatementStructure.Keys);
            Assert.Contains("NetCashProvidedByUsedInInvestingActivities", resultFinancialStatementStructure.Keys);
            Assert.Contains("NetCashProvidedByUsedInFinancingActivities", resultFinancialStatementStructure.Keys);
        }

        #region Mocking "SecApiClient" service

        private Mock<ISecApiClient> _mockSecApiClient = null;
        
        private Mock<ISecApiClient> GetMockSecApiClient()
        {
            if (_mockSecApiClient != null)
            {
                return _mockSecApiClient;
            }

            string currentDirectory = Directory.GetCurrentDirectory();
            var mockSecApiClient = new Mock<ISecApiClient>();

            // Oracle
            string pathToSampleXmlFile = Path.GetFullPath(currentDirectory + "\\" + ".\\SecApiResponseSamples\\orcl-20220531_cal.xml");
            string secApiMockResponse = File.ReadAllText(pathToSampleXmlFile);

            mockSecApiClient
                .Setup(expr => expr.RetrieveTaxanomyCalDocXml(
                    It.Is<string>(cikNumber => cikNumber.Equals("CIK0001341439")),
                    It.IsAny<string>(),
                    It.Is<string>(tickerSymbol => tickerSymbol.Equals("ORCL")),
                    It.IsAny<string>(),
                    It.IsAny<Action<string>>()))
                .ReturnsAsync(secApiMockResponse);

            // IBM
            pathToSampleXmlFile = Path.GetFullPath(currentDirectory + "\\" + ".\\SecApiResponseSamples\\ibm-20211231_cal.xml");
            secApiMockResponse = File.ReadAllText(pathToSampleXmlFile);

            mockSecApiClient
                .Setup(expr => expr.RetrieveTaxanomyCalDocXml(
                    It.Is<string>(cikNumber => cikNumber.Equals("CIK0000051143")),
                    It.IsAny<string>(),
                    It.Is<string>(tickerSymbol => tickerSymbol.Equals("IBM")),
                    It.IsAny<string>(),
                    It.IsAny<Action<string>>()))
                .ReturnsAsync(secApiMockResponse);

            // Johnson & Johnson
            pathToSampleXmlFile = Path.GetFullPath(currentDirectory + "\\" + ".\\SecApiResponseSamples\\jnj-20220102_cal.xml");
            secApiMockResponse = File.ReadAllText(pathToSampleXmlFile);

            mockSecApiClient
                .Setup(expr => expr.RetrieveTaxanomyCalDocXml(
                    It.Is<string>(cikNumber => cikNumber.Equals("CIK0000200406")),
                    It.IsAny<string>(),
                    It.Is<string>(tickerSymbol => tickerSymbol.Equals("JNJ")),
                    It.IsAny<string>(),
                    It.IsAny<Action<string>>()))
                .ReturnsAsync(secApiMockResponse);

            _mockSecApiClient = mockSecApiClient;

            return mockSecApiClient;
        }

        #endregion Mocking "SecApiClient" service
    }
}
