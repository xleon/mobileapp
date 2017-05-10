using System;

namespace Toggl.Ultrawave
{
    public interface IStatusClient
    {
        IObservable<bool> Get();
    }
}
