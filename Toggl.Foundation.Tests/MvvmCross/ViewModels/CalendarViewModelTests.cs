using System;
using System.Reactive;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Foundation.MvvmCross.ViewModels.Calendar;
using Toggl.Foundation.Calendar;
using Toggl.Foundation.Interactors;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.Tests.Generators;
using Toggl.Foundation.Tests.Mocks;
using Xunit;
using ITimeEntryPrototype = Toggl.Foundation.Models.ITimeEntryPrototype;
using FsCheck.Xunit;
using FsCheck;
using System.Linq;
using Microsoft.Reactive.Testing;
using Toggl.Multivac;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public sealed class CalendarViewModelTests
    {
        public abstract class CalendarViewModelTest : BaseViewModelTests<CalendarViewModel>
        {
            protected const long TimeEntryId = 10;
            protected const long DefaultWorkspaceId = 1;

            protected static DateTimeOffset Now { get; } = new DateTimeOffset(2018, 8, 10, 12, 0, 0, TimeSpan.Zero);

            protected IInteractor<IObservable<IEnumerable<CalendarItem>>> CalendarInteractor { get; }

            protected CalendarViewModelTest()
            {
                CalendarInteractor = Substitute.For<IInteractor<IObservable<IEnumerable<CalendarItem>>>>();

                var workspace = new MockWorkspace { Id = DefaultWorkspaceId };
                var timeEntry = new MockTimeEntry { Id = TimeEntryId };

                TimeService.CurrentDateTime.Returns(Now);

                InteractorFactory
                    .GetCalendarItemsForDate(Arg.Any<DateTime>())
                    .Returns(CalendarInteractor);

                InteractorFactory
                    .GetDefaultWorkspace()
                    .Execute()
                    .Returns(Observable.Return(workspace));

                InteractorFactory
                    .CreateTimeEntry(Arg.Any<ITimeEntryPrototype>())
                    .Execute()
                    .Returns(Observable.Return(timeEntry));
            }

            protected override CalendarViewModel CreateViewModel()
                => new CalendarViewModel(
                    DataSource,
                    TimeService,
                    InteractorFactory,
                    OnboardingStorage,
                    SchedulerProvider,
                    PermissionsService,
                    NavigationService
                );
        }

        public sealed class TheConstructor : CalendarViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useDataSource,
                bool useTimeService,
                bool useInteractorFactory,
                bool useOnboardingStorage,
                bool useSchedulerProvider,
                bool useNavigationService,
                bool usePermissionsService)
            {
                var dataSource = useDataSource ? DataSource : null;
                var timeService = useTimeService ? TimeService : null;
                var interactorFactory = useInteractorFactory ? InteractorFactory : null;
                var onboardingStorage = useOnboardingStorage ? OnboardingStorage : null;
                var schedulerProvider = useSchedulerProvider ? SchedulerProvider : null;
                var navigationService = useNavigationService ? NavigationService : null;
                var permissionsService = usePermissionsService ? PermissionsService : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new CalendarViewModel(
                        dataSource,
                        timeService,
                        interactorFactory,
                        onboardingStorage,
                        schedulerProvider,
                        permissionsService,
                        navigationService);

                tryingToConstructWithEmptyParameters.Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheShouldShowOnboardingProperty : CalendarViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void ReturnsTrueIfCalendarOnboardingHasntBeenCompleted()
            {
                OnboardingStorage.CompletedCalendarOnboarding().Returns(false);
                var viewModel = CreateViewModel();
                var observer = TestScheduler.CreateObserver<bool>();

                viewModel.ShouldShowOnboarding.Subscribe(observer);

                TestScheduler.Start();
                observer.Messages.Single().Value.Value.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseIfCalendarOnboardingHasBeenCompleted()
            {
                OnboardingStorage.CompletedCalendarOnboarding().Returns(true);
                var viewModel = CreateViewModel();
                var observer = TestScheduler.CreateObserver<bool>();

                viewModel.ShouldShowOnboarding.Subscribe(observer);

                TestScheduler.Start();
                observer.Messages.Single().Value.Value.Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public async Task EmitsFalseWhenUserGrantsCalendarAccess()
            {
                OnboardingStorage.CompletedCalendarOnboarding().Returns(false);
                var observer = TestScheduler.CreateObserver<bool>();
                ViewModel.ShouldShowOnboarding.Subscribe(observer);
                PermissionsService.RequestCalendarAuthorization().Returns(Observable.Return(true));
                NavigationService.Navigate<SelectUserCalendarsViewModel, string[]>().Returns(new string[0]);

                await ViewModel.GetStartedAction.Execute(Unit.Default);

                TestScheduler.Start();
                observer.Messages.Select(message => message.Value.Value).AssertEqual(true, false);
            }

            [Fact, LogIfTooSlow]
            public async Task EmitsFalseWhenUserWantsToContinueWithoutCalendarAccess()
            {
                OnboardingStorage.CompletedCalendarOnboarding().Returns(false);
                var observer = TestScheduler.CreateObserver<bool>();
                ViewModel.ShouldShowOnboarding.Subscribe(observer);
                PermissionsService.RequestCalendarAuthorization().Returns(Observable.Return(false));
                NavigationService.Navigate<CalendarPermissionDeniedViewModel, Unit>().Returns(Unit.Default);

                await ViewModel.GetStartedAction.Execute(Unit.Default);

                TestScheduler.Start();
                observer.Messages.Select(message => message.Value.Value).AssertEqual(true, false);
            }
        }

        public sealed class TheGetStartedAction : CalendarViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task RequestsCalendarPermission()
            {
                await ViewModel.GetStartedAction.Execute(Unit.Default);

                await PermissionsService.Received().RequestCalendarAuthorization();
            }

            [Fact, LogIfTooSlow]
            public async Task NavigatesToTheCalendarPermissionDeniedViewModelWhenPermissionIsDenied()
            {
                PermissionsService.RequestCalendarAuthorization().Returns(Observable.Return(false));

                await ViewModel.GetStartedAction.Execute(Unit.Default);

                await NavigationService.Received().Navigate<CalendarPermissionDeniedViewModel, Unit>();
            }

            [Fact, LogIfTooSlow]
            public async Task NavigatesToTheSelectUserCalendarsViewModelWhenThereAreCalendars()
            {
                PermissionsService.RequestCalendarAuthorization().Returns(Observable.Return(true));
                InteractorFactory.GetUserCalendars().Execute().Returns(
                    Observable.Return(new UserCalendar[] { new UserCalendar() })
                );

                await ViewModel.GetStartedAction.Execute(Unit.Default);

                await NavigationService.Received().Navigate<SelectUserCalendarsViewModel, string[]>();
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNotNavigateToTheSelectUserCalendarsViewModelWhenThereAreNoCalendars()
            {
                PermissionsService.RequestCalendarAuthorization().Returns(Observable.Return(true));
                InteractorFactory.GetUserCalendars().Execute().Returns(
                    Observable.Return(new UserCalendar[0])
                );

                await ViewModel.GetStartedAction.Execute(Unit.Default);

                await NavigationService.DidNotReceive().Navigate<SelectUserCalendarsViewModel, string[]>();
            }

            [Property]
            public void SetsTheEnabledCalendarsWhenThereAreCalendars(NonEmptyString[] nonEmptyStrings)
            {
                if (nonEmptyStrings == null) return;
                var calendarIds = nonEmptyStrings.Select(str => str.Get).ToArray();
                NavigationService.Navigate<SelectUserCalendarsViewModel, string[]>().Returns(calendarIds);
                PermissionsService.RequestCalendarAuthorization().Returns(Observable.Return(true));
                InteractorFactory.GetUserCalendars().Execute().Returns(
                    Observable.Return(new UserCalendar[] { new UserCalendar() })
                );

                ViewModel.GetStartedAction.Execute(Unit.Default).Wait();

                InteractorFactory.Received().SetEnabledCalendars(calendarIds).Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task SetsCalendarOnboardingAsCompletedIfUserGrantsAccess()
            {
                PermissionsService.RequestCalendarAuthorization().Returns(Observable.Return(true));
                NavigationService.Navigate<SelectUserCalendarsViewModel, string[]>().Returns(new string[0]);

                await ViewModel.GetStartedAction.Execute(Unit.Default);

                OnboardingStorage.Received().SetCompletedCalendarOnboarding();
            }

            [Fact, LogIfTooSlow]
            public async Task SetsCalendarOnboardingAsCompletedIfUserWantsToContinueWithoutGivingPermission()
            {
                PermissionsService.RequestCalendarAuthorization().Returns(Observable.Return(false));
                NavigationService.Navigate<CalendarPermissionDeniedViewModel, Unit>().Returns(Unit.Default);

                await ViewModel.GetStartedAction.Execute(Unit.Default);

                OnboardingStorage.Received().SetCompletedCalendarOnboarding();
            }
        }

        public sealed class TheCalendarItemsProperty : CalendarViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ReturnsTheCalendarItemsForToday()
            {
                var now = new DateTimeOffset(2018, 8, 9, 12, 0, 0, TimeSpan.Zero);
                TimeService.CurrentDateTime.Returns(now);

                var items = new List<CalendarItem>
                {
                    new CalendarItem(CalendarItemSource.Calendar, now.AddMinutes(30), TimeSpan.FromMinutes(15), "Weekly meeting", CalendarIconKind.Event, "#ff0000"),
                    new CalendarItem(CalendarItemSource.TimeEntry, now.AddHours(-3), TimeSpan.FromMinutes(30), "Bug fixes", CalendarIconKind.None, "#00ff00"),
                    new CalendarItem(CalendarItemSource.Calendar, now.AddHours(2), TimeSpan.FromMinutes(30), "F**** timesheets", CalendarIconKind.Event, "#ff0000")
                };
                var interactor = Substitute.For<IInteractor<IObservable<IEnumerable<CalendarItem>>>>();
                interactor.Execute().Returns(Observable.Return(items));
                InteractorFactory.GetCalendarItemsForDate(Arg.Any<DateTime>()).Returns(interactor);

                await ViewModel.Initialize();

                ViewModel.CalendarItems[0].Should().BeEquivalentTo(items);
            }
        }

        public abstract class TheOnItemTappedAction : CalendarViewModelTest
        {
            protected abstract CalendarItem CalendarItem { get; }

            [Fact]
            public async Task NavigatesToTheEditTimeEntryViewModelUsingTheTimeEntryId()
            {
                await ViewModel.OnItemTapped.Execute(CalendarItem);

                await NavigationService.Received().Navigate<EditTimeEntryViewModel, long, Unit>(Arg.Is(TimeEntryId));
            }

            [Fact]
            public async Task RefetchesTheTimeEntryItemsUsingTheInteractor()
            {
                await ViewModel.OnItemTapped.Execute(CalendarItem);

                await CalendarInteractor.Received().Execute();
            }

            public sealed class WhenHandlingTimeEntryItems : TheOnItemTappedAction
            {
                protected override CalendarItem CalendarItem { get; } = new CalendarItem(
                    CalendarItemSource.TimeEntry,
                    new DateTimeOffset(2018, 08, 10, 0, 0, 0, TimeSpan.Zero),
                    TimeSpan.FromMinutes(10),
                    "Working on something",
                    CalendarIconKind.None,
                    "#00FF00",
                    TimeEntryId
                );
            }

            public sealed class WhenHandlingCalendarItems : TheOnItemTappedAction
            {
                protected override CalendarItem CalendarItem { get; } = new CalendarItem(
                    CalendarItemSource.Calendar,
                    new DateTimeOffset(2018, 08, 10, 0, 15, 0, TimeSpan.Zero),
                    TimeSpan.FromMinutes(10),
                    "Meeting with someone",
                    CalendarIconKind.Event
                );

                [Fact]
                public async Task CreatesATimeEntryUsingTheCalendarItemInfo()
                {
                    await ViewModel.OnItemTapped.Execute(CalendarItem);

                    await InteractorFactory
                        .CreateTimeEntry(Arg.Is<ITimeEntryPrototype>(p => p.Description == CalendarItem.Description))
                        .Received()
                        .Execute();
                }

                [Fact]
                public async Task CreatesATimeEntryInTheDefaultWorkspace()
                {
                    await ViewModel.OnItemTapped.Execute(CalendarItem);

                    await InteractorFactory
                        .CreateTimeEntry(Arg.Is<ITimeEntryPrototype>(p => p.WorkspaceId == DefaultWorkspaceId))
                        .Received()
                        .Execute();
                }
            }
        }

        public sealed class TheOnDurationSelectedAction : CalendarViewModelTest
        {
            [Fact]
            public async Task CreatesATimeEntryWithTheSelectedStartDate()
            {
                var now = DateTimeOffset.UtcNow;
                var duration = TimeSpan.FromMinutes(30);
                var tuple = (now, duration);

                await ViewModel.OnDurationSelected.Execute(tuple);

                await InteractorFactory
                    .CreateTimeEntry(Arg.Is<ITimeEntryPrototype>(p => p.StartTime == now))
                    .Received()
                    .Execute();
            }

            [Fact]
            public async Task CreatesATimeEntryWithTheSelectedDuration()
            {
                var now = DateTimeOffset.UtcNow;
                var duration = TimeSpan.FromMinutes(30);
                var tuple = (now, duration);

                await ViewModel.OnDurationSelected.Execute(tuple);

                await InteractorFactory
                    .CreateTimeEntry(Arg.Is<ITimeEntryPrototype>(p => p.Duration == duration))
                    .Received()
                    .Execute();
            }

            [Fact]
            public async Task CreatesATimeEntryInTheDefaultWorkspace()
            {
                var now = DateTimeOffset.UtcNow;
                var duration = TimeSpan.FromMinutes(30);
                var tuple = (now, duration);

                await ViewModel.OnDurationSelected.Execute(tuple);

                await InteractorFactory
                    .CreateTimeEntry(Arg.Is<ITimeEntryPrototype>(p => p.WorkspaceId == DefaultWorkspaceId))
                    .Received()
                    .Execute();
            }

            [Fact]
            public async Task RefetchesTheTimeEntryItemsUsingTheInteractor()
            {
                var now = DateTimeOffset.UtcNow;
                var duration = TimeSpan.FromMinutes(30);
                var tuple = (now, duration);

                await ViewModel.OnDurationSelected.Execute(tuple);

                await CalendarInteractor.Received().Execute();
            }
        }
    }
}
