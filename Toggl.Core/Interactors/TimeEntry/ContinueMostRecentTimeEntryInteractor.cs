using System;
using System.Reactive.Linq;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Extensions;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Sync;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage;
using Toggl.Storage.Models;

namespace Toggl.Core.Interactors
{
    public class ContinueMostRecentTimeEntryInteractor : IInteractor<IObservable<IThreadSafeTimeEntry>>
    {
        private readonly IIdProvider idProvider;
        private readonly ITimeService timeService;
        private readonly ITogglDataSource dataSource;
        private readonly IAnalyticsService analyticsService;
        private readonly ISyncManager syncManager;

        public ContinueMostRecentTimeEntryInteractor(
            IIdProvider idProvider,
            ITimeService timeService,
            ITogglDataSource dataSource,
            IAnalyticsService analyticsService,
            ISyncManager syncManager)
        {
            Ensure.Argument.IsNotNull(idProvider, nameof(idProvider));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(syncManager, nameof(syncManager));

            this.idProvider = idProvider;
            this.dataSource = dataSource;
            this.timeService = timeService;
            this.analyticsService = analyticsService;
            this.syncManager = syncManager;
        }

        public IObservable<IThreadSafeTimeEntry> Execute()
            => dataSource.TimeEntries
                .GetAll(te => !te.IsDeleted)
                .Select(timeEntries => timeEntries.MaxBy(te => te.Start))
                .Select(newTimeEntry)
                .SelectMany(dataSource.TimeEntries.Create)
                .Do(_ => syncManager.InitiatePushSync())
                .Track(StartTimeEntryEvent.With(TimeEntryStartOrigin.ContinueMostRecent), analyticsService);

        private IThreadSafeTimeEntry newTimeEntry(IThreadSafeTimeEntry timeEntry)
            => TimeEntry.Builder
                        .Create(idProvider.GetNextIdentifier())
                        .SetTagIds(timeEntry.TagIds)
                        .SetUserId(timeEntry.UserId)
                        .SetTaskId(timeEntry.TaskId)
                        .SetBillable(timeEntry.Billable)
                        .SetProjectId(timeEntry.ProjectId)
                        .SetAt(timeService.CurrentDateTime)
                        .SetSyncStatus(SyncStatus.SyncNeeded)
                        .SetDescription(timeEntry.Description)
                        .SetStart(timeService.CurrentDateTime)
                        .SetWorkspaceId(timeEntry.WorkspaceId)
                        .Build();
    }
}
