using System;

namespace Toggl.Ultrawave.ApiClients
{
    public interface IStatusApi
    {
        IObservable<bool> Get();
    }
}
