using System;

namespace Toggl.Ultrawave.Clients
{
    public interface IStatusClient
    {
        IObservable<bool> Get();
    }
}
