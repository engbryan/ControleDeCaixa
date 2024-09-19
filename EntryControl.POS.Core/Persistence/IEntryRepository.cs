//using EntryControl.POS.Data.Contexts;
using EntryControl.POS.Domain.Entities;

namespace EntryControl.POS.Data.Repositories
{
    public interface IEntryRepository
    {
        //ApplicationDbContext ApplicationDbContext { get; }
        Task Add(POSEntry entry);
        Task MarkAsSynchronized(int entryId);
        IQueryable<POSEntry> GetDirty();
        IQueryable<POSEntry> GetAll();
        IQueryable<POSEntry> GetSynchronized();
    }
}
