using System;
using System.Collections.Generic;

namespace Toggl.Ultrawave.Clients
{
    public interface IClientsClient
    {
        IObservable<List<Client>> GetAll();
        IObservable<Client> Create(Client client);
    }
}
