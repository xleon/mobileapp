using System;
using System.Collections.Generic;
using System.Reactive;
using Toggl.Core.Analytics;
using Toggl.Core.Diagnostics;
using Toggl.Core.Models;
using Toggl.Core.Sync;

namespace Toggl.Core.Interactors
{
    public sealed partial class InteractorFactory : IInteractorFactory
    {
        public IInteractor<IObservable<IEnumerable<SyncFailureItem>>> GetItemsThatFailedToSync()
            => new GetItemsThatFailedToSyncInteractor(dataSource);

        public IInteractor<IObservable<bool>> HasFinishedSyncBefore()
            => new HasFinsihedSyncBeforeInteractor(dataSource);

        public IInteractor<IObservable<SyncOutcome>> RunBackgroundSync()
            => new RunBackgroundSyncInteractor(syncManager, analyticsService, stopwatchProvider);

        public IInteractor<IObservable<bool>> ContainsPlaceholders()
            => new ContainsPlaceholdersInteractor(dataSource);

        public IInteractor<IObservable<SyncOutcome>> RunPushNotificationInitiatedSyncInForeground()
            => new RunSyncInteractor(
                syncManager,
                stopwatchProvider,
                analyticsService,
                PushNotificationSyncSourceState.Foreground);

        public IInteractor<IObservable<SyncOutcome>> RunPushNotificationInitiatedSyncInBackground()
            => new RunSyncInteractor(
                syncManager,
                stopwatchProvider,
                analyticsService,
                PushNotificationSyncSourceState.Background);
    }
}
