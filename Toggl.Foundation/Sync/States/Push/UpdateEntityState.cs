using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Toggl.Foundation.Analytics;
using Toggl.Foundation.DataSources.Interfaces;
using Toggl.Foundation.Extensions;
using Toggl.Foundation.Models.Interfaces;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave.ApiClients;
using static Toggl.Foundation.Sync.PushSyncOperation;

namespace Toggl.Foundation.Sync.States.Push
{
    internal sealed class UpdateEntityState<TModel, TThreadsafeModel>
        : BasePushEntityState<TThreadsafeModel>
        where TThreadsafeModel : class, TModel, IDatabaseSyncable, IThreadSafeModel
    {
        private readonly IUpdatingApiClient<TModel> api;

        private readonly IBaseDataSource<TThreadsafeModel> dataSource;

        private readonly Func<TModel, TThreadsafeModel> convertToThreadsafeModel;

        public StateResult<TThreadsafeModel> EntityChanged { get; } = new StateResult<TThreadsafeModel>();

        public StateResult<TThreadsafeModel> Finished { get; } = new StateResult<TThreadsafeModel>();

        public UpdateEntityState(
            IUpdatingApiClient<TModel> api,
            IBaseDataSource<TThreadsafeModel> dataSource,
            IAnalyticsService analyticsService,
            Func<TModel, TThreadsafeModel> convertToThreadsafeModel)
        {
            Ensure.Argument.IsNotNull(api, nameof(api));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(convertToThreadsafeModel, nameof(convertToThreadsafeModel));

            this.api = api;
            this.dataSource = dataSource;
            this.convertToThreadsafeModel = convertToThreadsafeModel;
        }

        public override IObservable<ITransition> Start(TThreadsafeModel entity)
            => update(entity)
                .Select(convertToThreadsafeModel)
                .SelectMany(tryOverwrite(entity))
                .SelectMany(CommonFunctions.Identity)
                .SingleAsync()
                .Track(AnalyticsService.EntitySynced, Update, entity.GetSafeTypeName())
                .Track(AnalyticsService.EntitySyncStatus, entity.GetSafeTypeName(), $"{Update}:{Resources.Success}")
                .Catch(Fail(entity, Update));

        private Func<TThreadsafeModel, IObservable<IEnumerable<ITransition>>> tryOverwrite(TThreadsafeModel originalEntity)
            => updatedEntity
                => dataSource.OverwriteIfOriginalDidNotChange(originalEntity, updatedEntity)
                    .Select(result =>
                        result is IgnoreResult<TThreadsafeModel>
                            ? EntityChanged.Transition(originalEntity)
                            : Finished.Transition(extractFrom(result)));

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

        private IObservable<TModel> update(TModel entity)
            => entity == null
                ? Observable.Throw<TModel>(new ArgumentNullException(nameof(entity)))
                : api.Update(entity);
    }
}
