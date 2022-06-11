using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecApiFinancialStatementLoader.IServices
{
    public interface ISnsService
    {
        public Task PublishFinancialPositionToLoadAsync(string snsMsgJsonStr);
        public Task PublishFinancialPositionsToLoadAsync(IList<string> snsMessages);
    }
}
