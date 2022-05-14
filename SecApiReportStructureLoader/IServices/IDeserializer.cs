using Amazon.Lambda.SQSEvents;
using SecApiReportStructureLoader.Models;

namespace SecApiReportStructureLoader.IServices
{
    public interface IDeserializer
    {
        LambdaTriggerMessage Get(SQSEvent.SQSMessage sqsMessage);
    }
}
