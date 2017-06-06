using System;
using System.Collections.Generic;
using Toggl.Ultrawave.Models;

namespace Toggl.Ultrawave.ApiClients
{
    public interface IWorkspacesApi
    {
        IObservable<List<Workspace>> GetAll();
        IObservable<Workspace> GetById(int id);
    }
}
