using System;
using System.Collections.Generic;

namespace Toggl.Ultrawave.ApiClients
{
    public interface IClientsApi
    {
        IObservable<List<Client>> GetAll();
        IObservable<Client> Create(Client client);
    }
}
