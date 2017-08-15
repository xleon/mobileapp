using System;
using System.Collections.Generic;
using Toggl.Multivac.Models;

namespace Toggl.Ultrawave.ApiClients
{
    public interface IClientsApi
    {
        IObservable<List<IClient>> GetAll();
        IObservable<List<IClient>> GetAllSince(DateTimeOffset threshold);
        IObservable<IClient> Create(IClient client);
    }
}
