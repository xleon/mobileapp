using System;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Foundation.Analytics;
using Toggl.Foundation.DataSources.Interfaces;
using Toggl.Foundation.Extensions;
using Toggl.Foundation.Models.Interfaces;
using Toggl.Multivac;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave.ApiClients.Interfaces;
using static Toggl.Foundation.Sync.PushSyncOperation;

namespace Toggl.Foundation.Sync.States.Push
{
    internal sealed class DeleteEntityState<TModel, TDatabaseModel, TThreadsafeModel>
        : BasePushEntityState<TThreadsafeModel>
        where TModel : IIdentifiable
        where TDatabaseModel : class, TModel, IDatabaseSyncable
        where TThreadsafeModel : class, TDatabaseModel, IThreadSafeModel
    {
        private readonly IDeletingApiClient<TModel> api;

        private readonly IDataSource<TThreadsafeModel, TDatabaseModel> dataSource;

        private readonly IRateLimiter limiter;

        public StateResult DeletingFinished { get; } = new StateResult();

        public DeleteEntityState(
            IDeletingApiClient<TModel> api,
            IAnalyticsService analyticsService,
            IDataSource<TThreadsafeModel, TDatabaseModel> dataSource,
            IRateLimiter limiter)
            : base(analyticsService)
        {
            Ensure.Argument.IsNotNull(api, nameof(api));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(limiter, nameof(limiter));

            this.api = api;
            this.dataSource = dataSource;
            this.limiter = limiter;
        }

        public override IObservable<ITransition> Start(TThreadsafeModel entity)
            => delete(entity)
                .SelectMany(_ => dataSource.Delete(entity.Id))
                .Track(AnalyticsService.EntitySynced, Delete, entity.GetSafeTypeName())
                .Track(AnalyticsService.EntitySyncStatus, entity.GetSafeTypeName(), $"{Delete}:{Resources.Success}")
                .Select(_ => DeletingFinished.Transition())
                .Catch(Fail(entity, Delete));

        private IObservable<Unit> delete(TModel entity)
            => entity == null
                ? Observable.Throw<Unit>(new ArgumentNullException(nameof(entity)))
                : limiter.WaitForFreeSlot()
                    .ThenExecute(() => api.Delete(entity));
    }
}
