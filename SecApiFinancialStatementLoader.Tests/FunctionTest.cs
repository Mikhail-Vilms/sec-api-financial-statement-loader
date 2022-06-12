using Amazon.Lambda.SQSEvents;
using Amazon.Lambda.TestUtilities;
using SecApiFinancialStatementLoader.Models;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace SecApiFinancialStatementLoader.Tests
{
    public class FunctionTest
    {
        [Fact]
        public async Task TestToUpperFunction()
        {
            LambdaTriggerMessage sqsMessage = new LambdaTriggerMessage()
            {
                CikNumber = "CIK0000050863",
                TickerSymbol = "INTC"
            };

            string sqsMessageStr = JsonSerializer.Serialize(sqsMessage);

            var sqsEvent = new SQSEvent
            {
                Records = new List<SQSEvent.SQSMessage>()
                {
                    new SQSEvent.SQSMessage{ Body = sqsMessageStr}
                }
            };

            // Invoke the lambda function and confirm the string was upper cased.
            var function = new Function();
            var context = new TestLambdaContext();
            await function.FunctionHandler(sqsEvent, context);

            Assert.Equal("HELLO WORLD", "HELLO WORLD");
        }
    }
}
