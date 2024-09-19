using EntryControl.POS.Domain.Entities;

namespace EntryControl.POS.Core.Interfaces.Services
{
    public interface ISendService
    {
        public event Action<POSEntry> OnSendCompleted;

        Task<bool> Send(POSEntry entry);
        Task<bool> SendWithoutInvoke(POSEntry entry);
    }
}
