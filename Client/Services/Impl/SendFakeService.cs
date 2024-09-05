using EntryControl.Core.Entities;
using EntryControl.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntryControl.Services.Impl
{
    public class SendFakeService : ISendService
    {
        private readonly IEntryRepository _entryRepository;
        private readonly IEntryService _entryService;
        private readonly ILogger<SendFakeService> _logger;
        private readonly IEntryApi _entryApi;

        public event Action<IEnumerable<Entry>> OnSyncCompleted;
        public event Action<Entry> OnSendCompleted;
        public event Action<Entry> OnSendEntries;

        public SendFakeService(
            IEntryRepository entryRepository,
            IEntryService entryService,
            ILogger<SendFakeService> logger,
            IEntryApi entryApi)
        {
            _entryRepository = entryRepository;
            _entryService = entryService;
            _logger = logger;
            _entryApi = entryApi;

            OnSendEntries += async _ => await Send(_);
            _entryService.OnEntryAdded += _ => OnSendEntries(_);
        }

        public async Task<bool> Send(Entry entry)
        {
            var success = await SendWithoutInvoke(entry);
            if (success)
            {
                await _entryRepository.MarkAsSynchronized(entry.ClientId);

                OnSendCompleted?.Invoke(entry);
            }

            return success;
        }

        public async Task<bool> SendWithoutInvoke(Entry entry)
        {
            try
            {
                var response = await _entryApi.SynchronizeEntriesAsync(entry);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Lançamentos sincronizados com sucesso.");
                    return true;
                }

                _logger.LogWarning($"Erro ao sincronizar lançamentos: {response.StatusCode}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar lançamentos para o serviço online.");
                return false;
            }
        }

    }
}
