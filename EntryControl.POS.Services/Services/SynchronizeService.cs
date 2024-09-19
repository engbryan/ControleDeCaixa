using EntryControl.POS.Core.Interfaces.Services;
using EntryControl.POS.Data.Repositories;
using EntryControl.POS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EntryControl.POS.Services.Services
{
    public class SynchronizeService : ISynchronizeService
    {
        private readonly IEntryRepository _entryRepository;
        private readonly IHealthCheckService _healthCheckService;
        private readonly ISendService _sendService;
        private readonly ILogger<SynchronizeService> _logger;

        public event Action<IEnumerable<POSEntry>> OnSyncCompleted;

        public SynchronizeService(
            IEntryRepository entryRepository,
            IHealthCheckService healthCheckService,
            ISendService sendService,
            ILogger<SynchronizeService> logger)
        {
            _entryRepository = entryRepository;
            _healthCheckService = healthCheckService;
            _sendService = sendService;
            _logger = logger;

            _healthCheckService.OnHealthStatusChanged += async _ => await CheckForSync(_);
        }

        public async Task Synchronize()
        {
            _logger.LogInformation("Iniciando sincronizacão.");

            var entries = await _entryRepository.GetDirty().ToListAsync();

            entries.ForEach(async _ => await _sendService.Send(_));

            OnSyncCompleted?.Invoke(entries);
        }

        public async Task CheckForSync(bool isHealthy)
        {
            try
            {
                var anyPendingSync = await _entryRepository.GetDirty().AnyAsync();
                if (isHealthy && anyPendingSync)
                {
                    await Synchronize();
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Erro de multithread.");
            }
        }

    }
}
