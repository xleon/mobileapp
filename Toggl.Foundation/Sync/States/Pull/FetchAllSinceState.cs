using System;
using System.Reactive.Linq;
using Toggl.Foundation.Sync.Helpers;
using Toggl.Multivac.Extensions;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant.Models;
using Toggl.Ultrawave;
using Toggl.Ultrawave.ApiClients;

namespace Toggl.Foundation.Sync.States.Pull
{
    internal sealed class FetchAllSinceState : ISyncState
    {
        private readonly IRequestSender sender;
        private readonly ITogglApi api;
        private readonly ITimeService timeService;
        private readonly ILeakyBucket leakyBucket;

        public StateResult<TimeSpan> PreventOverloadingServer { get; } = new StateResult<TimeSpan>();
        public StateResult<IFetchObservables> FetchStarted { get; } = new StateResult<IFetchObservables>();

        public FetchAllSinceState(
            ITogglApi api,
            ITimeService timeService,
            IRequestSender sender,
            ILeakyBucket leakyBucket)
        {
            this.api = api;
            this.timeService = timeService;
            this.sender = sender;
            this.leakyBucket = leakyBucket;
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
            var workspaces = sender.FetchAll<IWorkspace, IWorkspacesApi>(api.Workspaces);
            var user = sender.Fetch<IUser, IUserApi>(api.User);
            var features = sender.FetchAll<IWorkspaceFeatureCollection, IWorkspaceFeaturesApi>(api.WorkspaceFeatures);

            // second wave
            var preferences = followUp(workspaces, () => sender.Fetch<IPreferences, IPreferencesApi>(api.Preferences));
            var tags = followUp(user, () => sender.FetchAllSinceIfPossible<ITag, IDatabaseTag, ITagsApi>(api.Tags));
            var clients = followUp(features, () => sender.FetchAllSinceIfPossible<IClient, IDatabaseClient, IClientsApi>(api.Clients));

            // third wave
            var projects = followUp(preferences, () => sender.FetchAllSinceIfPossible<IProject, IDatabaseProject, IProjectsApi>(api.Projects));
            var timeEntries = followUp(tags, () => sender.FetchTimeEntries(api.TimeEntries));
            var tasks = followUp(clients, () => sender.FetchAllSinceIfPossible<ITask, IDatabaseTask, ITasksApi>(api.Tasks));

            var observables = new FetchObservables(
                workspaces, features, user, clients, projects, timeEntries, tags, tasks, preferences);

            observer.CompleteWith(FetchStarted.Transition(observables));
            return () => { };
        });

        private static IObservable<T2> followUp<T1, T2>(IObservable<T1> observable, Func<IObservable<T2>> continuation)
            => observable.SingleAsync().SelectMany(_ => continuation()).ConnectedReplay();
    }
}
