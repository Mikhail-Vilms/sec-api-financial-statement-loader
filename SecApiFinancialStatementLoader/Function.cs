using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SecApiFinancialStatementLoader.IServices;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SecApiFinancialStatementLoader
{
    public class Function
    {
        private readonly ILambdaInvocationHandler _lambdaInvocationHandler;

        public Function()
        {
            var host = new HostBuilder()
                .SetupHostForLambda()
                .Build();

            var serviceProvider = host.Services;

            _lambdaInvocationHandler = serviceProvider
                .GetRequiredService<ILambdaInvocationHandler>();
        }

        /// <summary>
        /// Function that points to lambda invocation handler
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(SQSEvent sqsEvnt, ILambdaContext context)
        {
            await _lambdaInvocationHandler.FunctionHandler(sqsEvnt, context);
        }
    }
}
