using EntryControl.Entities;
using System;

namespace EntryControl.Providers
{
    public interface ITimerProvider : IDisposable
    {
        void SetHealthCheckTimer(int interval);
        void OnOptionsChanged(ExternalServiceOptions options);
        void ConfigureAction(Action action);
    }
}