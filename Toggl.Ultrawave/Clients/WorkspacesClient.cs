using System;
using System.Collections.Generic;
using Toggl.Ultrawave.Network;
using Toggl.Ultrawave.Serialization;

namespace Toggl.Ultrawave.Clients
{
    internal sealed class WorkspacesClient : BaseClient, IWorkspacesClient
    {
        private readonly WorkspaceEndpoints endPoints;

        public WorkspacesClient(WorkspaceEndpoints endPoints, IApiClient apiClient, IJsonSerializer serializer,
            Credentials credentials)
            : base(apiClient, serializer, credentials)
        {
            this.endPoints = endPoints;
        }

        public IObservable<List<Workspace>> GetAll()
        {
            var observable = CreateObservable<List<Workspace>>(endPoints.Get, AuthHeader);
            return observable;
        }

        public IObservable<Workspace> GetById(int id)
        {
            var endpoint = endPoints.GetById(id);
            var observable = CreateObservable<Workspace>(endpoint, AuthHeader);
            return observable;
        }
    }
}
