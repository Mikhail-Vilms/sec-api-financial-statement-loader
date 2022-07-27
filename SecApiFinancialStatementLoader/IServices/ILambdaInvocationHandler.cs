using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using System.Threading.Tasks;

namespace SecApiFinancialStatementLoader.IServices
{
    public interface ILambdaInvocationHandler
    {
        public Task FunctionHandler(SQSEvent sqsEvnt, ILambdaContext context);
    }
}
