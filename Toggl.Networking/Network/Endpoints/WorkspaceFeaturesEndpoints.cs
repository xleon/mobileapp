using System;

namespace Toggl.Ultrawave.Network
{
    internal struct WorkspaceFeaturesEndpoints
    {
        private readonly Uri baseUrl;

        public WorkspaceFeaturesEndpoints(Uri baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public Endpoint Get => Endpoint.Get(baseUrl, "me/features");
    }
}
