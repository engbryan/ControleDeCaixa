using EntryControl.POS.Core.Interfaces.Services;
using EntryControl.POS.Data.Repositories;
using EntryControl.POS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EntryControl.POS.Services.Services
{
    public class EntryService : IEntryService
    {
        private readonly IEntryRepository _entryRepository;
        private readonly IUserService _userProvider;
        private readonly ILogger<EntryService> _logger;

        public event Action<POSEntry> OnEntryAdded;

        public EntryService(IEntryRepository entryRepository,
                        IUserService userProvider,
                        ILogger<EntryService> logger)
        {
            _entryRepository = entryRepository;
            _userProvider = userProvider;
            _logger = logger;
        }

        public async Task Add(string type, decimal ammount, string description)
        {
            _logger.LogInformation("Adicionando lançamento.");

            try
            {
                var entry = new POSEntry
                {
                    Type = type,
                    Amount = ammount,
                    Description = description,
                    EntryDate = DateTime.Now,
                    UserId = _userProvider.GetMachineUserDomainName(),
                    Synchronized = false
                };

                await _entryRepository.Add(entry);
                _logger.LogInformation("Lançamento adicionado com sucesso.");

                OnEntryAdded?.Invoke(entry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar lançamento");
                throw;
            }
        }

        public async Task<List<POSEntry>> GetAll()
        {
            return await _entryRepository.GetAll().ToListAsync();
        }

        public async Task<List<POSEntry>> GetAllSinchronized()
        {
            return await _entryRepository.GetSynchronized().ToListAsync();
        }

        public async Task<List<POSEntry>> GetAllDirty()
        {
            return await _entryRepository.GetDirty().ToListAsync();
        }

        public async Task MarkAsSynchronized(int entryId)
        {
            await _entryRepository.MarkAsSynchronized(entryId);
        }
    }
}