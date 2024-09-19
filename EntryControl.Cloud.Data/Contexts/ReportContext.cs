using EntryControl.Cloud.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace EntryControl.Cloud.Data.Contexts
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

            modelBuilder.Entity<CloudEntry>().HasKey(t => t.CloudId);
            modelBuilder.Entity<DailyReport>().HasKey(t => t.Id);
        }

        public DbSet<CloudEntry> Entries { get; private set; }
        public DbSet<DailyReport> DailyReports { get; private set; }

    }
}
