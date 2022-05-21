using Amazon.Lambda.SQSEvents;
using SecApiReportStructureLoader.IServices;
using SecApiReportStructureLoader.Models;
using System.Text.Json;

namespace SecApiReportStructureLoader.Services
{
    public class Deserializer : IDeserializer
    {
        public class SNSMessage {
            public string Type { get; set; }
            public string MessageId { get; set; }
            public string TopicArn { get; set; }
            public string Message { get; set; }
            public string Timestamp { get; set; }
            public string SignatureVersion { get; set; }
            public string Signature { get; set; }
            public string SigningCertURL { get; set; }
            public string UnsubscribeURL { get; set; }
        }

        public LambdaTriggerMessage Get(SQSEvent.SQSMessage triggerMessage)
        {
            // First we trying to understand if this message was posted to the queue from an SNS ropic subscription
            // And deserialize it accordingly
            var snsWrapper = JsonSerializer.Deserialize<SNSMessage>(triggerMessage.Body);

            if (snsWrapper != null &&
                !string.IsNullOrWhiteSpace(snsWrapper.Type) &&
                !string.IsNullOrWhiteSpace(snsWrapper.MessageId) &&
                !string.IsNullOrWhiteSpace(snsWrapper.TopicArn) &&
                !string.IsNullOrWhiteSpace(snsWrapper.Message) &&
                !string.IsNullOrWhiteSpace(snsWrapper.Timestamp) &&
                !string.IsNullOrWhiteSpace(snsWrapper.SignatureVersion) &&
                !string.IsNullOrWhiteSpace(snsWrapper.Signature) &&
                !string.IsNullOrWhiteSpace(snsWrapper.SigningCertURL) &&
                !string.IsNullOrWhiteSpace(snsWrapper.UnsubscribeURL))
            {
                return JsonSerializer.Deserialize<LambdaTriggerMessage>(snsWrapper.Message);
            }

            // Otherwise we deserialize it as sqs message that was created manually and has no sns wrapper around it
            return JsonSerializer.Deserialize<LambdaTriggerMessage>(triggerMessage.Body);
        }
    }
}
