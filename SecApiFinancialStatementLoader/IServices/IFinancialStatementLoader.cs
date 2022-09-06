using SecApiFinancialStatementLoader.Models;
using System;
using System.Threading.Tasks;

namespace SecApiFinancialStatementLoader.IServices
{
    public interface IFinancialStatementLoader
    {
        public Task Load(FinancialStatementDetails finStatementDetails, Action<string> Log);
    }
}
