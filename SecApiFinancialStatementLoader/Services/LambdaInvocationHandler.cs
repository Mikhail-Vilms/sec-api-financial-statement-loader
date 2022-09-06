using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using SecApiFinancialStatementLoader.IServices;
using SecApiFinancialStatementLoader.Models;
using System;
using System.Threading.Tasks;

namespace SecApiFinancialStatementLoader.Services
{
    public class LambdaInvocationHandler : ILambdaInvocationHandler
    {
        private readonly IDeserializer _deserializer;
        private readonly IFinancialStatementLoader _financialStatementLoader;

        public LambdaInvocationHandler(
            IDeserializer deserializer,
            IFinancialStatementLoader financialStatementLoader)
        {
            _deserializer = deserializer;
            _financialStatementLoader = financialStatementLoader;
        }

        public async Task FunctionHandler(SQSEvent sqsEvnt, ILambdaContext context)
        {
            foreach (var msg in sqsEvnt.Records)
            {
                await ProcessMessageAsync(msg, context);
            }
        }

        private async Task ProcessMessageAsync(SQSEvent.SQSMessage msg, ILambdaContext context)
        {
            void Log(string logMessage)
            {
                context.Logger.LogLine($"[RequestId: {context.AwsRequestId}]: {logMessage}");
            }

            Log($">>>>> Processing message {msg.Body}");

            LambdaTriggerMessage triggerMessage = null;
            try
            {
                triggerMessage = _deserializer.Get(msg);

                if (triggerMessage == null || triggerMessage.CikNumber == null || triggerMessage.TickerSymbol == null || triggerMessage.FinancialStatement == null)
                {
                    throw new ArgumentNullException("Cik number, ticker symbol and financial statement values have to be provided in the trigger message");
                }
            }
            catch (Exception ex)
            {
                Log($"Failed to deserialize lambda's trigger message: {ex}");
                return;
            }

            try
            {
                FinancialStatementDetails finStatementDetails = new FinancialStatementDetails()
                {
                    CikNumber = triggerMessage.CikNumber,
                    TickerSymbol = triggerMessage.TickerSymbol,
                    FinancialStatement = (FinancialStatementEnum) Enum.Parse(typeof(FinancialStatementEnum), triggerMessage.FinancialStatement)
                };

                await _financialStatementLoader.Load(finStatementDetails, Log);
            }
            catch (Exception ex)
            {
                Log($"Failed to process message: triggerMessage; msg: {ex}");
            }

            Log($"Finished processing. <<<<<");
        }
    }
}
