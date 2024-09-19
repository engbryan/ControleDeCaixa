using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using EntryControl.Cloud.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EntryControl.Lambdas
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            var function = new ReportsLambda();

            Func<ILambdaContext, Task<APIGatewayProxyResponse>> handler = function.FunctionHandler;
            await LambdaBootstrapBuilder.Create(handler, new DefaultLambdaJsonSerializer())
                .Build()
                .RunAsync();
        }
    }

    public class ReportsLambda
    {
        private readonly IServiceProvider _serviceProvider;

        public ReportsLambda()
        {
            var services = new ServiceCollection();
            services.AddDbContext<ReportContext>(
                contextLifetime: ServiceLifetime.Scoped,
                optionsLifetime: ServiceLifetime.Singleton
                );

            _serviceProvider = services.BuildServiceProvider();

            using (var serviceScope = _serviceProvider.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ReportContext>();
                context.Database.Migrate();
            }
        }

        public async Task<APIGatewayProxyResponse> FunctionHandler(ILambdaContext context)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ReportContext>();

                context.Logger.LogLine("Fetching all daily reports.");

                var reports = await dbContext.DailyReports.OrderByDescending(r => r.ReportDate).ToListAsync();


                var response = new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = JsonSerializer.Serialize(reports)
                };

                response.Headers = new Dictionary<string, string>();
                response.Headers.Add("Content-Type", "application/json");
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Access-Control-Allow-Headers", "*");
                response.Headers.Add("Access-Control-Allow-Methods", "*");
                response.Headers.Add("Access-Control-Allow-Credentials", "true");

                return response;
            }
            catch (Exception ex)
            {
                context.Logger.LogLine($"Error obtaining reports.");
                throw;
            }
        }
    }

}
