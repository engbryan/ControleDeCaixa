using EntryControl.Core.Entities;
using EntryControl.Core.Enums;
using EntryControl.Core.Services;
using EntryControl.Providers;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EntryControl.Services.Impl
{
    public class HealthCheckAWSService : IHealthCheckService
    {
        private readonly ILogger<HealthCheckAWSService> _logger;
        private readonly ISnsClientService _snsClientService;
        private readonly ITimerProvider _timerProvider;

        public bool IsServiceHealthy { get; private set; }

        public event Action<bool> OnHealthStatusChanged;

        public HealthCheckAWSService(
            ISnsClientService snsClientService,
            ITimerProvider timerProvider,
            ILogger<HealthCheckAWSService> logger)
        {
            _snsClientService = snsClientService;
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
                var response = await _snsClientService.PublishMessageAsync(new Entry(), SnsTopic.HealthCheck);
                IsServiceHealthy = response.HttpStatusCode == System.Net.HttpStatusCode.OK;

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

        ~HealthCheckAWSService() => this.Dispose();
    }
}
