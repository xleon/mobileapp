using System;

namespace Toggl.Networking.ApiClients
{
    public interface ICreatingApiClient<T>
    {
        IObservable<T> Create(T entity);
    }
}
