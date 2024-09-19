using EntryControl.POS.Domain.Entities;

namespace EntryControl.POS.Core.Interfaces.Services
{
    public interface ISynchronizeService
    {
        public event Action<IEnumerable<POSEntry>> OnSyncCompleted;

        Task Synchronize();
        Task CheckForSync(bool isHealthy);
    }
}
