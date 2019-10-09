using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using FluentAssertions;
using FluentAssertions.Equivalency;
using NSubstitute;
using Toggl.Core.Analytics;
using Toggl.Core.Calendar;
using Toggl.Core.DTOs;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.Mocks;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Core.UI.Views;
using Xunit;
using static Toggl.Core.UI.ViewModels.Calendar.CalendarContextualMenuViewModel;
using ColorHelper = Toggl.Core.Helper.Colors;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class CalendarContextualMenuViewModelTests
    {
        public abstract class CalendarContextualMenuViewModelTest : BaseViewModelTests<CalendarContextualMenuViewModel>
        {
            protected override CalendarContextualMenuViewModel CreateViewModel()
                => new CalendarContextualMenuViewModel(InteractorFactory, AnalyticsService, RxActionFactory, TimeService, NavigationService);
            
            protected CalendarItem CreateEmptyCalendarItem()
                => new CalendarItem();
            
            protected CalendarItem CreateDummyTimeEntryCalendarItem(bool isRunning = false, long timeEntryId = 1, DateTimeOffset? startTime = null, TimeSpan? duration = null)
            {
                TimeSpan? nullTimespan = null;
                return new CalendarItem("1",
                    CalendarItemSource.TimeEntry,
                    startTime ?? DateTimeOffset.Now,
                    duration: isRunning ? nullTimespan : (duration ?? TimeSpan.FromMinutes(30)),
                    "",
                    CalendarIconKind.None,
                    timeEntryId: timeEntryId);
            }

            protected CalendarItem CreateDummyCalendarEventCalendarItem(
                string description = "",
                DateTimeOffset? startTime = null,
                TimeSpan? duration = null)
                => new CalendarItem("Id", 
                    CalendarItemSource.Calendar, 
                    startTime ?? DateTimeOffset.Now, 
                    duration, 
                    description,
                    CalendarIconKind.Event,
                    calendarId: "Id");
        }

        public sealed class TheConstructor : CalendarContextualMenuViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useInteractorFactory,
                bool useAnalyticsService,
                bool useRxActionFactory,
                bool useTimeService,
                bool useNavigationService)
            {
                Action tryingToConstructWithEmptyParameters =
                    () => new CalendarContextualMenuViewModel(
                        useInteractorFactory ? InteractorFactory : null,
                        useAnalyticsService ? AnalyticsService : null,
                        useRxActionFactory ? RxActionFactory : null,
                        useTimeService ? TimeService : null,
                        useNavigationService ? NavigationService : null
                    );

                tryingToConstructWithEmptyParameters.Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheCurrentMenuObservable : CalendarContextualMenuViewModelTest
        {
            [Fact]
            public void ShouldStartEmpty()
            {
                var menuObserver = TestScheduler.CreateObserver<CalendarContextualMenu>();
                var visibilityObserver = TestScheduler.CreateObserver<bool>();
                ViewModel.CurrentMenu.Subscribe(menuObserver);
                ViewModel.MenuVisible.Subscribe(visibilityObserver);
                
                menuObserver.Messages.Should().HaveCount(1);
                menuObserver.Messages.First().Value.Value.Actions.Should().BeEmpty();
                visibilityObserver.Messages.Should().HaveCount(1);
                visibilityObserver.Messages.First().Value.Value.Should().BeFalse();
            }
            
            [Fact]
            public void EmitsDiscardEditSaveActionsWhenNewCalendarItemIsBeingEdited()
            {
                var observer = TestScheduler.CreateObserver<CalendarContextualMenu>();
                var calendarItem = CreateEmptyCalendarItem();
                ViewModel.CurrentMenu.Subscribe(observer);
                
                ViewModel.OnItemSelected.Inputs.OnNext(calendarItem);

                observer.Messages.Should().HaveCount(2);
                var kinds = observer.Messages[1].Value.Value.Actions.Select(action => action.Kind);
                kinds.Should().ContainInOrder(
                    CalendarMenuActionKind.Discard,
                    CalendarMenuActionKind.Edit,
                    CalendarMenuActionKind.Save);
            }
            
            [Fact]
            public void EmitsDeleteEditSaveContinueActionsWhenExistingTimeEntryCalendarItemIsBeingEdited()
            {
                var observer = TestScheduler.CreateObserver<CalendarContextualMenu>();
                var calendarItem = CreateDummyTimeEntryCalendarItem(isRunning: false);
                ViewModel.CurrentMenu.Subscribe(observer);
                
                ViewModel.OnItemSelected.Inputs.OnNext(calendarItem);

                observer.Messages.Should().HaveCount(2);
                var kinds = observer.Messages[1].Value.Value.Actions.Select(action => action.Kind);
                kinds.Should().ContainInOrder(
                    CalendarMenuActionKind.Delete,
                    CalendarMenuActionKind.Edit,
                    CalendarMenuActionKind.Save,
                    CalendarMenuActionKind.Continue);
            }
            
            [Fact]
            public void EmitsDiscardEditSaveStopWhenItemIsBeingEdited()
            {
                var observer = TestScheduler.CreateObserver<CalendarContextualMenu>();
                var calendarItem = CreateDummyTimeEntryCalendarItem(isRunning: true);
                ViewModel.CurrentMenu.Subscribe(observer);
                
                ViewModel.OnItemSelected.Inputs.OnNext(calendarItem);

                observer.Messages.Should().HaveCount(2);
                var kinds = observer.Messages[1].Value.Value.Actions.Select(action => action.Kind);
                kinds.Should().ContainInOrder(
                    CalendarMenuActionKind.Discard,
                    CalendarMenuActionKind.Edit,
                    CalendarMenuActionKind.Save,
                    CalendarMenuActionKind.Stop);
            }
            
            [Fact]
            public void EmitsCopyStartActionsWhenCalendarEventCalendarItemIsBeingEdited()
            {
                var observer = TestScheduler.CreateObserver<CalendarContextualMenu>();
                var calendarItem = CreateDummyCalendarEventCalendarItem(duration: TimeSpan.FromMinutes(30));
                ViewModel.CurrentMenu.Subscribe(observer);
                
                ViewModel.OnItemSelected.Inputs.OnNext(calendarItem);

                observer.Messages.Should().HaveCount(2);
                var kinds = observer.Messages[1].Value.Value.Actions.Select(action => action.Kind);
                kinds.Should().ContainInOrder(
                    CalendarMenuActionKind.Copy,
                    CalendarMenuActionKind.Start);
            }
        }

        public abstract class TheMenuActionTests : CalendarContextualMenuViewModelTest
        {
            protected CalendarContextualMenu ContextualMenu { get; }
            
            public TheMenuActionTests()
            {
                var observer = TestScheduler.CreateObserver<CalendarContextualMenu>();
                var calendarItem = CreateMenuTypeCalendarItemTrigger();
                ViewModel.CurrentMenu.Subscribe(observer);
                ViewModel.OnItemSelected.Inputs.OnNext(calendarItem);

                ContextualMenu = observer.Messages.Last().Value.Value;
            }

            public abstract CalendarItem CreateMenuTypeCalendarItemTrigger();
            
            public virtual void TheDismissActionDiscardChangesAndClosesTheMenu()
                => DiscardsChangesAndCloseMenu(() => ContextualMenu.Dismiss.Inputs.OnNext(Unit.Default));
            
            public void ExecutesActionAndClosesMenu(Action action)
            {
                var menuVisibilityObserver = TestScheduler.CreateObserver<bool>();
                var discardsObserver = TestScheduler.CreateObserver<Unit>();
                var menuObserver = TestScheduler.CreateObserver<CalendarContextualMenu>();
                ViewModel.MenuVisible.Subscribe(menuVisibilityObserver);
                ViewModel.DiscardChanges.Subscribe(discardsObserver);
                ViewModel.CurrentMenu.Subscribe(menuObserver);
                menuVisibilityObserver.Messages.Clear();

                action();

                menuVisibilityObserver.Messages.First().Value.Value.Should().BeFalse();
                discardsObserver.Messages.Should().BeEmpty();
                menuObserver.Messages.Last().Value.Value.Actions.Should().BeEmpty();
            }

            public void DiscardsChangesAndCloseMenu(Action action)
            {
                var menuVisibilityObserver = TestScheduler.CreateObserver<bool>();
                var discardsObserver = TestScheduler.CreateObserver<Unit>();
                var menuObserver = TestScheduler.CreateObserver<CalendarContextualMenu>();
                ViewModel.MenuVisible.Subscribe(menuVisibilityObserver);
                ViewModel.DiscardChanges.Subscribe(discardsObserver);
                menuVisibilityObserver.Messages.Clear();
                ViewModel.CurrentMenu.Subscribe(menuObserver);
                discardsObserver.Messages.Should().HaveCount(0);

                action();

                menuVisibilityObserver.Messages.First().Value.Value.Should().BeFalse();
                discardsObserver.Messages.Should().HaveCount(1);
                menuObserver.Messages.Last().Value.Value.Actions.Should().BeEmpty();
            }
        }
        
        public sealed class TheMenuActionForCalendarEvents : TheMenuActionTests
        {
            public override CalendarItem CreateMenuTypeCalendarItemTrigger() 
                => CreateDummyCalendarEventCalendarItem();

            [Fact]
            public void TheCopyActionCreatesATimeEntryWithTheDetailsFromTheCalendarEvent()
            {
                var copyAction = ContextualMenu.Actions.First(action => action.Kind == CalendarMenuActionKind.Copy);
                var expectedDescription = "X";
                var expectedStartTime = new DateTimeOffset(2019, 10, 10, 10, 10, 10, TimeSpan.Zero);
                var expectedDuration = TimeSpan.FromHours(1);
                var calendarEventBeingEdited = CreateDummyCalendarEventCalendarItem(expectedDescription, expectedStartTime, expectedDuration);
                InteractorFactory.GetDefaultWorkspace().Execute().Returns(Observable.Return(new MockWorkspace(1)));
                
                copyAction.MenuItemAction.Inputs.OnNext(calendarEventBeingEdited);

                var prototypeArg = Arg.Is<ITimeEntryPrototype>(te =>
                        te.Description == expectedDescription
                           && te.StartTime == expectedStartTime
                           && te.Duration == expectedDuration
                           && te.WorkspaceId == 1
                );
                InteractorFactory.Received().CreateTimeEntry(prototypeArg, TimeEntryStartOrigin.CalendarEvent);
            }

            [Fact]
            public void TheCopyActionClosesTheMenuAfterItsExecution()
                => ExecutesActionAndClosesMenu(TheCopyActionCreatesATimeEntryWithTheDetailsFromTheCalendarEvent);

            [Fact]
            public void TheStartActionCreatesARunningTimeEntryWithTheDetailsFromTheCalendarEvent()
            {
                var startAction = ContextualMenu.Actions.First(action => action.Kind == CalendarMenuActionKind.Start);
                var expectedDescription = "X";
                var originalStartTime = new DateTimeOffset(2019, 10, 10, 10, 10, 10, TimeSpan.Zero);
                var calendarEventBeingEdited = CreateDummyCalendarEventCalendarItem(expectedDescription, originalStartTime);
                var now = DateTimeOffset.Now;
                TimeService.CurrentDateTime.Returns(now);
                InteractorFactory.GetDefaultWorkspace().Execute().Returns(Observable.Return(new MockWorkspace(1)));
                
                startAction.MenuItemAction.Inputs.OnNext(calendarEventBeingEdited);

                var prototypeArg = Arg.Is<ITimeEntryPrototype>(te =>
                    te.Description == expectedDescription
                    && te.StartTime == now 
                    && te.StartTime != originalStartTime
                    && te.Duration == null
                    && te.WorkspaceId == 1
                );
                InteractorFactory.Received().CreateTimeEntry(prototypeArg, TimeEntryStartOrigin.CalendarEvent);
            }

            [Fact]
            public void TheStartActionClosesTheMenuAfterItsExecution()
                => ExecutesActionAndClosesMenu(TheStartActionCreatesARunningTimeEntryWithTheDetailsFromTheCalendarEvent);

            [Fact]
            public override void TheDismissActionDiscardChangesAndClosesTheMenu()
            {
                base.TheDismissActionDiscardChangesAndClosesTheMenu();
            }
        }

        public sealed class TheMenuForNewTimeEntries : TheMenuActionTests
        {
            public override CalendarItem CreateMenuTypeCalendarItemTrigger()
                => CreateEmptyCalendarItem();

            [Fact]
            public void TheDiscardActionTriggersTheDiscardChangesObservable()
            {
                var observer = TestScheduler.CreateObserver<Unit>();
                ViewModel.DiscardChanges.Subscribe(observer);
                var discardAction = ContextualMenu.Actions.First(action => action.Kind == CalendarMenuActionKind.Discard);
                var expectedStartTime = new DateTimeOffset(2019, 10, 10, 10, 10, 10, TimeSpan.Zero);
                var expectedDuration = TimeSpan.FromMinutes(30);
                var expectedDescription = "whatever";
                var newCalendarItem = new CalendarItem(
                    string.Empty,
                    CalendarItemSource.TimeEntry,
                    expectedStartTime,
                    expectedDuration, 
                    expectedDescription,
                    CalendarIconKind.None);
                InteractorFactory.GetDefaultWorkspace().Execute().Returns(Observable.Return(new MockWorkspace(1)));
                observer.Messages.Should().HaveCount(0);
                
                discardAction.MenuItemAction.Inputs.OnNext(newCalendarItem);

                observer.Messages.Should().HaveCount(1);
            }
            
            [Fact]
            public void TheDiscardActionExecutesActionAndClosesMenu()
                => DiscardsChangesAndCloseMenu(TheDiscardActionTriggersTheDiscardChangesObservable);

            [Fact]
            public void TheEditActionNavigatesToTheStartTimeEntryViewModelWithProperParameters()
            {
                var editAction = ContextualMenu.Actions.First(action => action.Kind == CalendarMenuActionKind.Edit);

                var expectedStartTime = new DateTimeOffset(2019, 10, 10, 10, 10, 10, TimeSpan.Zero);
                var expectedDuration = TimeSpan.FromMinutes(30);
                var newCalendarItem = new CalendarItem(
                    string.Empty,
                    CalendarItemSource.TimeEntry,
                    expectedStartTime,
                    expectedDuration, 
                    string.Empty,
                    CalendarIconKind.None);
                InteractorFactory.GetDefaultWorkspace().Execute().Returns(Observable.Return(new MockWorkspace(1)));
                var view = Substitute.For<IView>();
                ViewModel.AttachView(view);
                
                editAction.MenuItemAction.Inputs.OnNext(newCalendarItem);

                var startTimeEntryArg = Arg.Is<StartTimeEntryParameters>(param =>
                    param.StartTime == expectedStartTime
                    && param.Duration == expectedDuration
                    && param.EntryDescription == string.Empty
                    && param.WorkspaceId == 1
                );
                
                NavigationService.Received().Navigate<StartTimeEntryViewModel, StartTimeEntryParameters, Unit>(startTimeEntryArg, view);
            }
            
            [Fact]
            public void TheEditActionExecutesActionAndClosesMenu()
                => ExecutesActionAndClosesMenu(TheEditActionNavigatesToTheStartTimeEntryViewModelWithProperParameters);

            [Fact]
            public void TheSaveActionCreatesATimeEntryWithNoDescriptionWithTheRightStartTimeAndDuration()
            {
                var saveAction = ContextualMenu.Actions.First(action => action.Kind == CalendarMenuActionKind.Save);
                var expectedStartTime = new DateTimeOffset(2019, 10, 10, 10, 10, 10, TimeSpan.Zero);
                var expectedDuration = TimeSpan.FromMinutes(30);
                var expectedDescription = "whatever";
                var newCalendarItem = new CalendarItem(
                    string.Empty,
                    CalendarItemSource.TimeEntry,
                    expectedStartTime,
                    expectedDuration, 
                    expectedDescription,
                    CalendarIconKind.None);
                InteractorFactory.GetDefaultWorkspace().Execute().Returns(Observable.Return(new MockWorkspace(1)));
                
                saveAction.MenuItemAction.Inputs.OnNext(newCalendarItem);
                
                var prototypeArg = Arg.Is<ITimeEntryPrototype>(te =>
                    te.Description == expectedDescription
                    && te.StartTime == expectedStartTime
                    && te.Duration == expectedDuration
                    && te.WorkspaceId == 1
                );
                InteractorFactory.Received().CreateTimeEntry(prototypeArg, TimeEntryStartOrigin.CalendarEvent);
            }
            
            [Fact]
            public void TheSaveActionExecutesActionAndClosesMenu()
                => ExecutesActionAndClosesMenu(TheSaveActionCreatesATimeEntryWithNoDescriptionWithTheRightStartTimeAndDuration);
            
            [Fact]
            public override void TheDismissActionDiscardChangesAndClosesTheMenu()
            {
                base.TheDismissActionDiscardChangesAndClosesTheMenu();
            }
        }

        public sealed class TheMenuForRunningEntries : TheMenuActionTests
        {
            public override CalendarItem CreateMenuTypeCalendarItemTrigger()
                => CreateDummyTimeEntryCalendarItem(isRunning: true);

            [Fact]
            public void TheDiscardActionDeletesTheRunningTimeEntry()
            {
                var discardAction = ContextualMenu.Actions.First(action => action.Kind == CalendarMenuActionKind.Discard);
                var runningTimeEntryId = 10;
                var runningTimeEntry = CreateDummyTimeEntryCalendarItem(isRunning: true, timeEntryId: runningTimeEntryId);
                
                discardAction.MenuItemAction.Inputs.OnNext(runningTimeEntry);

                InteractorFactory.Received().DeleteTimeEntry(runningTimeEntryId);
            }
            
            [Fact]
            public void TheDiscardActionExecutesActionAndClosesMenu()
                => ExecutesActionAndClosesMenu(TheDiscardActionDeletesTheRunningTimeEntry);
            
            [Fact]
            public void TheEditActionNavigatesToTheEditTimeEntryViewModelWithTheRightId()
            {
                var editAction = ContextualMenu.Actions.First(action => action.Kind == CalendarMenuActionKind.Edit);
                var runningTimeEntryId = 10L;
                var runningTimeEntry = CreateDummyTimeEntryCalendarItem(isRunning: true, timeEntryId: runningTimeEntryId);
                var view = Substitute.For<IView>();
                ViewModel.AttachView(view);
                
                editAction.MenuItemAction.Inputs.OnNext(runningTimeEntry);

                var idArg = Arg.Is<long[]>(ids => ids[0] == runningTimeEntryId);
                NavigationService.Received().Navigate<EditTimeEntryViewModel, long[], Unit>(idArg, view);
            }
            
            [Fact]
            public void TheEditActionExecutesActionAndClosesMenu()
                => ExecutesActionAndClosesMenu(TheEditActionNavigatesToTheEditTimeEntryViewModelWithTheRightId);
            
            [Fact]
            public void TheSaveActionUpdatesTheRunningTimeEntry()
            {
                var saveAction = ContextualMenu.Actions.First(action => action.Kind == CalendarMenuActionKind.Save);
                var runningTimeEntryId = 10L;
                var newStartTime = new DateTimeOffset(2019, 10, 10, 10, 10, 10, TimeSpan.Zero);
                var runningTimeEntry = CreateDummyTimeEntryCalendarItem(isRunning: true, timeEntryId: runningTimeEntryId, newStartTime);
                var originalStartTime = new DateTimeOffset(2019, 10, 10, 11, 10, 10, TimeSpan.Zero);
                var mockWorkspace = new MockWorkspace(1);
                var timeEntryMock = new MockTimeEntry(runningTimeEntryId, mockWorkspace, originalStartTime);
                InteractorFactory.GetTimeEntryById(runningTimeEntryId).Execute().Returns(Observable.Return(timeEntryMock));
                
                saveAction.MenuItemAction.Inputs.OnNext(runningTimeEntry);

                var dtoArg = Arg.Is<EditTimeEntryDto>(dto 
                    => dto.Id == runningTimeEntryId
                       && dto.StartTime == newStartTime
                       && dto.StartTime != originalStartTime
                       && dto.WorkspaceId == mockWorkspace.Id
                       && !dto.StopTime.HasValue);
                InteractorFactory.Received().UpdateTimeEntry(dtoArg);
            }
            
            [Fact]
            public void TheSaveActionExecutesActionAndClosesMenu()
                => ExecutesActionAndClosesMenu(TheSaveActionUpdatesTheRunningTimeEntry);
            
            [Fact]
            public void TheStopActionStopsTheRunningTimeEntry()
            {
                var stopAction = ContextualMenu.Actions.First(action => action.Kind == CalendarMenuActionKind.Stop);
                var runningTimeEntryId = 10L;
                var runningTimeEntry = CreateDummyTimeEntryCalendarItem(isRunning: true, timeEntryId: runningTimeEntryId);
                var now = DateTimeOffset.Now;
                TimeService.CurrentDateTime.Returns(now);
                
                stopAction.MenuItemAction.Inputs.OnNext(runningTimeEntry);

                InteractorFactory.Received().StopTimeEntry(now, TimeEntryStopOrigin.CalendarContextualMenu);
            }
            
            [Fact]
            public void TheStopActionExecutesActionAndClosesMenu()
                => ExecutesActionAndClosesMenu(TheStopActionStopsTheRunningTimeEntry);
            
            [Fact]
            public override void TheDismissActionDiscardChangesAndClosesTheMenu()
            {
                base.TheDismissActionDiscardChangesAndClosesTheMenu();
            }
        }
        
        public sealed class TheMenuForStoppedEntries : TheMenuActionTests
        {
            public override CalendarItem CreateMenuTypeCalendarItemTrigger()
                => CreateDummyTimeEntryCalendarItem(isRunning: false);

            [Fact]
            public void TheDeleteActionDeletesTheRunningTimeEntry()
            {
                var deleteAction = ContextualMenu.Actions.First(action => action.Kind == CalendarMenuActionKind.Delete);
                var runningTimeEntryId = 10;
                var runningTimeEntry = CreateDummyTimeEntryCalendarItem(isRunning: false, timeEntryId: runningTimeEntryId);
                
                deleteAction.MenuItemAction.Inputs.OnNext(runningTimeEntry);

                InteractorFactory.Received().DeleteTimeEntry(runningTimeEntryId);
            }
            
            [Fact]
            public void TheDeleteActionExecutesActionAndClosesMenu()
                => ExecutesActionAndClosesMenu(TheDeleteActionDeletesTheRunningTimeEntry);
            
            [Fact]
            public void TheEditActionNavigatesToTheEditTimeEntryViewModelWithTheRightId()
            {
                var editAction = ContextualMenu.Actions.First(action => action.Kind == CalendarMenuActionKind.Edit);
                var runningTimeEntryId = 10L;
                var runningTimeEntry = CreateDummyTimeEntryCalendarItem(isRunning: false, timeEntryId: runningTimeEntryId);
                var view = Substitute.For<IView>();
                ViewModel.AttachView(view);
                
                editAction.MenuItemAction.Inputs.OnNext(runningTimeEntry);

                var idArg = Arg.Is<long[]>(ids => ids[0] == runningTimeEntryId);
                NavigationService.Received().Navigate<EditTimeEntryViewModel, long[], Unit>(idArg, view);
            }
            
            [Fact]
            public void TheEditActionExecutesActionAndClosesMenu()
                => ExecutesActionAndClosesMenu(TheEditActionNavigatesToTheEditTimeEntryViewModelWithTheRightId);
            
            [Fact]
            public void TheSaveActionUpdatesTheRunningTimeEntry()
            {
                var saveAction = ContextualMenu.Actions.First(action => action.Kind == CalendarMenuActionKind.Save);
                var runningTimeEntryId = 10L;
                var newStartTime = new DateTimeOffset(2019, 10, 10, 10, 10, 10, TimeSpan.Zero);
                var newEndTime = new DateTimeOffset(2019, 10, 10, 10, 30, 10, TimeSpan.Zero);
                var runningTimeEntry = CreateDummyTimeEntryCalendarItem(isRunning: false, timeEntryId: runningTimeEntryId, newStartTime, newEndTime - newStartTime);
                var originalStartTime = new DateTimeOffset(2019, 10, 10, 11, 10, 10, TimeSpan.Zero);
                var originalEndTime = new DateTimeOffset(2019, 10, 10, 11, 30, 10, TimeSpan.Zero);
                var mockWorkspace = new MockWorkspace(1);
                var timeEntryMock = new MockTimeEntry(runningTimeEntryId, mockWorkspace, originalStartTime, (long)(originalEndTime - originalStartTime).TotalSeconds);
                InteractorFactory.GetTimeEntryById(runningTimeEntryId).Execute().Returns(Observable.Return(timeEntryMock));
                
                saveAction.MenuItemAction.Inputs.OnNext(runningTimeEntry);

                var dtoArg = Arg.Is<EditTimeEntryDto>(dto 
                    => dto.Id == runningTimeEntryId
                       && dto.StartTime == newStartTime
                       && dto.StartTime != originalStartTime
                       && dto.WorkspaceId == mockWorkspace.Id
                       && dto.StopTime.HasValue
                       && dto.StopTime.Value == newEndTime
                       && dto.StopTime.Value != originalEndTime);
                InteractorFactory.Received().UpdateTimeEntry(dtoArg);
            }
            
            [Fact]
            public void TheSaveActionExecutesActionAndClosesMenu()
                => ExecutesActionAndClosesMenu(TheSaveActionUpdatesTheRunningTimeEntry);
            
            [Fact]
            public void TheContinueActionStartsANewTheRunningTimeEntryWithTheDetailsFromTheCalendarItemCalledFromTheMenuAction()
            {
                var continueAction = ContextualMenu.Actions.First(action => action.Kind == CalendarMenuActionKind.Continue);
                var runningTimeEntryId = 10L;
                var runningTimeEntry = CreateDummyTimeEntryCalendarItem(isRunning: false, timeEntryId: runningTimeEntryId);
                var mockWorkspace = new MockWorkspace(1);
                var mockProject = new MockProject(1, mockWorkspace);
                var mockTask = new MockTask(1, mockWorkspace, mockProject);
                var expectedTimeEntryToContinue = Substitute.For<IThreadSafeTimeEntry>();
                expectedTimeEntryToContinue.WorkspaceId.Returns(1);
                expectedTimeEntryToContinue.Description.Returns("");
                expectedTimeEntryToContinue.Duration.Returns(100);
                expectedTimeEntryToContinue.Start.Returns(runningTimeEntry.StartTime);
                expectedTimeEntryToContinue.Project.Returns(mockProject);
                expectedTimeEntryToContinue.Task.Returns(mockTask);
                expectedTimeEntryToContinue.TagIds.Returns(Array.Empty<long>());
                expectedTimeEntryToContinue.Billable.Returns(false);
                InteractorFactory.GetTimeEntryById(runningTimeEntryId).Execute().Returns(Observable.Return(expectedTimeEntryToContinue));
                
                continueAction.MenuItemAction.Inputs.OnNext(runningTimeEntry);

                var continuePrototype = Arg.Is<ITimeEntryPrototype>(prot =>
                    prot.WorkspaceId == mockWorkspace.Id);
                
                InteractorFactory.Received().ContinueTimeEntry(continuePrototype, ContinueTimeEntryMode.CalendarContextualMenu);
            }
            
            [Fact]
            public void TheContinueActionExecutesActionAndClosesMenu()
                => ExecutesActionAndClosesMenu(TheContinueActionStartsANewTheRunningTimeEntryWithTheDetailsFromTheCalendarItemCalledFromTheMenuAction);
            
            [Fact]
            public override void TheDismissActionDiscardChangesAndClosesTheMenu()
            {
                base.TheDismissActionDiscardChangesAndClosesTheMenu();
            }
        }

        public sealed class TheTimeEntryInfoObservable : CalendarContextualMenuViewModelTest
        {
            [Fact]
            public void StartsWithTheTimeEntryInfoFromPassedThroughOnItemSelected()
            {
                var observer = TestScheduler.CreateObserver<TimeEntryDisplayInfo>();
                var calendarItem = new CalendarItem(
                    "",
                    CalendarItemSource.TimeEntry,
                    DateTimeOffset.Now,
                    TimeSpan.FromMinutes(30), 
                    "Such description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "Such Project",
                    task: "Such Task",
                    client: "Such Client");
                ViewModel.TimeEntryInfo.Subscribe(observer);
                TestScheduler.Start();
                
                ViewModel.OnItemSelected.Execute(calendarItem);
                TestScheduler.Start();

                observer.Messages.First().Value.Value
                    .Should()
                    .Match<TimeEntryDisplayInfo>(e => 
                        e.Description == "Such description"
                        && e.ProjectTaskColor == "#c2c2c2"
                        && e.Project == "Such Project"
                        && e.Task == "Such Task"
                        && e.Client == "Such Client");
            }
            
            [Fact]
            public void EmitsWhenItemIsUpdatedWithNewDetails()
            {
                var observer = TestScheduler.CreateObserver<TimeEntryDisplayInfo>();
                var startingCalendarItem = new CalendarItem(
                    "",
                    CalendarItemSource.TimeEntry,
                    DateTimeOffset.Now,
                    TimeSpan.FromMinutes(30), 
                    "Old description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "Old Project",
                    task: "Old Task",
                    client: "Old Client");
                
                var updatedCalendarItem = new CalendarItem(
                    "",
                    CalendarItemSource.TimeEntry,
                    DateTimeOffset.Now,
                    TimeSpan.FromMinutes(30), 
                    "New description",
                    CalendarIconKind.None,
                    "#f2f2f2",
                    project: "New Project",
                    task: "New Task",
                    client: "New Client");
                ViewModel.TimeEntryInfo.Subscribe(observer);
                TestScheduler.Start();
                ViewModel.OnItemSelected.Execute(startingCalendarItem);
                
                ViewModel.OnItemUpdated.Execute(updatedCalendarItem);
                TestScheduler.Start();

                observer.Messages[0].Value.Value
                    .Should()
                    .Match<TimeEntryDisplayInfo>(e => 
                        e.Description == "Old description"
                        && e.ProjectTaskColor == "#c2c2c2"
                        && e.Project == "Old Project"
                        && e.Task == "Old Task"
                        && e.Client == "Old Client");
                
                observer.Messages[1].Value.Value
                    .Should()
                    .Match<TimeEntryDisplayInfo>(e => 
                        e.Description == "New description"
                        && e.ProjectTaskColor == "#f2f2f2"
                        && e.Project == "New Project"
                        && e.Task == "New Task"
                        && e.Client == "New Client");
            }

            [Fact]
            public void DoesNotEmitWhenItemIsUpdatedWithTheSameDetailsEvenIfStartTimeAndDurationAreDifferent()
            {
                var observer = TestScheduler.CreateObserver<TimeEntryDisplayInfo>();
                var startingCalendarItem = new CalendarItem(
                    "",
                    CalendarItemSource.TimeEntry,
                    DateTimeOffset.Now,
                    TimeSpan.FromMinutes(30), 
                    "description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "project",
                    task: "task",
                    client: "client");
                
                var updatedCalendarItem = new CalendarItem(
                    "",
                    CalendarItemSource.TimeEntry,
                    DateTimeOffset.Now.AddHours(1),
                    TimeSpan.FromMinutes(25), 
                    "description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "project",
                    task: "task",
                    client: "client");
                ViewModel.TimeEntryInfo.Subscribe(observer);
                TestScheduler.Start();
                ViewModel.OnItemSelected.Execute(startingCalendarItem);
                
                ViewModel.OnItemUpdated.Execute(updatedCalendarItem);
                TestScheduler.Start();

                observer.Messages.Should().HaveCount(1);
                observer.Messages[0].Value.Value
                    .Should()
                    .Match<TimeEntryDisplayInfo>(e => 
                        e.Description == "description"
                        && e.ProjectTaskColor == "#c2c2c2"
                        && e.Project == "project"
                        && e.Task == "task"
                        && e.Client == "client");
            }
            
            [Fact]
            public void DoesNotEmitAnotherContextualMenuWhenTheItemIsUpdated()
            {
                var observer = TestScheduler.CreateObserver<CalendarContextualMenu>();
                var startingCalendarItem = new CalendarItem("", CalendarItemSource.TimeEntry, DateTimeOffset.Now, TimeSpan.FromMinutes(30), "Old description", CalendarIconKind.None, "#c2c2c2", project: "Old Project", task: "Old Task", client: "Old Client");
                var updatedCalendarItem = new CalendarItem("", CalendarItemSource.TimeEntry, DateTimeOffset.Now, TimeSpan.FromMinutes(30), "New description", CalendarIconKind.None, "#f2f2f2", project: "New Project", task: "New Task", client: "New Client");
                TestScheduler.Start();
                ViewModel.OnItemSelected.Execute(startingCalendarItem);
                ViewModel.CurrentMenu.Subscribe(observer);
                
                ViewModel.OnItemUpdated.Execute(updatedCalendarItem);
                TestScheduler.Start();

                observer.Messages.Should().HaveCount(1);
            }
        }

        public sealed class TheTimeEntryPeriodObservable : CalendarContextualMenuViewModelTest
        {
            [Fact]
            public void StartsWithThePeriodFromCalendarItemPassedThroughOnItemSelected()
            {
                var observer = TestScheduler.CreateObserver<string>();
                var startTime = new DateTimeOffset(2019, 10, 10, 10, 10, 0, TimeSpan.Zero);
                var duration = TimeSpan.FromMinutes(30);
                var calendarItem = new CalendarItem(
                    "",
                    CalendarItemSource.TimeEntry,
                    startTime,
                    duration, 
                    "Such description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "Such Project",
                    task: "Such Task",
                    client: "Such Client");
                ViewModel.TimeEntryPeriod.Subscribe(observer);
                TestScheduler.Start();
                
                ViewModel.OnItemSelected.Execute(calendarItem);
                TestScheduler.Start();

                observer.Messages.Should().HaveCount(1);
                observer.Messages.First().Value.Value
                    .Should()
                    .Be("10:10 AM - 10:40 AM");
            }
            
            [Fact]
            public void EmitsWhenItemIsUpdatedWithADifferentStartTime()
            {
                var observer = TestScheduler.CreateObserver<string>();
                var startTime = new DateTimeOffset(2019, 10, 10, 10, 10, 0, TimeSpan.Zero);
                var duration = TimeSpan.FromMinutes(30);
                var calendarItem = new CalendarItem(
                    "",
                    CalendarItemSource.TimeEntry,
                    startTime,
                    duration, 
                    "Such description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "Such Project",
                    task: "Such Task",
                    client: "Such Client");
                ViewModel.TimeEntryPeriod.Subscribe(observer);
                ViewModel.OnItemSelected.Execute(calendarItem);
                var newStartTime = startTime.AddHours(1);
                var newCalendarItem = calendarItem.WithStartTime(newStartTime);
                TestScheduler.Start();
                
                ViewModel.OnItemUpdated.Execute(newCalendarItem);
                TestScheduler.Start();

                observer.Messages.Should().HaveCount(2);
                observer.Messages.Last().Value.Value
                    .Should()
                    .Be("11:10 AM - 11:40 AM");
            }
            
            [Fact]
            public void EmitsWhenItemIsUpdatedWithADifferentDuration()
            {
                var observer = TestScheduler.CreateObserver<string>();
                var startTime = new DateTimeOffset(2019, 10, 10, 10, 10, 0, TimeSpan.Zero);
                var duration = TimeSpan.FromMinutes(30);
                var calendarItem = new CalendarItem(
                    "",
                    CalendarItemSource.TimeEntry,
                    startTime,
                    duration, 
                    "Such description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "Such Project",
                    task: "Such Task",
                    client: "Such Client");
                ViewModel.TimeEntryPeriod.Subscribe(observer);
                ViewModel.OnItemSelected.Execute(calendarItem);
                var newDuration = TimeSpan.FromMinutes(10);
                var newCalendarItem = calendarItem.WithDuration(newDuration);
                TestScheduler.Start();
                
                ViewModel.OnItemUpdated.Execute(newCalendarItem);
                TestScheduler.Start();

                observer.Messages.Should().HaveCount(2);
                observer.Messages.Last().Value.Value
                    .Should()
                    .Be("10:10 AM - 10:20 AM");
            }
            
            [Fact]
            public void DoesNotEmitWhenItemIsUpdatedWithSameStartTimeAndDuration()
            {
                var observer = TestScheduler.CreateObserver<string>();
                var startTime = new DateTimeOffset(2019, 10, 10, 10, 10, 0, TimeSpan.Zero);
                var duration = TimeSpan.FromMinutes(30);
                var calendarItem = new CalendarItem(
                    "",
                    CalendarItemSource.TimeEntry,
                    startTime,
                    duration, 
                    "Such description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "Such Project",
                    task: "Such Task",
                    client: "Such Client");
                var newCalendarItem = new CalendarItem(
                    "",
                    CalendarItemSource.TimeEntry,
                    startTime,
                    duration, 
                    "New description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "New Project",
                    task: "New Task",
                    client: "New Client");
                ViewModel.TimeEntryPeriod.Subscribe(observer);
                ViewModel.OnItemSelected.Execute(calendarItem);
                TestScheduler.Start();
                
                ViewModel.OnItemUpdated.Execute(newCalendarItem);
                TestScheduler.Start();

                observer.Messages.Should().HaveCount(1);
            }
            
            [Fact]
            public void EmitsEndTimeAsNowWhenItemIsUpdatedWithNullDuration()
            {
                var observer = TestScheduler.CreateObserver<string>();
                var startTime = new DateTimeOffset(2019, 10, 10, 10, 10, 0, TimeSpan.Zero);
                var duration = TimeSpan.FromMinutes(30);
                var calendarItem = new CalendarItem(
                    "",
                    CalendarItemSource.TimeEntry,
                    startTime,
                    duration, 
                    "Such description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "Such Project",
                    task: "Such Task",
                    client: "Such Client");
                ViewModel.TimeEntryPeriod.Subscribe(observer);
                ViewModel.OnItemSelected.Execute(calendarItem);
                
                var newCalendarItem = calendarItem.WithDuration(null);
                TestScheduler.Start();
                
                ViewModel.OnItemUpdated.Execute(newCalendarItem);
                TestScheduler.Start();

                observer.Messages.Should().HaveCount(2);
                observer.Messages.Last().Value.Value
                    .Should()
                    .Be($"10:10 AM - {Shared.Resources.Now}");
            }
        }
    }
}