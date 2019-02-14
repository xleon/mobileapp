using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Foundation.Analytics;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.Helper;
using Toggl.Foundation.Interactors;
using Toggl.Foundation.Models.Interfaces;
using Toggl.Foundation.MvvmCross.Collections;
using Toggl.Foundation.MvvmCross.Extensions;
using Toggl.Foundation.MvvmCross.Transformations;
using Toggl.Foundation.MvvmCross.ViewModels.TimeEntriesLog;
using Toggl.Foundation.Services;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;

namespace Toggl.Foundation.MvvmCross.ViewModels
{

    [Preserve(AllMembers = true)]
    public sealed class TimeEntriesViewModel
    {
        private readonly ITogglDataSource dataSource;
        private readonly IInteractorFactory interactorFactory;
        private readonly IAnalyticsService analyticsService;
        private readonly ISchedulerProvider schedulerProvider;

        private readonly TimeEntriesCollapsing collapsingStrategy;

        private Subject<bool> showUndoSubject = new Subject<bool>();
        private IDisposable delayedDeletionDisposable;
        private long? timeEntryToDelete;

        public IObservable<IEnumerable<CollectionSection<DaySummaryViewModel, LogItemViewModel>>> TimeEntries { get; }
        public IObservable<bool> Empty { get; }
        public IObservable<int> Count { get; }
        public IObservable<bool> ShouldShowUndo { get; }

        public InputAction<long> DelayDeleteTimeEntry { get; }
        public InputAction<GroupId> ToggleGroupExpansion { get; }
        public UIAction CancelDeleteTimeEntry { get; }

        public TimeEntriesViewModel(
            ITogglDataSource dataSource,
            IInteractorFactory interactorFactory,
            IAnalyticsService analyticsService,
            ISchedulerProvider schedulerProvider,
            IRxActionFactory rxActionFactory,
            ITimeService timeService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.dataSource = dataSource;
            this.interactorFactory = interactorFactory;
            this.analyticsService = analyticsService;
            this.schedulerProvider = schedulerProvider;

            DelayDeleteTimeEntry = rxActionFactory.FromAction<long>(delayDeleteTimeEntry);
            ToggleGroupExpansion = rxActionFactory.FromAction<GroupId>(toggleGroupExpansion);
            CancelDeleteTimeEntry = rxActionFactory.FromAction(cancelDeleteTimeEntry);

            collapsingStrategy = new TimeEntriesCollapsing(timeService, dataSource.Preferences.Current);

            var deletingOrPressingUndo = showUndoSubject.SelectUnit().StartWith(Unit.Default);
            var collapsingOrExpanding = ToggleGroupExpansion.Elements.StartWith(Unit.Default);

            TimeEntries =
                interactorFactory.ObserveAllTimeEntriesVisibleToTheUser().Execute()
                    .Select(timeEntries => timeEntries.Where(isNotRunning))
                    .CombineLatest(
                        deletingOrPressingUndo,
                        (timeEntries, _) => timeEntries.Where(isNotDeleted))
                    .CombineLatest(dataSource.Preferences.Current, group)
                    .CombineLatest(collapsingOrExpanding, (groups, _) => groups)
                    .Select(collapsingStrategy.Flatten)
                    .AsDriver(schedulerProvider);

            Empty = TimeEntries
                .Select(groups => groups.None())
                .AsDriver(schedulerProvider);

            Count = TimeEntries
                .Select(log => log.Sum(day => day.Items.Count))
                .AsDriver(schedulerProvider);

            ShouldShowUndo = showUndoSubject.AsObservable().AsDriver(schedulerProvider);
        }

        public async Task FinilizeDelayDeleteTimeEntryIfNeeded()
        {
            if (!timeEntryToDelete.HasValue)
            {
                return;
            }

            delayedDeletionDisposable.Dispose();
            await deleteTimeEntry(timeEntryToDelete.Value);
            timeEntryToDelete = null;
            showUndoSubject.OnNext(false);
        }

        private IEnumerable<CollectionSection<DateTimeOffset, IThreadSafeTimeEntry[]>> group(
            IEnumerable<IThreadSafeTimeEntry> timeEntries, IThreadSafePreferences preferences)
            => preferences.CollapseTimeEntries
                ? TimeEntriesGrouping.GroupSimilar(timeEntries)
                : TimeEntriesGrouping.WithoutGroupingSimilar(timeEntries);

        private void toggleGroupExpansion(GroupId groupId)
        {
            collapsingStrategy.ToggleGroupExpansion(groupId);
        }

        private void delayDeleteTimeEntry(long timeEntryId)
        {
            timeEntryToDelete = timeEntryId;

            showUndoSubject.OnNext(true);

            delayedDeletionDisposable = Observable.Merge( // If 5 seconds pass or we try to delete another TE
                    Observable.Return(timeEntryId).Delay(Constants.UndoTime, schedulerProvider.DefaultScheduler),
                    showUndoSubject.Where(t => t).SelectValue(timeEntryId)
                )
                .Take(1)
                .SelectMany(deleteTimeEntry)
                .Do(deletedTimeEntryId =>
                {
                    if (deletedTimeEntryId == timeEntryToDelete) // Hide bar if there isn't other TE trying to be deleted
                        showUndoSubject.OnNext(false);
                })
                .Subscribe();
        }

        private void cancelDeleteTimeEntry()
        {
            timeEntryToDelete = null;
            delayedDeletionDisposable.Dispose();
            showUndoSubject.OnNext(false);
        }

        private IObservable<long> deleteTimeEntry(long timeEntryId)
        {
            return interactorFactory
                .DeleteTimeEntry(timeEntryId)
                .Execute()
                .Do(_ =>
                {
                    analyticsService.DeleteTimeEntry.Track();
                    dataSource.SyncManager.PushSync();
                })
                .SelectValue(timeEntryId);
        }

        private bool isNotRunning(IThreadSafeTimeEntry timeEntry) => !timeEntry.IsRunning();

        private bool isNotDeleted(IThreadSafeTimeEntry timeEntry) => timeEntry.Id != timeEntryToDelete;
    }
}
