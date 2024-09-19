using EntryControl.POS.Domain.Entities;

namespace EntryControl.POS.Core.Interfaces.Services
{
    public interface IEntryService
    {
        event Action<POSEntry> OnEntryAdded;
        Task Add(string type, decimal ammount, string description);
        Task MarkAsSynchronized(int entryId);
        Task<List<POSEntry>> GetAll();
        Task<List<POSEntry>> GetAllSinchronized();
        Task<List<POSEntry>> GetAllDirty();
    }
}