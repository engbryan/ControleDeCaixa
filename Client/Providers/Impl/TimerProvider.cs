using EntryControl.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EntryControl.Providers.Impl
{
    public class TimerProvider : ITimerProvider
    {
        private readonly IOptionsMonitor<ExternalServiceOptions> _optionsMonitor;
        private readonly ILogger<TimerProvider> _logger;

        private Timer _healthCheckTimer;
        private Task<Action> _action;
        private bool _isRunning;

        public TimerProvider(IOptionsMonitor<ExternalServiceOptions> optionsMonitor,
                                  ILogger<TimerProvider> logger)
        {
            _optionsMonitor = optionsMonitor;
            _logger = logger;

            _optionsMonitor.OnChange(OnOptionsChanged);

            SetHealthCheckTimer(_optionsMonitor.CurrentValue.HealthCheckInterval);
        }

        private async Task Act()
        {
            if (_action != null && !_isRunning)
            {
                try
                {
                    _isRunning = true;
                    var exec = await _action;
                    exec();
                }
                finally
                {
                    _isRunning = false;
                }
            }
            else
            {
                _logger.LogInformation("A execução anterior ainda está em andamento.");
            }
        }

        public void OnOptionsChanged(ExternalServiceOptions options)
        {
            _logger.LogInformation("As configurações do serviço foram alteradas. Reconfigurando o timer de health check.");
            SetHealthCheckTimer(options.HealthCheckInterval);
        }

        public void SetHealthCheckTimer(int interval)
        {
            _healthCheckTimer?.Dispose();
            _healthCheckTimer = new Timer(async state => await Act(), null, TimeSpan.Zero, TimeSpan.FromMilliseconds(interval));
            _logger.LogInformation($"Health check timer configurado com intervalo de {interval} milissegundos.");
        }

        public void ConfigureAction(Action action)
        {
            _action = Task.FromResult(action);
        }

        public void Dispose()
        {
            _healthCheckTimer?.Dispose();
        }

        ~TimerProvider() => Dispose();
    }
}
