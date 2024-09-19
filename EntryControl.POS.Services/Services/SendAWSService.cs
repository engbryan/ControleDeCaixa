using EntryControl.Contracts.Modules.Messaging.Enums;
using EntryControl.Contracts.Services;
using EntryControl.POS.Core.Interfaces.Services;
using EntryControl.POS.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EntryControl.POS.Services.Services
{
    public class SendAWSService : ISendService
    {
        private readonly ISnsClientService _snsClientService;
        private readonly IEntryService _entryService;
        private readonly ILogger<SendAWSService> _logger;

        public event Action<IEnumerable<POSEntry>> OnSyncCompleted;
        public event Action<POSEntry> OnSendCompleted;
        public event Action<POSEntry> OnSendEntries;

        public SendAWSService(
            ISnsClientService snsClientService,
            IEntryService entryService,
            ILogger<SendAWSService> logger)
        {
            _snsClientService = snsClientService;
            _entryService = entryService;
            _logger = logger; ;

            OnSendEntries += async _ => await Send(_);
            _entryService.OnEntryAdded += _ => OnSendEntries(_);
        }

        public async Task<bool> Send(POSEntry entry)
        {
            var success = await SendWithoutInvoke(entry);
            if (success)
            {
                await _entryService.MarkAsSynchronized(entry.ClientId);

                OnSendCompleted?.Invoke(entry);
            }

            return success;
        }

        public async Task<bool> SendWithoutInvoke(POSEntry entry)
        {
            try
            {
                var response = await _snsClientService.PublishMessageAsync(entry, SnsTopic.CommitEntry);

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
