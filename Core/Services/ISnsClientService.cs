using Amazon.SimpleNotificationService.Model;
using EntryControl.Core.Enums;
using System.Threading.Tasks;

namespace EntryControl.Core.Services
{
    public interface ISnsClientService
    {
        Task<PublishResponse> PublishMessageAsync<T>(T message, SnsTopic snsTopic)
            where T : class, new();
    }
}
