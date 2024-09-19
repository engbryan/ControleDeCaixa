using EntryControl.Contracts.Modules.Messaging.Enums;
using EntryControl.Contracts.Services;
using EntryControl.POS.Core.Interfaces.Services;
using EntryControl.POS.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EntryControl.POS.Services.Services
{
    public class HealthCheckAWSService : IHealthCheckService
    {
        private readonly ILogger<HealthCheckAWSService> _logger;
        private readonly ISnsClientService _snsClientService;
        private readonly ITimerService _timerProvider;

        public bool IsServiceHealthy { get; private set; }

        public event Action<bool> OnHealthStatusChanged;

        public HealthCheckAWSService(
            ISnsClientService snsClientService,
            ITimerService timerProvider,
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
                var response = await _snsClientService.PublishMessageAsync(new POSEntry(), SnsTopic.HealthCheck);
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

        ~HealthCheckAWSService() => Dispose();
    }
}
