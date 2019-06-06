using System;
using System.Reactive.Linq;
using Toggl.Core.Analytics;
using Toggl.Core.Diagnostics;
using Toggl.Core.Models;
using Toggl.Core.Sync;
using Toggl.Shared;

namespace Toggl.Core.Interactors
{
    public class RunSyncInteractor : IInteractor<IObservable<SyncOutcome>>
    {
        private readonly ISyncManager syncManager;
        private readonly IStopwatchProvider stopwatchProvider;
        private readonly Func<ISyncManager, IObservable<SyncState>> syncAction;
        private readonly MeasuredOperation measuredSyncOperation;
        private readonly IAnalyticsEvent syncStartedAnalyticsEvent;
        private readonly IAnalyticsEvent<string> syncFinishedAnalyticsEvent;
        private readonly IAnalyticsEvent<string, string, string> syncFailedAnalyticsEvent;

        public RunSyncInteractor(
            ISyncManager syncManager,
            IStopwatchProvider stopwatchProvider,
            Func<ISyncManager, IObservable<SyncState>> syncAction,
            MeasuredOperation measuredSyncOperation,
            IAnalyticsEvent syncStartedAnalyticsEvent,
            IAnalyticsEvent<string> syncFinishedAnalyticsEvent,
            IAnalyticsEvent<string, string, string> syncFailedAnalyticsEvent)
        {
            Ensure.Argument.IsNotNull(syncManager, nameof(syncManager));
            Ensure.Argument.IsNotNull(stopwatchProvider, nameof(stopwatchProvider));
            Ensure.Argument.IsNotNull(syncAction, nameof(syncAction));
            Ensure.Argument.IsADefinedEnumValue(measuredSyncOperation, nameof(measuredSyncOperation));

            this.syncManager = syncManager;
            this.stopwatchProvider = stopwatchProvider;
            this.syncAction = syncAction;
            this.measuredSyncOperation = measuredSyncOperation;
            this.syncStartedAnalyticsEvent = syncStartedAnalyticsEvent;
            this.syncFinishedAnalyticsEvent = syncFinishedAnalyticsEvent;
            this.syncFailedAnalyticsEvent = syncFailedAnalyticsEvent;
        }

        public IObservable<SyncOutcome> Execute()
        {
            var syncTimeStopwatch = stopwatchProvider.Create(measuredSyncOperation);
            syncTimeStopwatch.Start();
            syncStartedAnalyticsEvent?.Track();
            return syncAction(syncManager)
                .LastAsync()
                .Select(_ => SyncOutcome.NewData)
                .Catch((Exception error) => syncFailed(error))
                .Do(outcome =>
                {
                    syncTimeStopwatch.Stop();
                    syncFinishedAnalyticsEvent?.Track(outcome.ToString());
                });
        }

        private IObservable<SyncOutcome> syncFailed(Exception error)
        {
            syncFailedAnalyticsEvent?
                .Track(error.GetType().FullName, error.Message, error.StackTrace);
            return Observable.Return(SyncOutcome.Failed);
        }
    }
}
