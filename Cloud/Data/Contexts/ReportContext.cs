
using EntryControl.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace EntryControl.Data.Contexts
{
    public class ReportContext : DbContext
    {
        public ReportContext(DbContextOptions<ReportContext> options)
        : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Entry>().HasKey(t => t.CloudId);
            modelBuilder.Entity<DailyReport>().HasKey(t => t.Id);
        }

        public DbSet<Entry> Entries { get; set; }
        public DbSet<DailyReport> DailyReports { get; set; }

    }
}
