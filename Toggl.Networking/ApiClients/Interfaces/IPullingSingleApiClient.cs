using System;

namespace Toggl.Networking.ApiClients
{
    public interface IPullingSingleApiClient<T>
    {
        IObservable<T> Get();
    }
}
