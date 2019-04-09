using System;

namespace Toggl.Networking.ApiClients
{
    public interface IUpdatingApiClient<T>
    {
        IObservable<T> Update(T entity);
    }
}
