using System;

namespace Toggl.Ultrawave.Network
{
    internal struct WorkspaceEndpoints
    {
        private readonly Uri baseUrl;

        public WorkspaceEndpoints(Uri baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public Endpoint Get => Endpoint.Get(baseUrl, "me/workspaces");

        public Endpoint GetById(long id) => Endpoint.Get(baseUrl, $"workspaces/{id}");
    }
}
