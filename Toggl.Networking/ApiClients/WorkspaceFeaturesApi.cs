using System;
using System.Collections.Generic;
using Toggl.Networking.Models;
using Toggl.Networking.Network;
using Toggl.Networking.Serialization;
using Toggl.Shared.Models;

namespace Toggl.Networking.ApiClients
{
    internal sealed class WorkspaceFeaturesApi : BaseApi, IWorkspaceFeaturesApi
    {
        private readonly WorkspaceFeaturesEndpoints endPoints;

        public WorkspaceFeaturesApi(Endpoints endPoints, IApiClient apiClient, IJsonSerializer serializer,
            Credentials credentials)
            : base(apiClient, serializer, credentials, endPoints.LoggedIn)
        {
            this.endPoints = endPoints.WorkspaceFeatures;
        }

        public IObservable<List<IWorkspaceFeatureCollection>> GetAll()
            => SendRequest<WorkspaceFeatureCollection, IWorkspaceFeatureCollection>(endPoints.Get, AuthHeader);

    }
}
