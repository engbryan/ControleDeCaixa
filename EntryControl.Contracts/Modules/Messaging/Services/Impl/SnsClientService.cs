
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using EntryControl.Contracts.Modules.Messaging.Enums;
using System.Text.Json;

namespace EntryControl.Contracts.Services.Impl
{
    public class SnsClientService : ISnsClientService
    {
        private readonly IAmazonSimpleNotificationService _snsClient;

        public SnsClientService(IAmazonSimpleNotificationService snsClient)
        {
            _snsClient = snsClient;
        }

        public async Task<PublishResponse> PublishMessageAsync<T>(T message, SnsTopic snsTopic)
            where T : class, new()

        {
            var messageJson = JsonSerializer.Serialize(message);
            var publishRequest = new PublishRequest
            {
                TopicArn = snsTopic.GetArn(),
                Message = messageJson,
                MessageAttributes = new()
                {
                    { "MessageType", new() { DataType = "String", StringValue = typeof(T).FullName } }
                }
            };

            return await _snsClient.PublishAsync(publishRequest);
        }
    }
}
