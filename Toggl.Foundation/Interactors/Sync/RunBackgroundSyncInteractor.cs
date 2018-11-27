using System;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Multivac;
using Toggl.Foundation.Models;
using Toggl.Foundation.Sync;
using Toggl.Foundation.Analytics;

namespace Toggl.Foundation.Interactors
{
    public class RunBackgroundSyncInteractor : IInteractor<IObservable<SyncOutcome>>
    {
        private readonly ISyncManager syncManager;
        private readonly IAnalyticsService analyticsService;

        public RunBackgroundSyncInteractor(ISyncManager syncManager, IAnalyticsService analyticsService)
        {
            Ensure.Argument.IsNotNull(syncManager, nameof(syncManager));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));

            this.syncManager = syncManager;
            this.analyticsService = analyticsService;
        }

        public IObservable<SyncOutcome> Execute()
        {
            return syncManager.ForceFullSync()
                              .LastAsync()
                              .Select(_ => SyncOutcome.NewData)
                              .Catch(Observable.Return(SyncOutcome.Failed));
        }
    }
}
