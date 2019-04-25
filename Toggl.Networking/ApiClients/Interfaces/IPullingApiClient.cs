using System;
using System.Collections.Generic;

namespace Toggl.Networking.ApiClients
{
    public interface IPullingApiClient<T>
    {
        IObservable<List<T>> GetAll();
    }
}
