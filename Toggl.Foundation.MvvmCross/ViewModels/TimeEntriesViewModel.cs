using System;
using System.Linq;
using System.Reactive.Disposables;
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
using Toggl.Foundation.Services;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using System.Collections.Immutable;
using Toggl.Foundation.MvvmCross.ViewModels.TimeEntriesLog;
using Toggl.Foundation.Extensions;
using System.Collections.Generic;
using Toggl.Foundation.MvvmCross.Transformations;
using System.Reactive;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class TimeEntriesViewModel
    {
        private readonly ITogglDataSource dataSource;
        private readonly IInteractorFactory interactorFactory;
        private readonly IAnalyticsService analyticsService;
        private readonly ISchedulerProvider schedulerProvider;
        private readonly ITimeService timeService;

        private Subject<bool> showUndoSubject = new Subject<bool>();
        private IDisposable delayedDeletionDisposable;
        private TimeEntryViewModel timeEntryToDelete;

        public IObservable<IImmutableList<CollectionSection<DaySummaryViewModel, TimeEntryViewModel>>> TimeEntries { get; }
        public IObservable<bool> Empty { get; }
        public IObservable<int> Count { get; }
        public IObservable<bool> ShouldShowUndo { get; }

        public InputAction<TimeEntryViewModel> DelayDeleteTimeEntry { get; }
        public UIAction CancelDeleteTimeEntry { get; }

        public TimeEntriesViewModel (ITogglDataSource dataSource,
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
            this.timeService = timeService;

            var whenDeletingTimeEntry = showUndoSubject.SelectUnit().StartWith(Unit.Default);

            TimeEntries =
                interactorFactory.ObserveAllTimeEntriesVisibleToTheUser().Execute()
                    .CombineLatest(whenDeletingTimeEntry, (timeEntries, _) => timeEntries)
                    .CombineLatest(dataSource.Preferences.Current, processTimeEntries)
                    .AsDriver(schedulerProvider);

            Empty = TimeEntries
                .Select(groups => groups.None())
                .AsDriver(schedulerProvider);

            Count = TimeEntries
                .Select(log => log.Sum(day => day.Items.Count))
                .AsDriver(schedulerProvider);

            DelayDeleteTimeEntry = rxActionFactory.FromAction<TimeEntryViewModel>(delayDeleteTimeEntry);
            CancelDeleteTimeEntry = rxActionFactory.FromAction(cancelDeleteTimeEntry);

            ShouldShowUndo = showUndoSubject.AsObservable().AsDriver(schedulerProvider);
        }

        public async Task FinilizeDelayDeleteTimeEntryIfNeeded()
        {
            if (timeEntryToDelete == null)
            {
                return;
            }

            delayedDeletionDisposable.Dispose();
            await deleteTimeEntry(timeEntryToDelete);
            timeEntryToDelete = null;
            showUndoSubject.OnNext(false);
        }

        private void delayDeleteTimeEntry(TimeEntryViewModel timeEntry)
        {
            timeEntryToDelete = timeEntry;

            showUndoSubject.OnNext(true);

            delayedDeletionDisposable = Observable.Merge( // If 5 seconds pass or we try to delete another TE
                    Observable.Return(timeEntry).Delay(Constants.UndoTime, schedulerProvider.DefaultScheduler),
                    showUndoSubject.Where(t => t).SelectValue(timeEntry)
                )
                .Take(1)
                .SelectMany(deleteTimeEntry)
                .Do(te =>
                {
                    if (te == timeEntryToDelete) // Hide bar if there isn't other TE trying to be deleted
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

        private IObservable<TimeEntryViewModel> deleteTimeEntry(TimeEntryViewModel timeEntry)
        {
            return interactorFactory
                .DeleteTimeEntry(timeEntry.Id)
                .Execute()
                .Do(_ =>
                {
                    analyticsService.DeleteTimeEntry.Track();
                    dataSource.SyncManager.PushSync();
                })
                .SelectValue(timeEntry);
        }

        private IImmutableList<CollectionSection<DaySummaryViewModel, TimeEntryViewModel>> processTimeEntries(
            IEnumerable<IThreadSafeTimeEntry> timeEntries, IThreadSafePreferences preferences)
            => timeEntries
                .Where(isNotRunning)
                .Where(isNotDeleted)
                .Select(te => new TimeEntryViewModel(te, preferences.DurationFormat))
                .OrderByDescending(te => te.StartTime)
                .GroupBy(te => te.StartTime.LocalDateTime.Date)
                .Select(group => transform(group, preferences))
                .ToImmutableList();

        private bool isNotRunning(IThreadSafeTimeEntry timeEntry) => !timeEntry.IsRunning();

        private bool isNotDeleted(IThreadSafeTimeEntry timeEntry) => timeEntry.Id != timeEntryToDelete?.Id;

        private CollectionSection<DaySummaryViewModel, TimeEntryViewModel> transform(
            IGrouping<DateTime, TimeEntryViewModel> dayGroup, IThreadSafePreferences preferences)
            => new CollectionSection<DaySummaryViewModel, TimeEntryViewModel>(
                new DaySummaryViewModel(
                    DateToTitleString.Convert(dayGroup.Key, timeService.CurrentDateTime),
                    totalTrackedTime(dayGroup).ToFormattedString(preferences.DurationFormat)),
                dayGroup);

        private TimeSpan totalTrackedTime(IEnumerable<TimeEntryViewModel> timeEntries)
            => timeEntries.Sum(timeEntry => timeEntry.Duration ?? TimeSpan.Zero);
    }
}
