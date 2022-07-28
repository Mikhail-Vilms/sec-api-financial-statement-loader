using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecApiFinancialStatementLoader.IServices
{
    /// <summary>
    /// Contract for interactions with SNS topics
    /// </summary>
    public interface ISnsService
    {
        /// <summary>
        /// Method that publishes individual message to SNS topic
        /// </summary>
        public Task PublishMsgAsync(string snsMsgJsonStr);

        /// <summary>
        /// Method that publishes list of messages to SNS topic
        /// </summary>
        public Task PublishMsgsAsync(IList<string> snsMsgs);
    }
}
