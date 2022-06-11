using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SecApiFinancialStatementLoader.Services;
using SecApiFinancialStatementLoader.IServices;

namespace SecApiFinancialStatementLoader
{
    public static class Startup
    {
        internal static IHostBuilder SetupHostForLambdaFunction(this IHostBuilder hostBuilder) =>
            hostBuilder
                .AddRuntime();

        internal static IHostBuilder AddRuntime(this IHostBuilder hostBuilder) =>
            hostBuilder.ConfigureServices((context, services) => services
                .AddSingleton<IDeserializer, Deserializer>()
                .AddSingleton<ISnsService, SnsService>());
    }
}
