namespace EntryControl.Contracts.Data.Entities
{
    public interface IEntry : IEntity
    {
        int ClientId { get; set; }
        int CloudId { get; set; }
        string UserId { get; set; }
        string Type { get; set; }
        decimal Amount { get; set; }
        string Description { get; set; }
        DateTime EntryDate { get; set; }
        bool Synchronized { get; set; }
    }
}
