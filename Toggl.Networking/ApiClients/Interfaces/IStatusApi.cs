using System;
using System.Reactive;

namespace Toggl.Networking.ApiClients
{
    public interface IStatusApi
    {
        IObservable<Unit> IsAvailable();
    }
}
