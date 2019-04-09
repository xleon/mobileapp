using System;
using System.Collections.Generic;
using Toggl.Shared.Models;
using Toggl.Networking.ApiClients.Interfaces;

namespace Toggl.Networking.ApiClients
{
    public interface ITimeEntriesApi
        : IDeletingApiClient<ITimeEntry>,
          ICreatingApiClient<ITimeEntry>,
          IUpdatingApiClient<ITimeEntry>,
          IPullingApiClient<ITimeEntry>,
          IPullingChangedApiClient<ITimeEntry>
    {
        IObservable<List<ITimeEntry>> GetAll(DateTimeOffset start, DateTimeOffset end);
    }
}
