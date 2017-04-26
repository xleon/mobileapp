using System;
using Toggl.Ultrawave.Network;

namespace Toggl.Ultrawave
{
    internal struct ClientEndpoints
    {
        private readonly Uri baseUrl;

        public ClientEndpoints(Uri baseUrl)
        {
        	this.baseUrl = baseUrl;
        }

        public Endpoint Get => Endpoint.Get(baseUrl, "me/clients");

        public Endpoint Post(int workspaceId)
            => Endpoint.Post(baseUrl, $"workspaces/{ workspaceId }/clients");
    }
}
