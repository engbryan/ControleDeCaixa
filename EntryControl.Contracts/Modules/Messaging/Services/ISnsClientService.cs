using Amazon.SimpleNotificationService.Model;
using EntryControl.Contracts.Modules.Messaging.Enums;

namespace EntryControl.Contracts.Services
{
    public interface ISnsClientService
    {
        Task<PublishResponse> PublishMessageAsync<T>(T message, SnsTopic snsTopic)
            where T : class, new();
    }
}
