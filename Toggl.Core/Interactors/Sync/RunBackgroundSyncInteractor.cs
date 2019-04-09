using System;
using System.Diagnostics;
using System.Reactive.Linq;
using Toggl.Core.Analytics;
using Toggl.Core.Diagnostics;
using Toggl.Core.Models;
using Toggl.Core.Sync;
using Toggl.Shared;

namespace Toggl.Core.Interactors
{
    public class RunBackgroundSyncInteractor : IInteractor<IObservable<SyncOutcome>>
    {
        private readonly ISyncManager syncManager;
        private readonly IAnalyticsService analyticsService;
        private readonly IStopwatchProvider stopwatchProvider;

        public RunBackgroundSyncInteractor(
            ISyncManager syncManager,
            IAnalyticsService analyticsService,
            IStopwatchProvider stopwatchProvider)
        {
            Ensure.Argument.IsNotNull(syncManager, nameof(syncManager));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(stopwatchProvider, nameof(stopwatchProvider));

            this.syncManager = syncManager;
            this.analyticsService = analyticsService;
            this.stopwatchProvider = stopwatchProvider;
        }

        public IObservable<SyncOutcome> Execute()
        {
            var syncTimeStopwatch = stopwatchProvider.Create(MeasuredOperation.BackgroundSync);
            syncTimeStopwatch.Start();
            analyticsService.BackgroundSyncStarted.Track();
            return syncManager.ForceFullSync()
                .LastAsync()
                .Select(_ => SyncOutcome.NewData)
                .Catch((Exception error) => syncFailed(error))
                .Do(outcome =>
                {
                    syncTimeStopwatch.Stop();
                    analyticsService.BackgroundSyncFinished.Track(outcome.ToString());
                });
        }

        private IObservable<SyncOutcome> syncFailed(Exception error)
        {
            analyticsService.BackgroundSyncFailed
                .Track(error.GetType().FullName, error.Message, error.StackTrace);
            return Observable.Return(SyncOutcome.Failed);
        }
    }
}
