using EntryControl.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace EntryControl
{
    public class SqlServerDesignTimeDbContextFactory : IDesignTimeDbContextFactory<ReportContext>
    {
        public ReportContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ReportContext>();
            builder.UseSqlServer(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"));

            return new ReportContext(builder.Options);
        }
    }
}