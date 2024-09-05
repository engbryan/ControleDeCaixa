using EntryControl.Core.Entities;
using EntryControl.POS.Data.Contexts;
using System.Linq;
using System.Threading.Tasks;

namespace EntryControl.Repositories
{
    public interface IEntryRepository
    {
        ApplicationDbContext ApplicationDbContext { get; }
        Task Add(Entry entry);
        Task MarkAsSynchronized(int entryId);
        IQueryable<Entry> GetDirty();
        IQueryable<Entry> GetAll();
        IQueryable<Entry> GetSynchronized();
    }
}
