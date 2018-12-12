using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Toggl.Foundation.Extensions;
using Toggl.Multivac.Extensions;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;
using Toggl.Ultrawave;

namespace Toggl.Foundation.Sync.States.Pull
{
    internal sealed class FetchAllSinceState : ISyncState
    {
        private const int sinceDateLimitMonths = 2;
        private const int fetchTimeEntriesForMonths = 2;
        private const int timeEntriesEndDateInclusiveExtraDaysCount = 2;

        private readonly ITogglApi api;
        private readonly ISinceParameterRepository since;
        private readonly ITimeService timeService;
        private readonly ILeakyBucket leakyBucket;
        private readonly IRateLimiter limiter;

        public StateResult<TimeSpan> PreventOverloadingServer { get; } = new StateResult<TimeSpan>();
        public StateResult<IFetchObservables> FetchStarted { get; } = new StateResult<IFetchObservables>();

        public FetchAllSinceState(
            ITogglApi api,
            ISinceParameterRepository since,
            ITimeService timeService,
            ILeakyBucket leakyBucket,
            IRateLimiter limiter)
        {
            this.api = api;
            this.since = since;
            this.timeService = timeService;
            this.leakyBucket = leakyBucket;
            this.limiter = limiter;
        }

        public IObservable<ITransition> Start() => Observable.Create<ITransition>(observer =>
        {
            if (!leakyBucket.TryClaimFreeSlots(
                timeService.CurrentDateTime, numberOfSlots: 9, timeToFreeSlot: out var timeToNextFreeSlot))
            {
                observer.CompleteWith(PreventOverloadingServer.Transition(timeToNextFreeSlot));
                return () => { };
            }

            // The dependencies between the requests are based on the intended order of processing the entities
            // in the sync states. Check the transitions graph and make sure that the requests and states are aligned.
            // We send at most 3 parallel requests to prevent overloading the server with large bursts of requests.

            // first wave
            var workspaces =
                limiter.WaitForFreeSlot()
                    .ThenExecute(() => api.Workspaces.GetAll())
                    .ConnectedReplay();

            var user =
                limiter.WaitForFreeSlot()
                    .ThenExecute(() => api.User.Get())
                    .ConnectedReplay();

            var features =
                limiter.WaitForFreeSlot()
                    .ThenExecute(() => api.WorkspaceFeatures.GetAll())
                    .ConnectedReplay();

            // second wave
            var preferences =
                workspaces.ThenExecute(limiter.WaitForFreeSlot)
                    .ThenExecute(() => api.Preferences.Get())
                    .ConnectedReplay();

            var tags =
                user.ThenExecute(limiter.WaitForFreeSlot)
                    .ThenExecute(() =>
                        sinceOrAll(since.Get<IDatabaseTag>(), api.Tags.GetAll, api.Tags.GetAllSince))
                    .ConnectedReplay();

            var clients =
                features.ThenExecute(limiter.WaitForFreeSlot)
                    .ThenExecute(() =>
                        sinceOrAll(since.Get<IDatabaseClient>(), api.Clients.GetAll, api.Clients.GetAllSince))
                    .ConnectedReplay();

            // third wave
            var projects =
                preferences.ThenExecute(limiter.WaitForFreeSlot)
                    .ThenExecute(() =>
                        sinceOrAll(since.Get<IDatabaseProject>(), api.Projects.GetAll, api.Projects.GetAllSince))
                    .ConnectedReplay();

            var timeEntries =
                tags.ThenExecute(limiter.WaitForFreeSlot)
                    .ThenExecute(() =>
                        sinceOrAll(since.Get<IDatabaseTimeEntry>(), fetchTwoMonthsOfTimeEntries, api.TimeEntries.GetAllSince))
                    .ConnectedReplay();

            var tasks =
                clients.ThenExecute(limiter.WaitForFreeSlot)
                    .ThenExecute(() =>
                        sinceOrAll(since.Get<IDatabaseTask>(), api.Tasks.GetAll, api.Tasks.GetAllSince))
                    .ConnectedReplay();

            var observables = new FetchObservables(
                workspaces, features, user, clients, projects, timeEntries, tags, tasks, preferences);

            observer.CompleteWith(FetchStarted.Transition(observables));
            return () => { };
        });

        private IObservable<List<ITimeEntry>> fetchTwoMonthsOfTimeEntries()
            => api.TimeEntries.GetAll(
                start: timeService.CurrentDateTime.AddMonths(-fetchTimeEntriesForMonths),
                end: timeService.CurrentDateTime.AddDays(timeEntriesEndDateInclusiveExtraDaysCount));

        private IObservable<List<T>> sinceOrAll<T>(
            DateTimeOffset? threshold,
            Func<IObservable<List<T>>> getAll,
            Func<DateTimeOffset, IObservable<List<T>>> getAllSince)
            => threshold.HasValue && isWithinLimit(threshold.Value)
                ? getAllSince(threshold.Value)
                : getAll();

        private bool isWithinLimit(DateTimeOffset threshold)
            => threshold > timeService.CurrentDateTime.AddMonths(-sinceDateLimitMonths);
    }
}
