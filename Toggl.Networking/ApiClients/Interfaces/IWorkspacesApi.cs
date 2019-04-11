using System;
using Toggl.Shared.Models;

namespace Toggl.Networking.ApiClients
{
    public interface IWorkspacesApi
        : IPullingApiClient<IWorkspace>,
          ICreatingApiClient<IWorkspace>
    {
        IObservable<IWorkspace> GetById(long id);
    }
}
