using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using Amazon.Lambda.SQSEvents;
using Amazon.Runtime.Telemetry;
using Amazon.Runtime;
using EntryControl.Core.Entities;
using EntryControl.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

namespace EntryControl.Lambdas.Tests
{
    public class RegisterMovementLambdaTests
    {
        [Fact]
        public async Task TestFunctionHandler_ValidMessage_ShouldRegisterMovement()
        {
            // Arrange
            var services = new ServiceCollection();

            var options = new DbContextOptionsBuilder<ReportContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDb");
            services.AddScoped(_ => new ReportContext(options.Options));

            var serviceProvider = services.BuildServiceProvider();

            var lambda = new RegisterMovementLambda(serviceProvider);

            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Mocks", "TestFunctionHandler_ValidMessage_ShouldRegisterMovement.json");
            var jsonMessage = File.ReadAllText(jsonPath);

            var sqsEvent = JsonSerializer.Deserialize<SQSEvent>(jsonMessage);

            var mockContext = new Mock<ILambdaContext>();
            mockContext.Setup(x => x.Logger).Returns(Mock.Of<ILambdaLogger>());

            var dbContext = serviceProvider.GetRequiredService<ReportContext>();

            // Act
            await lambda.FunctionHandler(sqsEvent, mockContext.Object);

            // Assert
            var registeredEntry = await dbContext.Entries.FirstOrDefaultAsync();
            Assert.NotNull(registeredEntry);
            Assert.Equal(8, registeredEntry.ClientId);
            Assert.Equal(213, registeredEntry.Amount);
        }
    }
}
