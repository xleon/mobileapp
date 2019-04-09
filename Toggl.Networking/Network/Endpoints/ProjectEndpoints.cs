using System;
using Toggl.Shared.Extensions;

namespace Toggl.Networking.Network
{
    internal struct ProjectEndpoints
    {
        private readonly Uri baseUrl;

        public ProjectEndpoints(Uri baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public Endpoint Get => Endpoint.Get(baseUrl, "me/projects");

        public Endpoint GetSince(DateTimeOffset threshold)
            => Endpoint.Get(baseUrl, $"me/projects?since={threshold.ToUnixTimeSeconds()}");

        public Endpoint Post(long workspaceId)
            => Endpoint.Post(baseUrl, $"workspaces/{workspaceId}/projects");
    }
}
