using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using SecApiFinancialStatementLoader.IServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecApiFinancialStatementLoader.Services
{
    /// <summary>
    /// Class responsible for interactions with SNS topics
    /// </summary>
    public class SnsService : ISnsService
    {
        private readonly string _snsArn = "arn:aws:sns:us-west-2:672009997609:Sec-Api-Financial-Positions-To-Load";

        /// <inheritdoc />
        public async Task PublishMsgAsync(string snsMsgJsonStr)
        {
            using (var client = new AmazonSimpleNotificationServiceClient(region: RegionEndpoint.USWest2))
            {
                var request = new PublishRequest(_snsArn, snsMsgJsonStr);
                await client.PublishAsync(request);
            }
        }

        /// <inheritdoc />
        public async Task PublishMsgsAsync(IList<string> snsMsgs)
        {
            using (var client = new AmazonSimpleNotificationServiceClient(region: RegionEndpoint.USWest2))
            {
                foreach(string snsMsgJsonStr in snsMsgs)
                {
                    var request = new PublishRequest(_snsArn, snsMsgJsonStr);
                    await client.PublishAsync(request);
                }
            }
        }
    }
}
