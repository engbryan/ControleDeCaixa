using System;
using System.Threading.Tasks;

namespace EntryControl.Services
{
    public interface IHealthCheckService : IDisposable
    {
        event Action<bool> OnHealthStatusChanged;
        bool IsServiceHealthy { get; }
        Task<bool> CheckHealthAsync();
    }

}
