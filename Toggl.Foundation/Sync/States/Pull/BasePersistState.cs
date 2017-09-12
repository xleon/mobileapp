using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Sync.States
{
    internal abstract class BasePersistState<TInterface, TDatabaseInterface>
        where TDatabaseInterface : TInterface, IBaseModel
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
                .Select(entities => entities ?? new List<TInterface>())
                .Select(entities => entities.Select(ConvertToDatabaseEntity).ToList())
                .SelectMany(databaseEntities => 
                    BatchUpdate(database, databaseEntities.Select(entity => (entity.Id, entity)))
                        .Select(results => results.Select(result => result.Item2))
                        .IgnoreElements()
                        .Concat(Observable.Return(databaseEntities)))
                .Select(databaseEntities => LastUpdated(since, databaseEntities))
                .Select(lastUpdated => UpdateSinceParameters(since, lastUpdated))
                .Do(database.SinceParameters.Set)
                .Select(sinceParameters => new FetchObservables(fetch, sinceParameters))
                .Select(FinishedPersisting.Transition);
        }

        protected abstract IObservable<IEnumerable<TInterface>> FetchObservable(FetchObservables fetch);

        protected abstract TDatabaseInterface ConvertToDatabaseEntity(TInterface entity);

        protected abstract IObservable<IEnumerable<(ConflictResolutionMode, TDatabaseInterface)>> BatchUpdate(ITogglDatabase database, IEnumerable<(long, TDatabaseInterface)> entities);

        protected abstract DateTimeOffset? LastUpdated(ISinceParameters old, IEnumerable<TDatabaseInterface> entities);

        protected abstract ISinceParameters UpdateSinceParameters(ISinceParameters old, DateTimeOffset? lastUpdated);
    }
}
