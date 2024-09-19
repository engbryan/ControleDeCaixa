namespace EntryControl.POS.Core.Interfaces.Services
{
    public interface IHealthCheckService : IDisposable
    {
        event Action<bool> OnHealthStatusChanged;
        bool IsServiceHealthy { get; }
        Task<bool> CheckHealthAsync();
    }

}
