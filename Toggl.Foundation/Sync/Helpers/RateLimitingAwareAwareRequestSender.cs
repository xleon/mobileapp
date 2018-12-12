using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave.ApiClients;

namespace Toggl.Foundation.Sync.Helpers
{
    internal sealed class RateLimitingAwareRequestSender : IRequestSender
    {
        private readonly ITimeService timeService;
        private readonly ILeakyBucket leakyBucket;
        private readonly IScheduler scheduler;
        private readonly IRequestSender requestSender;

        public RateLimitingAwareRequestSender(
            ITimeService timeService,
            ILeakyBucket leakyBucket,
            IScheduler scheduler,
            IRequestSender requestSender)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(leakyBucket, nameof(leakyBucket));
            Ensure.Argument.IsNotNull(scheduler, nameof(scheduler));
            Ensure.Argument.IsNotNull(requestSender, nameof(requestSender));

            this.timeService = timeService;
            this.leakyBucket = leakyBucket;
            this.scheduler = scheduler;
            this.requestSender = requestSender;
        }

        public IObservable<T> Fetch<T, TApi>(TApi api)
            where TApi : IPullingSingleApiClient<T>
        {
            return waitUntilFreeSlotIsAvailable()
                .SelectMany(_ => requestSender.Fetch<T, TApi>(api))
                .ConnectedReplay();
        }

        public IObservable<List<T>> FetchAll<T, TApi>(TApi api)
            where TApi : IPullingApiClient<T>
        {
            return waitUntilFreeSlotIsAvailable()
                .SelectMany(_ => requestSender.FetchAll<T, TApi>(api))
                .ConnectedReplay();
        }

        public IObservable<List<T>> FetchAllSinceIfPossible<T, TDatabase, TApi>(TApi api)
            where TDatabase : T, IDatabaseSyncable
            where TApi : IPullingApiClient<T>, IPullingChangedApiClient<T>
        {
            return waitUntilFreeSlotIsAvailable()
                .SelectMany(_ => requestSender.FetchAllSinceIfPossible<T, TDatabase, TApi>(api))
                .ConnectedReplay();
        }

        public IObservable<List<ITimeEntry>> FetchTimeEntries(ITimeEntriesApi api)
        {
            return waitUntilFreeSlotIsAvailable()
                .SelectMany(_ => requestSender.FetchTimeEntries(api))
                .ConnectedReplay();
        }

        private IObservable<Unit> waitUntilFreeSlotIsAvailable()
            => leakyBucket.TryClaimFreeSlot(timeService.CurrentDateTime, out var timeToFreeSlot)
                ? Observable.Return(Unit.Default)
                : Observable.Return(Unit.Default)
                    .Delay(timeToFreeSlot, scheduler)
                    .SelectMany(_ => waitUntilFreeSlotIsAvailable());
    }
}
