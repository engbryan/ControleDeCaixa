using EntryControl.Contracts.Data.Entities;

namespace EntryControl.POS.Domain.Entities
{
    public class POSEntry : IEntry
    {
        public int ClientId { get; set; }
        public int CloudId { get; set; }
        public string UserId { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime EntryDate { get; set; }
        public bool Synchronized { get; set; }
    }
}
