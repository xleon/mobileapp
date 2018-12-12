using System;
using System.Collections.Generic;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;
using Toggl.Ultrawave.ApiClients;

namespace Toggl.Foundation.Sync.Helpers
{
    internal sealed class RequestSender : IRequestSender
    {
        private const int sinceDateLimitMonths = 2;
        private const int fetchTimeEntriesForMonths = 2;
        private const int timeEntriesEndDateInclusiveExtraDaysCount = 2;

        private readonly ITogglDatabase database;
        private readonly ITimeService timeService;

        public RequestSender(
            ITogglDatabase database,
            ITimeService timeService)
        {
            Ensure.Argument.IsNotNull(database, nameof(database));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.database = database;
            this.timeService = timeService;
        }

        public IObservable<T> Fetch<T, TApi>(TApi api)
            where TApi : IPullingSingleApiClient<T>
            => api.Get().ConnectedReplay();

        public IObservable<List<T>> FetchAll<T, TApi>(TApi api)
            where TApi : IPullingApiClient<T>
            => api.GetAll().ConnectedReplay();

        public IObservable<List<T>> FetchAllSinceIfPossible<T, TDatabase, TApi>(TApi api)
            where TDatabase : T, IDatabaseSyncable
            where TApi : IPullingApiClient<T>, IPullingChangedApiClient<T>
        {
            var since = database.SinceParameters.Get<TDatabase>();
            var observableRequest = since.HasValue && isWithinLimit(since.Value)
                ? api.GetAllSince(since.Value) : api.GetAll();

            return observableRequest.ConnectedReplay();
        }

        public IObservable<List<ITimeEntry>> FetchTimeEntries(ITimeEntriesApi api)
        {
            var since = database.SinceParameters.Get<IDatabaseTimeEntry>();
            var observableRequest = since.HasValue && isWithinLimit(since.Value)
                ? api.GetAllSince(since.Value)
                : api.GetAll(
                    start: timeService.CurrentDateTime.AddMonths(-fetchTimeEntriesForMonths),
                    end: timeService.CurrentDateTime.AddDays(timeEntriesEndDateInclusiveExtraDaysCount));

            return observableRequest.ConnectedReplay();
        }

        private bool isWithinLimit(DateTimeOffset threshold)
            => threshold > timeService.CurrentDateTime.AddMonths(-sinceDateLimitMonths);
    }
}
