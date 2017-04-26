using System;

namespace Toggl.Ultrawave.Network
{
    internal struct TagEndpoints
    {
        private readonly Uri baseUrl;

        public TagEndpoints(Uri baseUrl)
        {
        	this.baseUrl = baseUrl;
        }

        public Endpoint Get => Endpoint.Get(baseUrl, "me/tags");
    }
}
