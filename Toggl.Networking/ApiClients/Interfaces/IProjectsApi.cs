using System;
using System.Collections.Generic;
using Toggl.Shared.Models;

namespace Toggl.Networking.ApiClients
{
    public interface IProjectsApi
        : IPullingApiClient<IProject>,
          IPullingChangedApiClient<IProject>,
          ICreatingApiClient<IProject>
    {
        IObservable<List<IProject>> Search(long workspaceId, long[] projectIds);
    }
}
