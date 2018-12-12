using System;
using System.Collections.Generic;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave.ApiClients;

namespace Toggl.Foundation.Sync.Helpers
{
    public interface IRequestSender
    {
        IObservable<T> Fetch<T, TApi>(TApi api)
            where TApi : IPullingSingleApiClient<T>;

        IObservable<List<T>> FetchAll<T, TApi>(TApi api)
            where TApi : IPullingApiClient<T>;

        IObservable<List<T>> FetchAllSinceIfPossible<T, TDatabase, TApi>(TApi api)
            where TDatabase : T, IDatabaseSyncable
            where TApi : IPullingApiClient<T>, IPullingChangedApiClient<T>;

        IObservable<List<ITimeEntry>> FetchTimeEntries(ITimeEntriesApi api);
    }
}
