using System;
using System.Reactive.Linq;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave;
using Toggl.Ultrawave.Exceptions;

namespace Toggl.Foundation.Sync.States
{
    public abstract class BasePushEntityState<TModel>
        where TModel : class, IBaseModel, IDatabaseSyncable
    {
        public StateResult<(Exception, TModel)> ServerError { get; } = new StateResult<(Exception, TModel)>();
        public StateResult<(Exception, TModel)> ClientError { get; } = new StateResult<(Exception, TModel)>();
        public StateResult<(Exception, TModel)> UnknownError { get; } = new StateResult<(Exception, TModel)>();

        protected ITogglApi Api { get; }
        protected IRepository<TModel> Repository { get; }

        public BasePushEntityState(ITogglApi api, IRepository<TModel> repository)
        {
            Api = api;
            Repository = repository;
        }

        protected Func<TModel, IObservable<TModel>> Overwrite(TModel entity)
            => pushedEntity => Repository.Update(entity.Id, pushedEntity).Select(CopyFrom);

        protected Func<Exception, IObservable<ITransition>> Fail(TModel entity)
            => exception =>
                exception is ServerErrorException
                    ? Observable.Return(ServerError.Transition((exception, entity)))
                    : exception is ClientErrorException
                        ? Observable.Return(ClientError.Transition((exception, entity)))
                        : Observable.Return(UnknownError.Transition((exception, entity)));

        public abstract IObservable<ITransition> Start(TModel entity);

        protected abstract TModel CopyFrom(TModel entity);
    }
}
