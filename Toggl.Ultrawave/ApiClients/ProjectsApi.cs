using System;
using System.Collections.Generic;
using Toggl.Ultrawave.Models;
using Toggl.Ultrawave.Network;
using Toggl.Ultrawave.Serialization;

namespace Toggl.Ultrawave.ApiClients
{
    internal sealed class ProjectsApi : BaseApi, IProjectsApi
    {
        private readonly ProjectEndpoints endPoints;

        public ProjectsApi(ProjectEndpoints endPoints, IApiClient apiClient, IJsonSerializer serializer, Credentials credentials)
            : base(apiClient, serializer, credentials)
        {
            this.endPoints = endPoints;
        }

        public IObservable<List<Project>> GetAll()
        {
            var observable = CreateObservable<List<Project>>(endPoints.Get, AuthHeader);
            return observable;
        }

        public IObservable<Project> Create(Project project)
        {
            var endPoint = endPoints.Post(project.WorkspaceId);
            var observable = CreateObservable<Project>(endPoint, AuthHeader, project, SerializationReason.Post);
            return observable;
        }
    }
}
