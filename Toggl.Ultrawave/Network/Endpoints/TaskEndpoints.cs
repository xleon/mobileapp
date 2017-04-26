using System;

namespace Toggl.Ultrawave.Network
{
    internal struct TaskEndpoints
    {
        private readonly Uri baseUrl;

        public TaskEndpoints(Uri baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public Endpoint Get => Endpoint.Get(baseUrl, "me/tasks");
    }
}
