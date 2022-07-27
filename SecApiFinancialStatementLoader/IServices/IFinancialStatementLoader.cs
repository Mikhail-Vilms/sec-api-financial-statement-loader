using SecApiFinancialStatementLoader.Models;
using System;
using System.Threading.Tasks;

namespace SecApiFinancialStatementLoader.IServices
{
    public interface IFinancialStatementLoader
    {
        public Task Load(LambdaTriggerMessage triggerMessage, Action<string> Log);
    }
}
