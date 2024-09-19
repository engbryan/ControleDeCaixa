using EntryControl.Contracts.Data;
using System;

namespace EntryControl.Cloud.Domain.Entities
{
    public class DailyReport : IEntity
    {
        public int Id { get; set; }
        public DateTime ReportDate { get; set; }
        public decimal TotalCredits { get; set; }
        public decimal TotalDebits { get; set; }
        public decimal Balance { get; set; }
    }
}
