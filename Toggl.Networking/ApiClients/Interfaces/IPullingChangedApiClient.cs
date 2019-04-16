using System;
using System.Collections.Generic;

namespace Toggl.Networking.ApiClients
{
    public interface IPullingChangedApiClient<T>
    {
        IObservable<List<T>> GetAllSince(DateTimeOffset threshold);
    }
}
