using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Interactors;
using Toggl.Core.Login;
using Toggl.Core.Sync;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage;

namespace Toggl.Core.Services
{
    public sealed class AutomaticSyncingService : IAutomaticSyncingService
    {
        public static TimeSpan MinimumTimeInBackgroundForFullSync { get; } = TimeSpan.FromMinutes(5);

        private readonly IBackgroundService backgroundService;
        private readonly ITimeService timeService;

        private CompositeDisposable syncingDisposeBag;

        public AutomaticSyncingService(
            IBackgroundService backgroundService,
            ITimeService timeService)
        {
            Ensure.Argument.IsNotNull(backgroundService, nameof(backgroundService));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.backgroundService = backgroundService;
            this.timeService = timeService;
        }

        public void Start(ISyncManager syncManager)
        {
            Stop();

            backgroundService.AppResumedFromBackground
                .Where(timeInBackground => timeInBackground >= MinimumTimeInBackgroundForFullSync)
                .SelectMany(_ => syncManager.ForceFullSync())
                .Subscribe()
                .DisposedBy(syncingDisposeBag);

            timeService.MidnightObservable
                .Subscribe(_ => syncManager.CleanUp())
                .DisposedBy(syncingDisposeBag);
        }

        public void Stop()
        {
            syncingDisposeBag?.Dispose();
            syncingDisposeBag = new CompositeDisposable();
        }
    }
}
