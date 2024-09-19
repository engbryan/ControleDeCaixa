using EntryControl.POS.Core.Models;

namespace EntryControl.POS.Core.Interfaces.Services
{
    public interface ITimerService : IDisposable
    {
        void SetHealthCheckTimer(int interval);
        void OnOptionsChanged(ExternalServiceOptions options);
        void ConfigureAction(Action action);
    }
}