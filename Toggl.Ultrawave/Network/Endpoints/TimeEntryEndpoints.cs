using System;
namespace Toggl.Ultrawave.Network
{
    internal struct TimeEntryEndpoints
    {
        private readonly Uri baseUrl;

        public TimeEntryEndpoints(Uri baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public Endpoint Get => Endpoint.Get(baseUrl, "me/time_entries");

        public Endpoint Post(int workspaceId)
            => Endpoint.Post(baseUrl, $"workspaces/{ workspaceId }/time_entries");
    }
}
