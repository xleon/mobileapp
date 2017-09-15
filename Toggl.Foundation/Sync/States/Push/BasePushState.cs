using System;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Multivac;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;

namespace Toggl.Foundation.Sync.States
{
    internal abstract class BasePushState<TModel>
        where TModel : class, IBaseModel, IDatabaseSyncable
    {
        private readonly ITogglDatabase database;

        public StateResult<TModel> PushEntity { get; } = new StateResult<TModel>();
        public StateResult NothingToPush { get; } = new StateResult();

        public BasePushState(ITogglDatabase database)
        {
            Ensure.Argument.IsNotNull(database, nameof(database));
        
            this.database = database;
        }

        public IObservable<ITransition> Start() =>
            getOldestUnsynced()
                .SingleAsync()
                .Select(entity =>
                    entity != null
                        ? (ITransition)PushEntity.Transition(CopyFrom(entity))
                        : NothingToPush.Transition());

        private IObservable<TModel> getOldestUnsynced()
            => GetRepository(database)
                .GetAll(syncNeeded)
                .SingleAsync()
                .Select(entities => entities
                    .OrderBy(LastChange)
                    .FirstOrDefault());

        protected abstract IRepository<TModel> GetRepository(ITogglDatabase database);

        protected abstract DateTimeOffset LastChange(TModel entity);

        private bool syncNeeded(TModel entity)
            => entity.SyncStatus == SyncStatus.SyncNeeded;
            
        protected abstract TModel CopyFrom(TModel entity);
    }
}
