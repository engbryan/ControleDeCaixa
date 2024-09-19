using EntryControl.POS.Data.Contexts;
using EntryControl.POS.Domain.Entities;

namespace EntryControl.POS.Data.Repositories
{
    public class EntryRepository : IEntryRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationDbContext ApplicationDbContext { get; set; }

        public EntryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(POSEntry entry)
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

        public IQueryable<POSEntry> GetDirty()
        {
            return _context.Entries
                           .Where(t => !t.Synchronized);
        }

        public IQueryable<POSEntry> GetAll()
        {
            return _context.Entries;
        }

        public IQueryable<POSEntry> GetSynchronized()
        {
            return _context.Entries
                           .Where(t => t.Synchronized);
        }

    }
}
