
using System;

namespace EntryControl.Core.Entities
{
    public class DailyReport
    {
        public int Id { get; set; }
        public DateTime ReportDate { get; set; }
        public decimal TotalCredits { get; set; }
        public decimal TotalDebits { get; set; }
        public decimal Balance { get; set; }
    }
}
