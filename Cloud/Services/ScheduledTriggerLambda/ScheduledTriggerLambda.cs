using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.Lambda.CloudWatchEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.SimpleNotificationService;
using EntryControl.Core.Entities;
using EntryControl.Core.Enums;
using EntryControl.Core.Services;
using EntryControl.Core.Services.Impl;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EntryControl.Lambdas.ScheduledTriggerLambda
{
    public class Fuction
    {
        private static async Task Main(string[] args)
        {
            var function = new ScheduledTriggerLambda();

            Func<CloudWatchEvent<object>, ILambdaContext, Task> handler = function.FunctionHandler;
            await LambdaBootstrapBuilder.Create(handler, new DefaultLambdaJsonSerializer())
                .Build()
                .RunAsync();
        }
    }

    public class ScheduledTriggerLambda
    {
        private readonly ISnsClientService _snsClientService;

        public ScheduledTriggerLambda()
        {
            _snsClientService = new SnsClientService(new AmazonSimpleNotificationServiceClient(RegionEndpoint.USEast1));
        }

        public async Task FunctionHandler(CloudWatchEvent<object> evnt, ILambdaContext context)
        {
            context.Logger.LogLine("Sending Daily Closure event.");

            DayClosure message = new()
            {
                AskedDate = DateTime.Now.Date
            };

            var response = await _snsClientService.PublishMessageAsync(message, SnsTopic.GenerateReport);
            
            context.Logger.LogLine("Daily Closure event sent successfully.");
        }
    }
}
