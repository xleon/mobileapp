using System;
using Toggl.Shared.Models;

namespace Toggl.Ultrawave.ApiClients
{
    public interface IWorkspacesApi
        : IPullingApiClient<IWorkspace>,
          ICreatingApiClient<IWorkspace>
    {
        IObservable<IWorkspace> GetById(long id);
    }
}
