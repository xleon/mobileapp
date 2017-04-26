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
    }
}
