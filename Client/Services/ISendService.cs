
using EntryControl.Core.Entities;
using System;
using System.Threading.Tasks;

namespace EntryControl.Services
{
    public interface ISendService
    {
        public event Action<Entry> OnSendCompleted;

        Task<bool> Send(Entry entry);
        Task<bool> SendWithoutInvoke(Entry entry);
    }
}
