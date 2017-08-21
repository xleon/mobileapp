using System;
using Toggl.Multivac.Extensions;

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

        public Endpoint GetSince(DateTimeOffset threshold)
            => Endpoint.Get(baseUrl, $"me/time_entries?since={threshold.ToUnixTimeSeconds()}");

        public Endpoint Post(long workspaceId)
            => Endpoint.Post(baseUrl, $"workspaces/{workspaceId}/time_entries");
    }
}
