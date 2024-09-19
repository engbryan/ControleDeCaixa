
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Lambda.SQSEvents;
using EntryControl.Cloud.Core.Messages;
using EntryControl.Cloud.Data.Contexts;
using EntryControl.Cloud.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using static Amazon.Lambda.SNSEvents.SNSEvent;

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

            var function = new DailyClosureLambda(serviceProvider);

            Func<SQSEvent, ILambdaContext, Task> handler = function.FunctionHandler;
            await LambdaBootstrapBuilder.Create(handler, new DefaultLambdaJsonSerializer())
                .Build()
                .RunAsync();
        }
    }

    public class DailyClosureLambda
    {
        private readonly IServiceProvider _serviceProvider;

        public DailyClosureLambda(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
        {
            foreach (var message in evnt.Records)
            {
                using IServiceScope scope = await ProcessMessageAsync(message, context);
            }
        }

        private async Task<IServiceScope> ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ReportContext>();

            context.Logger.LogLine($"Processing daily closure for message {message.Body}");

            var snsMessage = JsonSerializer.Deserialize<SNSMessage>(message.Body);
            var dayClosure = JsonSerializer.Deserialize<DayClosure>(snsMessage.Message);

            var dayClosureDate = dayClosure.AskedDate.Date;

            context.Logger.LogLine($"Processing report for date: {dayClosureDate}");

            // Obter todas as transações de hoje que não foram processadas
            var transactions = await dbContext.Entries
                .Where(e => e.EntryDate.Date == dayClosureDate)
                .ToListAsync();

            if (!transactions.Any())
            {
                context.Logger.LogLine("No transactions to process for today's closure.");
                return scope;
            }

            // Calcular totais gerais
            var totalCredits = transactions.Where(e => e.Type == "Credit").Sum(e => e.Amount);
            var totalDebits = transactions.Where(e => e.Type == "Debit").Sum(e => e.Amount);
            var balance = totalCredits - totalDebits;

            // Gerar relatório de fechamento geral
            var report = new DailyReport
            {
                ReportDate = dayClosureDate,
                TotalCredits = totalCredits,
                TotalDebits = totalDebits,
                Balance = balance,
            };

            dbContext.DailyReports.Add(report);

            // Salvar mudanças no banco de dados
            await dbContext.SaveChangesAsync();

            context.Logger.LogLine("Daily closure completed successfully.");

            return scope;
        }
    }

}
