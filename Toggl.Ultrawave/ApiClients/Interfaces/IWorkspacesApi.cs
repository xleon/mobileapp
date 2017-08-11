using System;
using System.Collections.Generic;
using Toggl.Multivac.Models;

namespace Toggl.Ultrawave.ApiClients
{
    public interface IWorkspacesApi
    {
        IObservable<List<IWorkspace>> GetAll();
        IObservable<IWorkspace> GetById(long id);
    }
}
