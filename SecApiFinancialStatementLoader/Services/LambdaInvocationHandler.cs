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
            }
            catch (Exception ex)
            {
                Log($"Failed to deserialize lambda's trigger message: {ex}");
                return;
            }

            try
            {
                await _financialStatementLoader.Load(triggerMessage, Log);
            }
            catch (Exception ex)
            {
                Log($"Failed to process message: triggerMessage; msg: {ex}");
            }

            Log($"Finished processing. <<<<<");
        }
    }
}
