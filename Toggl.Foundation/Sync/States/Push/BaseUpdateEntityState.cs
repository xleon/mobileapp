using System;
using System.Reactive.Linq;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave;
using Toggl.Ultrawave.Exceptions;

namespace Toggl.Foundation.Sync.States
{
    internal abstract class BaseUpdateEntityState<TModel>
        where TModel : class, IBaseModel, IDatabaseSyncable
    {
        private readonly ITogglApi api;
        private readonly IRepository<TModel> repository;

        public StateResult<(Exception, TModel)> ClientError { get; } = new StateResult<(Exception, TModel)>();
        public StateResult<(Exception, TModel)> ServerError { get; } = new StateResult<(Exception, TModel)>();
        public StateResult<(Exception, TModel)> UnknownError { get; } = new StateResult<(Exception, TModel)>();
        public StateResult<TModel> EntityChanged { get; } = new StateResult<TModel>();
        public StateResult<TModel> UpdatingSucceeded { get; } = new StateResult<TModel>();

        public BaseUpdateEntityState(ITogglApi api, IRepository<TModel> repository)
        {
            this.api = api;
            this.repository = repository;
        }

        public IObservable<ITransition> Start(TModel entity)
            => update(entity)
                .SelectMany(tryOverwrite(entity))
                .SelectMany(result => result.Mode == ConflictResolutionMode.Ignore
                    ? entityChanged(entity)
                    : succeeded(CopyFrom(result.UpdatedEntity)))
                .Catch(fail(entity));

        private IObservable<TModel> update(TModel entity)
            => entity == null
                ? Observable.Throw<TModel>(new ArgumentNullException(nameof(entity)))
                : Update(api, entity);

        private IObservable<ITransition> entityChanged(TModel entity)
            => Observable.Return(EntityChanged.Transition(entity));

        private Func<TModel, IObservable<(ConflictResolutionMode Mode, TModel UpdatedEntity)>> tryOverwrite(TModel entity)
            => updatedEntity => repository.UpdateWithConflictResolution(entity.Id, updatedEntity, overwriteIfLocalEntityDidNotChange(entity));

        private Func<TModel, TModel, ConflictResolutionMode> overwriteIfLocalEntityDidNotChange(TModel local)
            => (currentLocal, _) => HasChanged(local, currentLocal)
                ? ConflictResolutionMode.Ignore
                : ConflictResolutionMode.Update;

        private Func<Exception, IObservable<ITransition>> fail(TModel entity)
            => exception =>
                exception is ServerErrorException
                    ? Observable.Return(ServerError.Transition((exception, entity)))
                    : exception is ClientErrorException
                        ? Observable.Return(ClientError.Transition((exception, entity)))
                        : Observable.Return(UnknownError.Transition((exception, entity)));

        private IObservable<ITransition> succeeded(TModel entity)
            => Observable.Return((ITransition)UpdatingSucceeded.Transition(entity));

        protected abstract bool HasChanged(TModel original, TModel updated);

        protected abstract IObservable<TModel> Update(ITogglApi api, TModel entity);

        protected abstract TModel CopyFrom(TModel entity);
    }
}
