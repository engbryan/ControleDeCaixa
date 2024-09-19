using EntryControl.POS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EntryControl.POS.Data.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<POSEntry> Entries { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<POSEntry>().HasKey(t => t.ClientId);
        }
    }
}
