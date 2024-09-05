using EntryControl.Core.Entities;
using EntryControl.Core.Enums;
using EntryControl.Core.Services.Impl;
using System;
using System.Threading.Tasks;

namespace EntryControl.Core.Services
{
    public class ExampleDataService
    {
        private readonly SnsClientService _snsClientService;

        public ExampleDataService(SnsClientService snsClientService)
        {
            _snsClientService = snsClientService;
        }

        public async Task CommitEntryAsync()
        {
            var message = new Entry
            {
                UserId = "user1",
                ClientId = 123,
                Type = "debit",
                Amount = 100.50m,
                Description = "Payment for services",
                EntryDate = DateTime.UtcNow,
                Synchronized = false
            };

            var response = await _snsClientService.PublishMessageAsync(message, SnsTopic.CommitEntry);
        }
    }
}
