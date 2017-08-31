using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Sync.States
{
    internal abstract class BasePersistState<TInterface, TDatabaseInterface>
        where TDatabaseInterface : TInterface
    {
        private readonly ITogglDatabase database;

        public StateResult<FetchObservables> FinishedPersisting { get; } = new StateResult<FetchObservables>();

        protected BasePersistState(ITogglDatabase database)
        {
            this.database = database;
        }

        public IObservable<ITransition> Start(FetchObservables fetch)
        {
            var since = fetch.SinceParameters;
            return FetchObservable(fetch)
                .SingleAsync()
                .Select(entities => entities.Select(ConvertToDatabaseEntity).ToList())
                .Do(databaseEntities => BatchUpdate(database, databaseEntities))
                .Select(databaseEntities => LastUpdated(since, databaseEntities))
                .Select(lastUpdated => UpdateSinceParameters(since, lastUpdated))
                .Do(database.SinceParameters.Set)
                .Select(sinceParameters => new FetchObservables(fetch, sinceParameters))
                .Select(FinishedPersisting.Transition);
        }

        protected abstract IObservable<IEnumerable<TInterface>> FetchObservable(FetchObservables fetch);

        protected abstract TDatabaseInterface ConvertToDatabaseEntity(TInterface entity);

        protected abstract IObservable<IEnumerable<TDatabaseInterface>> BatchUpdate(ITogglDatabase database, IEnumerable<TDatabaseInterface> entities);

        protected abstract DateTimeOffset? LastUpdated(ISinceParameters old, IEnumerable<TDatabaseInterface> entities);

        protected abstract ISinceParameters UpdateSinceParameters(ISinceParameters old, DateTimeOffset? lastUpdated);
    }
}
