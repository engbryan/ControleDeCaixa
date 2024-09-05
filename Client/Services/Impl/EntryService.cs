using EntryControl.Core.Entities;
using EntryControl.Providers;
using EntryControl.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntryControl.Services.Impl
{
    public class EntryService : IEntryService
    {
        private readonly IEntryRepository _entryRepository;
        private readonly IUserProvider _userProvider;
        private readonly ILogger<EntryService> _logger;

        public event Action<Entry> OnEntryAdded;

        public EntryService(IEntryRepository entryRepository,
                        IUserProvider userProvider,
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
                var entry = new Entry
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

        public async Task<List<Entry>> GetAll()
        {
            return await _entryRepository.GetAll().ToListAsync();
        }

        public async Task<List<Entry>> GetAllSinchronized()
        {
            return await _entryRepository.GetSynchronized().ToListAsync();
        }

        public async Task<List<Entry>> GetAllDirty()
        {
            return await _entryRepository.GetDirty().ToListAsync();
        }

    }
}