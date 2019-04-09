using System;
using System.Reactive;

namespace Toggl.Networking.ApiClients.Interfaces
{
    public interface IDeletingApiClient<T>
    {
        IObservable<Unit> Delete(T entity);
    }
}
