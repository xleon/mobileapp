using System;
using System.Collections.Generic;

namespace Toggl.Ultrawave.Clients
{
    public interface IWorkspacesClient
    {
        IObservable<List<Workspace>> GetAll();
        IObservable<Workspace> GetById(int id);
    }
}
