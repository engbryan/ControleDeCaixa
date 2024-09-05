using EntryControl.Core.Entities;
using EntryControl.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntryControl.Services.Impl
{
    public class SynchronizeService : ISynchronizeService
    {
        private readonly IEntryRepository _entryRepository;
        private readonly IHealthCheckService _healthCheckService;
        private readonly ISendService _sendService;
        private readonly ILogger<SynchronizeService> _logger;

        public event Action<IEnumerable<Entry>> OnSyncCompleted;

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
