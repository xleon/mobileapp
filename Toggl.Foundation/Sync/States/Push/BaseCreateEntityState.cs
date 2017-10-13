using System;
using System.Reactive.Linq;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave;
using Toggl.Ultrawave.Exceptions;

namespace Toggl.Foundation.Sync.States
{
    internal abstract class BaseCreateEntityState<TModel>
        where TModel : class, IBaseModel, IDatabaseSyncable
    {
        private readonly ITogglApi api;
        private readonly IRepository<TModel> repository;

        public StateResult<(Exception, TModel)> ServerError { get; } = new StateResult<(Exception, TModel)>();
        public StateResult<(Exception, TModel)> ClientError { get; } = new StateResult<(Exception, TModel)>();
        public StateResult<(Exception, TModel)> UnknownError { get; } = new StateResult<(Exception, TModel)>();
        public StateResult<TModel> CreatingFinished { get; } = new StateResult<TModel>();

        public BaseCreateEntityState(ITogglApi api, IRepository<TModel> repository)
        {
            this.api = api;
            this.repository = repository;
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
            => createdEntity => repository.Update(entity.Id, createdEntity).Select(CopyFrom);

        private Func<Exception, IObservable<ITransition>> fail(TModel entity)
            => exception =>
                exception is ServerErrorException
                    ? Observable.Return(ServerError.Transition((exception, entity)))
                    : exception is ClientErrorException
                        ? Observable.Return(ClientError.Transition((exception, entity)))
                        : Observable.Return(UnknownError.Transition((exception, entity)));

        protected abstract IObservable<TModel> Create(ITogglApi api, TModel entity);

        protected abstract TModel CopyFrom(TModel entity);
    }
}
