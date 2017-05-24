using System;
using System.Collections.Generic;
using Toggl.Ultrawave.Network;
using Toggl.Ultrawave.Serialization;

namespace Toggl.Ultrawave.Clients
{
    internal sealed class TimeEntriesClient : BaseClient, ITimeEntriesClient
    {
        private readonly TimeEntryEndpoints endPoints;

        public TimeEntriesClient(TimeEntryEndpoints endPoints, IApiClient apiClient, IJsonSerializer serializer, Credentials credentials)
            : base(apiClient, serializer, credentials)
        {
            this.endPoints = endPoints;
        }

        public IObservable<List<TimeEntry>> GetAll()
        {
            var observable = CreateObservable<List<TimeEntry>>(endPoints.Get, AuthHeader);
            return observable;
        }

        public IObservable<TimeEntry> Create(TimeEntry timeEntry)
        {
            var endPoint = endPoints.Post(timeEntry.WorkspaceId);
            var observable = CreateObservable<TimeEntry>(endPoint, AuthHeader, timeEntry, SerializationReason.Post);
            return observable;
        }
    }
}
