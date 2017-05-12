using System;
using System.Collections.Generic;

namespace Toggl.Ultrawave.Clients
{
    public interface IWorkspacesClient
    {
        IObservable<List<Workspace>> GetAll(string username, string password);
        IObservable<Workspace> GetById(string username, string password, int id);
    }
}
