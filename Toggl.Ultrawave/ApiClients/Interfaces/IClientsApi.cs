using System;
using System.Collections.Generic;
using Toggl.Ultrawave.Models;

namespace Toggl.Ultrawave.ApiClients
{
    public interface IClientsApi
    {
        IObservable<List<Client>> GetAll();
        IObservable<Client> Create(Client client);
    }
}
