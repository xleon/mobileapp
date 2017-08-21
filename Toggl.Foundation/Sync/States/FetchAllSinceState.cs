using System;
using System.Reactive.Linq;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave;

namespace Toggl.Foundation.Sync.States
{
    internal sealed class FetchAllSinceState
    {
        private readonly ITogglDatabase database;
        private readonly ITogglApi api;

        public StateResult<FetchObservables> FetchStarted { get; } = new StateResult<FetchObservables>();

        public FetchAllSinceState(ITogglDatabase database, ITogglApi api)
        {
            this.database = database;
            this.api = api;
        }

        public IObservable<ITransition> Start() => Observable.Create<ITransition>(observer =>
        {
            var sinceDates = database.SinceParameters.Get();

            var observables = new FetchObservables(sinceDates,
                connectedReplayed(api.Workspaces.GetAll()),
                connectedReplayed(getSinceOrAll(sinceDates.Clients, api.Clients.GetAllSince, api.Clients.GetAll)),
                connectedReplayed(getSinceOrAll(sinceDates.Projects, api.Projects.GetAllSince, api.Projects.GetAll)),
                connectedReplayed(getSinceOrAll(sinceDates.TimeEntries, api.TimeEntries.GetAllSince, api.TimeEntries.GetAll))
            );

            observer.OnNext(FetchStarted.Transition(observables));
            observer.OnCompleted();

            return () => { };
        });

        private static IObservable<T> connectedReplayed<T>(IObservable<T> observable)
        {
            var replayed = observable.Replay();
            replayed.Connect();
            return replayed;
        }

        private static IObservable<T> getSinceOrAll<T>(DateTimeOffset? threshold,
            Func<DateTimeOffset, IObservable<T>> since, Func<IObservable<T>> all)
            => threshold.HasValue ? since(threshold.Value) : all();
    }
}
