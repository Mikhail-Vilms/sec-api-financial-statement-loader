using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using SecApiFinancialStatementLoader.IServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecApiFinancialStatementLoader.Services
{
    public class SnsService : ISnsService
    {
        private readonly string _snsArn = "arn:aws:sns:us-west-2:672009997609:Sec-Api-Financial-Positions-To-Load";

        public async Task PublishFinancialPositionToLoadAsync(string snsMsgJsonStr)
        {
            using (var client = new AmazonSimpleNotificationServiceClient(region: RegionEndpoint.USWest2))
            {
                var request = new PublishRequest(_snsArn, snsMsgJsonStr);
                await client.PublishAsync(request);
            }
        }

        public async Task PublishFinancialPositionsToLoadAsync(IList<string> snsMessages)
        {
            using (var client = new AmazonSimpleNotificationServiceClient(region: RegionEndpoint.USWest2))
            {
                foreach(string snsMsgJsonStr in snsMessages)
                {
                    var request = new PublishRequest(_snsArn, snsMsgJsonStr);
                    await client.PublishAsync(request);
                }
            }
        }
    }
}
