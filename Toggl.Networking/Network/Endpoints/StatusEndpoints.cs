using System;

namespace Toggl.Ultrawave.Network
{
    internal struct StatusEndpoints
    {
        private readonly Uri baseUrl;

        public StatusEndpoints(Uri baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public Endpoint Get => Endpoint.Get(baseUrl, "status");
    }
}
