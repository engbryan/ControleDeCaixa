
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Lambda.SQSEvents;
using EntryControl.Cloud.Data.Contexts;
using EntryControl.Cloud.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using static Amazon.Lambda.SNSEvents.SNSEvent;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EntryControl.Lambdas
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddDbContext<ReportContext>(
                contextLifetime: ServiceLifetime.Scoped,
                optionsLifetime: ServiceLifetime.Singleton
                );

            var serviceProvider = services.BuildServiceProvider();

            using (var serviceScope = serviceProvider.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ReportContext>();
                context.Database.Migrate();
            }

            var function = new RegisterMovementLambda(serviceProvider);

            Func<SQSEvent, ILambdaContext, Task> handler = function.FunctionHandler;
            await LambdaBootstrapBuilder.Create(handler, new DefaultLambdaJsonSerializer())
                .Build()
                .RunAsync();
        }
    }

    public class RegisterMovementLambda
    {
        private readonly IServiceProvider _serviceProvider;

        public RegisterMovementLambda(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
        {
            foreach (var record in evnt.Records)
            {
                using IServiceScope scope = await ProcessMessage(record, context);
            }
        }

        private async Task<IServiceScope> ProcessMessage(SQSEvent.SQSMessage record, ILambdaContext context)
        {
            var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ReportContext>();

            var snsMessage = JsonSerializer.Deserialize<SNSMessage>(record.Body);
            var entry = JsonSerializer.Deserialize<CloudEntry>(snsMessage.Message)
                ?? throw new ArgumentException("Message body is not valid.", nameof(record.Body));

            if (entry != null)
            {
                dbContext.Entries.Add(entry);
                await dbContext.SaveChangesAsync();

                context.Logger.LogLine($"Movement {entry.CloudId} registered successfully.");
            }
            else
            {
                context.Logger.LogLine("Failed to deserialize the movement.");
            }

            return scope;
        }
    }

}
