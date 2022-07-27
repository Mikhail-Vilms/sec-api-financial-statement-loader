using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SecApiFinancialStatementLoader.Services;
using SecApiFinancialStatementLoader.IServices;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Amazon.DynamoDBv2;
using Amazon;

namespace SecApiFinancialStatementLoader
{
    public static class Startup
    {
        internal static IHostBuilder SetupHostForLambda(this IHostBuilder hostBuilder) =>
            hostBuilder.AddRuntimeDependenciesBinding();

        private static IHostBuilder AddRuntimeDependenciesBinding(this IHostBuilder hostBuilder) => hostBuilder
            .ConfigureServices((context, serviceCollection) => serviceCollection
                .AddSingleton<ILambdaInvocationHandler, LambdaInvocationHandler>()
                .AddSingleton<IDeserializer, Deserializer>()
                .AddSingleton<IFinancialStatementLoader, FinancialStatementLoader>()

                .AddSingleton<IReportDetailsService, ReportDetailsService>()
                .AddSingleton<ISecApiClient, SecApiClient>()

                .AddSingleton<ITaxonomyExtSchemaService, TaxonomyExtSchemaService>()
                .AddSingleton<ITaxonomyExtLinkbaseService, TaxonomyExtLinkbaseService>()

                .AddSingleton<ISnsService, SnsService>()
                .TryAddSingleton<IDynamoDBContext>(provider =>
                {
                    var client = new AmazonDynamoDBClient(RegionEndpoint.USWest2);
                    return new DynamoDBContext(client, new DynamoDBContextConfig()
                    {
                        ConsistentRead = false
                    });
                }));
    }
}
