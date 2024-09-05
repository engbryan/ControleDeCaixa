
using EntryControl.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntryControl.Services
{
    public interface ISynchronizeService
    {
        public event Action<IEnumerable<Entry>> OnSyncCompleted;

        Task Synchronize();
        Task CheckForSync(bool isHealthy);
    }
}
