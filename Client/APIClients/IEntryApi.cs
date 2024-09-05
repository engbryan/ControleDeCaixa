using EntryControl.Core.Entities;
using Refit;
using System.Threading.Tasks;

namespace EntryControl
{
    public interface IEntryApi
    {
        [Post("/entries/synchronize")]
        Task<IApiResponse> SynchronizeEntriesAsync([Body] Entry entries);

        [Get("/healthcheck")]
        Task<IApiResponse> HealthCheckAsync();
    }
}