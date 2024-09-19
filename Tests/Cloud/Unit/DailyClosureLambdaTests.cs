using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using EntryControl.Cloud.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace EntryControl.Lambdas.Tests
{
    public class DailyClosureLambdaTests
    {
        [Fact]
        public async Task TestFunctionHandler_ValidMessage_ShouldProcessDailyClosure()
        {
            // Arrange
            var services = new ServiceCollection();

            var options = new DbContextOptionsBuilder<ReportContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDb");
            services.AddScoped(_ => new ReportContext(options.Options));

            var serviceProvider = services.BuildServiceProvider();

            var lambda = new DailyClosureLambda(serviceProvider);

            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "mocks", "TestFunctionHandler_ValidMessage_ShouldProcessDailyClosure.json");
            var jsonMessage = await File.ReadAllTextAsync(jsonPath);

            var sqsEvent = JsonSerializer.Deserialize<SQSEvent>(jsonMessage);

            var mockContext = new Mock<ILambdaContext>();
            mockContext.Setup(x => x.Logger).Returns(Mock.Of<ILambdaLogger>());

            var dbContext = serviceProvider.GetRequiredService<ReportContext>();

            // Act
            await lambda.FunctionHandler(sqsEvent, mockContext.Object);

            // Assert
            var dailyReport = await dbContext.DailyReports.FirstOrDefaultAsync();
            Assert.NotNull(dailyReport);
            Assert.Equal(2024, dailyReport.ReportDate.Year);
            Assert.Equal(9, dailyReport.ReportDate.Month);
            Assert.Equal(4, dailyReport.ReportDate.Day);
        }
    }
}
