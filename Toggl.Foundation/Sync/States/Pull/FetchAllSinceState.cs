using System;
using System.Reactive.Linq;
using Toggl.Multivac.Extensions;
using Toggl.Foundation.Models;
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
            var databaseSinceDates = database.SinceParameters.Get();
            var sinceDates = new SinceParameters(databaseSinceDates);

            var observables = new FetchObservables(sinceDates,
                api.Workspaces.GetAll().ConnectedReplay(),
                api.WorkspaceFeatures.GetAll().ConnectedReplay(),
                getSinceOrAll(sinceDates.Clients, api.Clients.GetAllSince, api.Clients.GetAll).ConnectedReplay(),
                getSinceOrAll(sinceDates.Projects, api.Projects.GetAllSince, api.Projects.GetAll).ConnectedReplay(),
                getSinceOrAll(sinceDates.TimeEntries, api.TimeEntries.GetAllSince, api.TimeEntries.GetAll).ConnectedReplay(),
                getSinceOrAll(sinceDates.Tags, api.Tags.GetAllSince, api.Tags.GetAll).ConnectedReplay()
            );

            observer.OnNext(FetchStarted.Transition(observables));
            observer.OnCompleted();

            return () => { };
        });

        private static IObservable<T> getSinceOrAll<T>(DateTimeOffset? threshold,
            Func<DateTimeOffset, IObservable<T>> since, Func<IObservable<T>> all)
            => threshold.HasValue ? since(threshold.Value) : all();
    }
}
