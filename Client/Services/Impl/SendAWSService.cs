using EntryControl.Core.Entities;
using EntryControl.Core.Services;
using EntryControl.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntryControl.Services.Impl
{
    public class SendAWSService : ISendService
    {
        private readonly ISnsClientService _snsClientService;
        private readonly IEntryRepository _entryRepository;
        private readonly IEntryService _entryService;
        private readonly ILogger<SendAWSService> _logger;

        public event Action<IEnumerable<Entry>> OnSyncCompleted;
        public event Action<Entry> OnSendCompleted;
        public event Action<Entry> OnSendEntries;

        public SendAWSService(
            ISnsClientService snsClientService,
            IEntryRepository entryRepository,
            IEntryService entryService,
            ILogger<SendAWSService> logger)
        {
            _snsClientService = snsClientService;
            _entryRepository = entryRepository;
            _entryService = entryService;
            _logger = logger; ;

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
                var response = await _snsClientService.PublishMessageAsync(entry, Core.Enums.SnsTopic.CommitEntry);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation("Lançamentos sincronizados com sucesso.");
                    return true;
                }

                _logger.LogWarning($"Erro ao sincronizar lançamentos: {response.HttpStatusCode}");
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
