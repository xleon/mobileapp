using System;
using System.Collections.Generic;
using Toggl.Foundation.Models.Interfaces;

namespace Toggl.Foundation.Interactors
{
    public sealed partial class InteractorFactory
    {
        public IInteractor<IObservable<IEnumerable<IThreadSafeClient>>> GetAllClientsInWorkspace(long workspaceId)
            => new GetAllClientsInWorkspaceInteractor(dataSource.Clients, workspaceId);

        public IInteractor<IObservable<IThreadSafeClient>> CreateClient(string clientName, long workspaceId)
            => new CreateClientInteractor(idProvider, timeService, dataSource.Clients, clientName, workspaceId);
    }
}
