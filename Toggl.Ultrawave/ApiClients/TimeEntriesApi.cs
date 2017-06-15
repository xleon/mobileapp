using System;
using System.Collections.Generic;
using Toggl.Ultrawave.Models;
using Toggl.Ultrawave.Network;
using Toggl.Ultrawave.Serialization;

namespace Toggl.Ultrawave.ApiClients
{
    internal sealed class TimeEntriesApi : BaseApi, ITimeEntriesApi
    {
        private readonly TimeEntryEndpoints endPoints;

        public TimeEntriesApi(TimeEntryEndpoints endPoints, IApiClient apiClient, IJsonSerializer serializer, Credentials credentials)
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
            var observable = CreateObservable(endPoint, AuthHeader, timeEntry, SerializationReason.Post);
            return observable;
        }
    }
}
