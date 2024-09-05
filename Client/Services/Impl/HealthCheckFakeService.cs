using EntryControl.Providers;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EntryControl.Services.Impl
{
    public class HealthCheckFakeService : IHealthCheckService
    {
        private readonly ILogger<HealthCheckAWSService> _logger;
        private readonly IEntryApi _entryApi;
        private readonly ITimerProvider _timerProvider;

        public bool IsServiceHealthy { get; private set; }

        public event Action<bool> OnHealthStatusChanged;

        public HealthCheckFakeService(IEntryApi entryApi,
                                  ITimerProvider timerProvider,
                                  ILogger<HealthCheckAWSService> logger)
        {
            _entryApi = entryApi;
            _timerProvider = timerProvider;
            _logger = logger;

            _timerProvider.ConfigureAction(async () => await CheckHealthAsync());
        }

        public async Task<bool> CheckHealthAsync()
        {
            _logger.LogInformation("Iniciando Health check do serviço online.");

            bool previousHealthStatus = IsServiceHealthy;

            try
            {
                var response = await _entryApi.HealthCheckAsync();
                IsServiceHealthy = response.IsSuccessStatusCode;

                if (IsServiceHealthy)
                {
                    _logger.LogInformation("Health check bem-sucedido no serviço online.");
                }
                else
                {
                    _logger.LogWarning("Health check falhou no serviço online.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao realizar health check: {ex.Message}");
                IsServiceHealthy = false;
            }

            if (IsServiceHealthy != previousHealthStatus)
            {
                OnHealthStatusChanged?.Invoke(IsServiceHealthy);
            }

            return IsServiceHealthy;
        }

        public void Dispose()
        {
            _timerProvider.Dispose();
        }

        ~HealthCheckFakeService() => this.Dispose();
    }
}
