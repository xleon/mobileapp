using System;
using System.Collections.Generic;
using Toggl.Multivac.Models;

namespace Toggl.Ultrawave.ApiClients
{
    public interface IClientsApi
    {
        IObservable<List<IClient>> GetAll();
        IObservable<IClient> Create(IClient client);
    }
}
