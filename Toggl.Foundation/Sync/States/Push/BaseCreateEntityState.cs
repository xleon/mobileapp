using System;
using System.Reactive.Linq;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave;

namespace Toggl.Foundation.Sync.States
{
    internal abstract class BaseCreateEntityState<TModel>
        where TModel : class, IBaseModel, IDatabaseSyncable
    {
        private readonly ITogglApi api;
        private readonly ITogglDatabase database;

        public StateResult<(Exception, TModel)> CreatingFailed { get; } = new StateResult<(Exception, TModel)>();
        public StateResult<TModel> CreatingFinished { get; } = new StateResult<TModel>();

        public BaseCreateEntityState(ITogglApi api, ITogglDatabase database)
        {
            this.api = api;
            this.database = database;
        }

        public IObservable<ITransition> Start(TModel entity)
            => create(entity)
                .SelectMany(overwrite(entity))
                .Select(CreatingFinished.Transition)
                .Catch(fail(entity));

        private IObservable<TModel> create(TModel entity)
            => entity == null
                ? Observable.Throw<TModel>(new ArgumentNullException(nameof(entity)))
                : Create(api, entity);

        private Func<TModel, IObservable<TModel>> overwrite(TModel entity)
            => createdEntity => GetRepository(database).Update(entity.Id, createdEntity).Select(CopyFrom);

        private Func<Exception, IObservable<ITransition>> fail(TModel entity)
            => e => Observable.Return(CreatingFailed.Transition((e, entity)));

        protected abstract IObservable<TModel> Create(ITogglApi api, TModel entity);

        protected abstract IRepository<TModel> GetRepository(ITogglDatabase database);

        protected abstract TModel CopyFrom(TModel entity);
    }
}
