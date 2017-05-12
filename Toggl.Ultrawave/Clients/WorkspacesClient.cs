using System;
using System.Collections.Generic;
using Toggl.Ultrawave.Network;
using Toggl.Ultrawave.Serialization;

namespace Toggl.Ultrawave.Clients
{
    internal sealed class WorkspacesClient : BaseClient, IWorkspacesClient
    {
        private readonly WorkspaceEndpoints endPoints;

        public WorkspacesClient(WorkspaceEndpoints endPoints, IApiClient apiClient, IJsonSerializer serializer)
            : base(apiClient, serializer)
        {
            this.endPoints = endPoints;
        }

        public IObservable<List<Workspace>> GetAll(string username, string password)
        {
            var header = GetAuthHeader(username, password);
            var observable = CreateObservable<List<Workspace>>(endPoints.Get, header);
            return observable;
        }

        public IObservable<Workspace> GetById(string username, string password, int id)
        {
            var header = GetAuthHeader(username, password);
            var endpoint = endPoints.GetById(id);
            var observable = CreateObservable<Workspace>(endpoint, header);
            return observable;
        }
    }
}
