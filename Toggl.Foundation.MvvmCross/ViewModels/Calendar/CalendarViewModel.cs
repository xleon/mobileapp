using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Toggl.Foundation;
using Toggl.Foundation.Calendar;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.Extensions;
using Toggl.Foundation.Interactors;
using Toggl.Foundation.MvvmCross.Collections;
using Toggl.Foundation.MvvmCross.Extensions;
using Toggl.Foundation.MvvmCross.Services;
using Toggl.Foundation.MvvmCross.ViewModels.Calendar;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.PrimeRadiant.Settings;

[assembly: MvxNavigation(typeof(CalendarViewModel), ApplicationUrls.Calendar.Regex)]
namespace Toggl.Foundation.MvvmCross.ViewModels.Calendar
{
    [Preserve(AllMembers = true)]
    public sealed class CalendarViewModel : MvxViewModel
    {
        private readonly ITimeService timeService;
        private readonly ITogglDataSource dataSource;
        private readonly IUserPreferences userPreferences;
        private readonly IInteractorFactory interactorFactory;
        private readonly IOnboardingStorage onboardingStorage;
        private readonly IPermissionsService permissionsService;
        private readonly IMvxNavigationService navigationService;

        private readonly ISubject<bool> shouldShowOnboardingSubject;
        private readonly CompositeDisposable disposeBag = new CompositeDisposable();

        public IObservable<bool> ShouldShowOnboarding { get; }

        public IObservable<TimeFormat> TimeOfDayFormat { get; }

        public IObservable<DateTime> Date { get; }

        public UIAction GetStartedAction { get; }

        public InputAction<CalendarItem> OnItemTapped { get; }

        public InputAction<(DateTimeOffset, TimeSpan)> OnDurationSelected { get; }

        public InputAction<CalendarItem> OnUpdateTimeEntry { get; }

        public ObservableGroupedOrderedCollection<CalendarItem> CalendarItems { get; }

        public CalendarViewModel(
            ITogglDataSource dataSource,
            ITimeService timeService,
            IUserPreferences userPreferences,
            IInteractorFactory interactorFactory,
            IOnboardingStorage onboardingStorage,
            ISchedulerProvider schedulerProvider,
            IPermissionsService permissionsService,
            IMvxNavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(userPreferences, nameof(userPreferences));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(onboardingStorage, nameof(onboardingStorage));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(permissionsService, nameof(permissionsService));

            this.dataSource = dataSource;
            this.timeService = timeService;
            this.userPreferences = userPreferences;
            this.interactorFactory = interactorFactory;
            this.onboardingStorage = onboardingStorage;
            this.navigationService = navigationService;
            this.permissionsService = permissionsService;

            var isCompleted = onboardingStorage.CompletedCalendarOnboarding();
            shouldShowOnboardingSubject = new BehaviorSubject<bool>(!isCompleted);
            ShouldShowOnboarding = shouldShowOnboardingSubject
                .AsObservable()
                .DistinctUntilChanged()
                .AsDriver(false, schedulerProvider);

            TimeOfDayFormat = dataSource
                .Preferences
                .Current
                .Select(preferences => preferences.TimeOfDayFormat);

            Date = Observable.Return(timeService.CurrentDateTime.Date);

            GetStartedAction = new UIAction(getStarted);

            OnItemTapped = new InputAction<CalendarItem>(onItemTapped);

            OnDurationSelected = new InputAction<(DateTimeOffset StartTime, TimeSpan Duration)>(
                tuple => onDurationSelected(tuple.StartTime, tuple.Duration)
            );

            OnUpdateTimeEntry = new InputAction<CalendarItem>(onUpdateTimeEntry);

            CalendarItems = new ObservableGroupedOrderedCollection<CalendarItem>(
                indexKey: item => item.StartTime,
                orderingKey: item => item.StartTime,
                groupingKey: _ => 0);
        }

        public void Init(string eventId)
        {
        }

        public async override Task Initialize()
        {
            var dayChangedObservable = timeService
                .MidnightObservable
                .SelectUnit();

            var selectedCalendarsChangedObservable = userPreferences
                .EnabledCalendars
                .SelectUnit();

            dataSource.TimeEntries
                .ItemsChanged()
                .Merge(dayChangedObservable)
                .Merge(selectedCalendarsChangedObservable)
                .Subscribe(reloadData)
                .DisposedBy(disposeBag);

            await reloadData();
        }

        private IObservable<Unit> getStarted()
            => Observable.FromAsync(async () =>
            {
                var calendarPermissionGranted = await permissionsService.RequestCalendarAuthorization();
                if (calendarPermissionGranted)
                {
                    await selectUserCalendars();
                    await permissionsService.RequestNotificationAuthorization();
                }
                else
                {
                    await navigationService.Navigate<CalendarPermissionDeniedViewModel, Unit>();
                }

                onboardingStorage.SetCompletedCalendarOnboarding();
                shouldShowOnboardingSubject.OnNext(false);
            });

        private async Task selectUserCalendars()
        {
            var calendarsExist = await interactorFactory
                .GetUserCalendars()
                .Execute()
                .Select(calendars => calendars.Any());

            if (calendarsExist)
            {
                var calendarIds = await navigationService.Navigate<SelectUserCalendarsViewModel, string[]>();
                interactorFactory.SetEnabledCalendars(calendarIds).Execute();
            }
        }

        private IObservable<Unit> onItemTapped(CalendarItem calendarItem)
            => Observable.FromAsync(async () =>
            {
                switch (calendarItem.Source)
                {
                    case CalendarItemSource.TimeEntry when calendarItem.TimeEntryId.HasValue:
                        await navigationService.Navigate<EditTimeEntryViewModel, long>(calendarItem.TimeEntryId.Value);
                        break;

                    case CalendarItemSource.Calendar:
                        var workspace = await interactorFactory.GetDefaultWorkspace().Execute();
                        var prototype = calendarItem.AsTimeEntryPrototype(workspace.Id);
                        var timeEntry = await interactorFactory.CreateTimeEntry(prototype).Execute();
                        await navigationService.Navigate<EditTimeEntryViewModel, long>(timeEntry.Id);
                        break;
                }
            });

        private IObservable<Unit> onDurationSelected(DateTimeOffset startTime, TimeSpan duration)
            => Observable.FromAsync(async () =>
            {
                var workspace = await interactorFactory.GetDefaultWorkspace().Execute();
                var prototype = duration.AsTimeEntryPrototype(startTime, workspace.Id);
                await interactorFactory.CreateTimeEntry(prototype).Execute();

                await reloadData();
            });

        private IObservable<Unit> onUpdateTimeEntry(CalendarItem calendarItem)
            => Observable.FromAsync(async () =>
            {
                if (!calendarItem.IsEditable() || calendarItem.TimeEntryId == null)
                    return;

                var timeEntry = await dataSource.TimeEntries.GetById((calendarItem.TimeEntryId.Value));

                var dto = new DTOs.EditTimeEntryDto
                {
                    Id = timeEntry.Id,
                    Description = timeEntry.Description,
                    StartTime = calendarItem.StartTime,
                    StopTime = calendarItem.EndTime,
                    ProjectId = timeEntry.ProjectId,
                    TaskId = timeEntry.TaskId,
                    Billable = timeEntry.Billable,
                    WorkspaceId = timeEntry.WorkspaceId,
                    TagIds = timeEntry.TagIds
                };

                await interactorFactory.UpdateTimeEntry(dto).Execute();
                await reloadData();
            });

        private async Task reloadData()
            => await interactorFactory
                .GetCalendarItemsForDate(timeService.CurrentDateTime.Date)
                .Execute()
                .Do(CalendarItems.ReplaceWith);
    }
}
