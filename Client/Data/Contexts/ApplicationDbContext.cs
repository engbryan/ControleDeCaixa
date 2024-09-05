using EntryControl.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace EntryControl.POS.Data.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Entry> Entries { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Entry>().HasKey(t => t.ClientId);
        }
    }
}
