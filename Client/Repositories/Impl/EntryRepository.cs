using EntryControl.Core.Entities;
using EntryControl.POS.Data.Contexts;
using System.Linq;
using System.Threading.Tasks;

namespace EntryControl.Repositories.Impl
{
    public class EntryRepository : IEntryRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationDbContext ApplicationDbContext { get; set; }

        public EntryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(Entry entry)
        {
            await _context.Entries.AddAsync(entry);
            await _context.SaveChangesAsync();
        }

        public async Task MarkAsSynchronized(int entryId)
        {
            var entry = await _context.Entries.FindAsync(entryId);
            if (entry != null)
            {
                entry.Synchronized = true;
                await _context.SaveChangesAsync();
            }
        }

        public IQueryable<Entry> GetDirty()
        {
            return _context.Entries
                           .Where(t => !t.Synchronized);
        }

        public IQueryable<Entry> GetAll()
        {
            return _context.Entries;
        }

        public IQueryable<Entry> GetSynchronized()
        {
            return _context.Entries
                           .Where(t => t.Synchronized);
        }

    }
}
