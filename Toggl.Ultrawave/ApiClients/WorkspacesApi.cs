using System;
using System.Collections.Generic;
using Toggl.Ultrawave.Network;
using Toggl.Ultrawave.Serialization;

namespace Toggl.Ultrawave.ApiClients
{
    internal sealed class WorkspacesApi : BaseApi, IWorkspacesApi
    {
        private readonly WorkspaceEndpoints endPoints;

        public WorkspacesApi(WorkspaceEndpoints endPoints, IApiClient apiClient, IJsonSerializer serializer,
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
