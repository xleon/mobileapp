using System;
using System.Reactive.Linq;
using Toggl.Foundation.DataSources.Interfaces;
using Toggl.Foundation.Models.Interfaces;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;
using Toggl.Ultrawave.ApiClients;

namespace Toggl.Foundation.Sync.States.Push
{
    internal sealed class CreateEntityState<TModel, TDatabaseModel, TThreadsafeModel>
        : BasePushEntityState<TThreadsafeModel>
        where TModel : IIdentifiable
        where TDatabaseModel : TModel, IDatabaseModel
        where TThreadsafeModel : class, TDatabaseModel, IThreadSafeModel
    {
        private readonly ICreatingApiClient<TModel> api;

        private readonly IDataSource<TThreadsafeModel, TDatabaseModel> dataSource;

        private readonly Func<TModel, TThreadsafeModel> convertToThreadsafeModel;

        public StateResult<TThreadsafeModel> EntityChanged { get; } = new StateResult<TThreadsafeModel>();

        public StateResult<TThreadsafeModel> Finished { get; } = new StateResult<TThreadsafeModel>();

        public CreateEntityState(
            ICreatingApiClient<TModel> api,
            IDataSource<TThreadsafeModel, TDatabaseModel> dataSource,
            Func<TModel, TThreadsafeModel> convertToThreadsafeModel)
        {
            Ensure.Argument.IsNotNull(api, nameof(api));
            Ensure.Argument.IsNotNull(convertToThreadsafeModel, nameof(convertToThreadsafeModel));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));

            this.api = api;
            this.convertToThreadsafeModel = convertToThreadsafeModel;
            this.dataSource = dataSource;
        }

        public override IObservable<ITransition> Start(TThreadsafeModel entity)
            => create(entity)
                .Select(convertToThreadsafeModel)
                .SelectMany(tryOverwrite(entity))
                .Catch(Fail(entity));

        private IObservable<TModel> create(TThreadsafeModel entity)
            => entity == null
                ? Observable.Throw<TModel>(new ArgumentNullException(nameof(entity)))
                : api.Create(entity);

        private Func<TThreadsafeModel, IObservable<ITransition>> tryOverwrite(TThreadsafeModel originalEntity)
            => serverEntity
                => dataSource.OverwriteIfOriginalDidNotChange(originalEntity, serverEntity)
                    .SelectMany(result =>
                        result is IgnoreResult<TThreadsafeModel>
                            ? updateId(originalEntity.Id, serverEntity.Id)
                            : Finished.Transition(extractFrom(result)).Apply(Observable.Return));

        private IObservable<ITransition> updateId(long originalId, long id)
            => dataSource.ChangeId(originalId, id).Select(EntityChanged.Transition);

        private TThreadsafeModel extractFrom(IConflictResolutionResult<TThreadsafeModel> result)
        {
            switch (result)
            {
                case CreateResult<TThreadsafeModel> c:
                    return c.Entity;
                case UpdateResult<TThreadsafeModel> u:
                    return u.Entity;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result));
            }
        }
    }
}
