using Amazon.Lambda.SQSEvents;
using SecApiFinancialStatementLoader.Models;

namespace SecApiFinancialStatementLoader.IServices
{
    public interface IDeserializer
    {
        LambdaTriggerMessage Get(SQSEvent.SQSMessage sqsMessage);
    }
}
