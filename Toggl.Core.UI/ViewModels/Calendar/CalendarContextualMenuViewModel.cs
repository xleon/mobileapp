using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.Calendar;
using Toggl.Core.Extensions;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Services;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Xamarin.Essentials;

namespace Toggl.Core.UI.ViewModels.Calendar
{
    public class CalendarContextualMenuViewModel : ViewModel
    {
        private readonly ISubject<CalendarContextualMenu> currentMenuSubject;
        private readonly Dictionary<ContextualMenuType, CalendarContextualMenu> contextualMenus;
        private readonly ISubject<Unit> discardChangesSubject = new Subject<Unit>();
        private readonly ISubject<bool> menuVisibilitySubject = new BehaviorSubject<bool>(false);
        private readonly ISubject<TimeEntryDisplayInfo> timeEntryInfoSubject = new Subject<TimeEntryDisplayInfo>();
        private readonly ISubject<string> timeEntryPeriodSubject = new Subject<string>();

        private readonly IInteractorFactory interactorFactory;
        private readonly IAnalyticsService analyticsService;
        private readonly IRxActionFactory rxActionFactory;
        private readonly ITimeService timeService;

        private TimeEntryDisplayInfo currentTimeEntryDisplayInfo;
        private DateTimeOffset currentStartTimeOffset;
        private TimeSpan? currentDuration;

        public IObservable<CalendarContextualMenu> CurrentMenu
            => currentMenuSubject.AsObservable();

        public IObservable<Unit> DiscardChanges 
            => discardChangesSubject.AsObservable();

        public IObservable<bool> MenuVisible
            => menuVisibilitySubject.AsObservable();

        public IObservable<TimeEntryDisplayInfo> TimeEntryInfo
            => timeEntryInfoSubject.AsObservable();

        public IObservable<string> TimeEntryPeriod
            => timeEntryPeriodSubject.AsObservable();

        public InputAction<CalendarItem> OnItemSelected { get; }
        public InputAction<CalendarItem> OnItemUpdated { get; }

        public CalendarContextualMenuViewModel(
            IInteractorFactory interactorFactory,
            IAnalyticsService analyticsService,
            IRxActionFactory rxActionFactory,
            ITimeService timeService,
            INavigationService navigationService)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            this.interactorFactory = interactorFactory;
            this.analyticsService = analyticsService;
            this.rxActionFactory = rxActionFactory;
            this.timeService = timeService;

            OnItemSelected = rxActionFactory.FromAction<CalendarItem>(handleCalendarItemSelection);
            OnItemUpdated = rxActionFactory.FromAction<CalendarItem>(handleCalendarItemUpdate);

            var closedMenu = new CalendarContextualMenu(ImmutableList<CalendarMenuAction>.Empty, rxActionFactory.FromAction(CommonFunctions.DoNothing));
            contextualMenus = new Dictionary<ContextualMenuType, CalendarContextualMenu>
            {
                { ContextualMenuType.CalendarEvent, setupCalendarEventActions() },
                { ContextualMenuType.RunningTimeEntry, setupRunningTimeEntryActions() },
                { ContextualMenuType.StoppedTimeEntry, setupStoppedTimeEntryActions() },
                { ContextualMenuType.NewTimeEntry, setupNewTimeEntryContextualActions() },
                { ContextualMenuType.Closed, closedMenu }
            };
            
            currentMenuSubject = new BehaviorSubject<CalendarContextualMenu>(closedMenu);
        }

        private void handleCalendarItemSelection(CalendarItem calendarItem)
        {
            if (!contextualMenus.TryGetValue(selectContextualMenuTypeFrom(calendarItem), out var actions))
                return;
            
            currentMenuSubject.OnNext(actions);
            menuVisibilitySubject.OnNext(true);
            handleCalendarItemUpdate(calendarItem);
        }

        private void handleCalendarItemUpdate(CalendarItem calendarItem)
        {
            if (!hasSameTimeEntryDisplayInfoAs(calendarItem))
            {
                currentTimeEntryDisplayInfo = new TimeEntryDisplayInfo(calendarItem);
                timeEntryInfoSubject.OnNext(currentTimeEntryDisplayInfo);    
            }

            if (calendarItem.StartTime != currentStartTimeOffset || calendarItem.Duration != currentDuration)
            {
                currentStartTimeOffset = calendarItem.StartTime;
                currentDuration = calendarItem.Duration;
                timeEntryPeriodSubject.OnNext(formatCurrentPeriod());
            }
        }

        private string formatCurrentPeriod()
        {
            var startTimeString = currentStartTimeOffset.ToString(Resources.EditingTwelveHoursFormat);
            var endTime = currentStartTimeOffset + currentDuration;
            var endTimeString = endTime.HasValue
                ? endTime.Value.ToString(Resources.EditingTwelveHoursFormat)
                : Resources.Now;

            return $"{startTimeString} - {endTimeString}";
        }
        
        private void closeMenuDismissingUncommittedChanges()
        {
            currentMenuSubject.OnNext(contextualMenus[ContextualMenuType.Closed]);
            discardChangesSubject.OnNext(Unit.Default);
            menuVisibilitySubject.OnNext(false);
        }

        private void closeMenuWithCommittedChanges()
        {
            currentMenuSubject.OnNext(contextualMenus[ContextualMenuType.Closed]);
            menuVisibilitySubject.OnNext(false);
        }

        private CalendarContextualMenu setupCalendarEventActions()
        {
            var actions = ImmutableList.Create(
                createCalendarMenuActionFor(CalendarMenuActionKind.Copy, Resources.CalendarCopyEventToTimeEntry, 
                    trackThenRunCalendarEventCreationAsync(
                        CalendarTimeEntryCreatedType.CopyFromCalendarEvent, 
                        CalendarContextualMenuActionType.CopyAsTimeEntry, 
                        createTimeEntryFromCalendarItem)
                    ),
                createCalendarMenuActionFor(CalendarMenuActionKind.Start, Resources.Start, 
                    trackThenRunCalendarEventCreationAsync(
                        CalendarTimeEntryCreatedType.StartFromCalendarEvent, 
                        CalendarContextualMenuActionType.StartFromCalendarEvent, 
                        startTimeEntryFromCalendarItem)
                    ));
            
            return new CalendarContextualMenu(actions, trackThenDismiss(analyticsService.CalendarEventContextualMenu));
        }

        private void trackCalendarEventCreation(CalendarItem item, CalendarTimeEntryCreatedType eventCreationType, CalendarContextualMenuActionType menuType)
        {
            var today = timeService.CurrentDateTime;
            var daysSinceToday = (int)(item.StartTime - today).TotalDays;
            var dayOfTheWeek = item.StartTime.DayOfWeek;
            analyticsService.CalendarEventContextualMenu.Track(menuType);
            analyticsService.CalendarTimeEntryCreated.Track(eventCreationType, daysSinceToday, dayOfTheWeek.ToString());
        }

        private async Task createTimeEntryFromCalendarItem(CalendarItem calendarItem)
        {
            var workspace = await interactorFactory.GetDefaultWorkspace()
                .TrackException<InvalidOperationException, IThreadSafeWorkspace>("CalendarContextualMenuViewModel.createTimeEntryFromCalendarItem")
                .Execute();
            var prototype = calendarItem.AsTimeEntryPrototype(workspace.Id);
            await interactorFactory.CreateTimeEntry(prototype, TimeEntryStartOrigin.CalendarEvent).Execute();
            closeMenuWithCommittedChanges();
        }
        
        private async Task startTimeEntryFromCalendarItem(CalendarItem calendarItem)
        {
            var timeEntryToStart = calendarItem
                    .WithStartTime(timeService.CurrentDateTime)
                    .WithDuration(null);
            
            var workspace = await interactorFactory.GetDefaultWorkspace()
                .TrackException<InvalidOperationException, IThreadSafeWorkspace>("CalendarContextualMenuViewModel.startTimeEntryFromCalendarItem")
                .Execute();
            var prototype = timeEntryToStart.AsTimeEntryPrototype(workspace.Id);
            await interactorFactory.CreateTimeEntry(prototype, TimeEntryStartOrigin.CalendarEvent).Execute();
            closeMenuWithCommittedChanges();
        }

        private CalendarContextualMenu setupRunningTimeEntryActions()
        {
            var analyticsEvent = analyticsService.CalendarRunningTimeEntryContextualMenu;
            var actions = ImmutableList.Create(
                createCalendarMenuDiscardAction(trackThenRunAsync(analyticsEvent, CalendarContextualMenuActionType.Discard, deleteTimeEntry)),
                createCalendarMenuEditAction(trackThenRunAsync(analyticsEvent, CalendarContextualMenuActionType.Edit, editTimeEntry)),
                createCalendarMenuSaveAction(trackThenRunAsync(analyticsEvent, CalendarContextualMenuActionType.Save, saveTimeEntry)),
                createCalendarMenuActionFor(CalendarMenuActionKind.Stop, Resources.Stop, trackThenRunAsync(analyticsEvent, CalendarContextualMenuActionType.Stop, stopTimeEntry))
            );
            
            return new CalendarContextualMenu(actions, trackThenDismiss(analyticsEvent));
        }

        private async Task deleteTimeEntry(CalendarItem calendarItem)
        {
            if (!calendarItem.TimeEntryId.HasValue)
                return;
            
            await interactorFactory.DeleteTimeEntry(calendarItem.TimeEntryId.Value).Execute();
            closeMenuWithCommittedChanges();
        }

        private async Task editTimeEntry(CalendarItem calendarItem)
        {
            if (!calendarItem.TimeEntryId.HasValue)
                return;
            
            analyticsService.EditViewOpenedFromCalendar.Track();
            await Navigate<EditTimeEntryViewModel, long[]>(new[] { calendarItem.TimeEntryId.Value });    
            closeMenuWithCommittedChanges();
        }

        private async Task saveTimeEntry(CalendarItem calendarItem)
        {
            if (!calendarItem.TimeEntryId.HasValue)
                return;
            
            var timeEntry = await interactorFactory.GetTimeEntryById(calendarItem.TimeEntryId.Value).Execute();

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
            closeMenuWithCommittedChanges();
        }

        private async Task stopTimeEntry(CalendarItem calendarItem)
        {
            var currentDateTime = timeService.CurrentDateTime;
            await interactorFactory.StopTimeEntry(currentDateTime, TimeEntryStopOrigin.CalendarContextualMenu).Execute();
            closeMenuWithCommittedChanges();
        }

        private CalendarContextualMenu setupStoppedTimeEntryActions()
        {
            var analyticsEvent = analyticsService.CalendarExistingTimeEntryContextualMenu;
            var actions = ImmutableList.Create(
                createCalendarMenuActionFor(CalendarMenuActionKind.Delete, Resources.Delete, trackThenRunAsync(analyticsEvent, CalendarContextualMenuActionType.Delete, deleteTimeEntry)),
                createCalendarMenuEditAction(trackThenRunAsync(analyticsEvent, CalendarContextualMenuActionType.Edit, editTimeEntry)),
                createCalendarMenuSaveAction(trackThenRunAsync(analyticsEvent, CalendarContextualMenuActionType.Save, saveTimeEntry)),
                createCalendarMenuActionFor(CalendarMenuActionKind.Continue, Resources.Continue, trackThenRunAsync(analyticsEvent, CalendarContextualMenuActionType.Continue, continueTimeEntry))
            );
            
            return new CalendarContextualMenu(actions, trackThenDismiss(analyticsEvent));
        }

        private async Task continueTimeEntry(CalendarItem calendarItem)
        {
            if (!calendarItem.TimeEntryId.HasValue) 
                return;
            
            var timeEntry = await interactorFactory.GetTimeEntryById(calendarItem.TimeEntryId.Value).Execute();
            
            var prototype = timeEntry.AsTimeEntryPrototype();
            await interactorFactory.ContinueTimeEntry(prototype, ContinueTimeEntryMode.CalendarContextualMenu).Execute();
            closeMenuWithCommittedChanges();
        }

        private CalendarContextualMenu setupNewTimeEntryContextualActions()
        {
            var analyticsEvent = analyticsService.CalendarNewTimeEntryContextualMenu;
            var actions = ImmutableList.Create(
                createCalendarMenuDiscardAction(trackThenRun(analyticsEvent, CalendarContextualMenuActionType.Discard, discardCurrentItemInEditMode)),
                createCalendarMenuEditAction(trackThenRunAsync(analyticsEvent, CalendarContextualMenuActionType.Edit, startTimeEntryFrom)),
                createCalendarMenuSaveAction(trackThenRunAsync(analyticsEvent, CalendarContextualMenuActionType.Save, createTimeEntryFromCalendarItem))
            );
            
            return new CalendarContextualMenu(actions, trackThenDismiss(analyticsEvent));
        }

        private async Task startTimeEntryFrom(CalendarItem calendarItem)
        {
            var workspace = await interactorFactory.GetDefaultWorkspace()
                .TrackException<InvalidOperationException, IThreadSafeWorkspace>("CalendarContextualMenuViewModel.startTimeEntryFromCalendarItem")
                .Execute();
            
            var timeEntryParams = new StartTimeEntryParameters(
                calendarItem.StartTime,
                string.Empty,
                calendarItem.Duration,
                workspace.Id);
            
            await Navigate<StartTimeEntryViewModel, StartTimeEntryParameters>(timeEntryParams);
            closeMenuWithCommittedChanges();
        } 

        private void discardCurrentItemInEditMode(CalendarItem calendarItem)
        {
            closeMenuDismissingUncommittedChanges();
        }
        
        private ContextualMenuType selectContextualMenuTypeFrom(CalendarItem calendarItem)
        {
            if (calendarItemIsANewTimeEntry(calendarItem))
                return ContextualMenuType.NewTimeEntry;
            
            if (calendarItemIsFromAnExistingTimeEntry(calendarItem))
            {
                return calendarItem.Duration.HasValue 
                    ? ContextualMenuType.StoppedTimeEntry 
                    : ContextualMenuType.RunningTimeEntry;
            }

            return ContextualMenuType.CalendarEvent;
        }

        private bool calendarItemIsFromAnExistingTimeEntry(CalendarItem calendarItem)
            => calendarItem.Source == CalendarItemSource.TimeEntry
               && !string.IsNullOrEmpty(calendarItem.Id)
               && calendarItem.TimeEntryId.HasValue;

        private bool calendarItemIsANewTimeEntry(CalendarItem calendarItem)
            => string.IsNullOrEmpty(calendarItem.Id);

        private bool hasSameTimeEntryDisplayInfoAs(CalendarItem calendarItem)
            => calendarItem.Description == currentTimeEntryDisplayInfo.Description
               && calendarItem.Project == currentTimeEntryDisplayInfo.Project
               && calendarItem.Task == currentTimeEntryDisplayInfo.Task
               && calendarItem.Color == currentTimeEntryDisplayInfo.ProjectTaskColor;
        
        private InputAction<CalendarItem> trackThenRun(IAnalyticsEvent<CalendarContextualMenuActionType> analyticsEvent, CalendarContextualMenuActionType eventValue, Action<CalendarItem> action)
            => rxActionFactory.FromAction<CalendarItem>(item =>
            {
                analyticsEvent.Track(eventValue);
                action(item);
            });
        
        private InputAction<CalendarItem> trackThenRunAsync(IAnalyticsEvent<CalendarContextualMenuActionType> analyticsEvent, CalendarContextualMenuActionType eventValue, Func<CalendarItem, Task> action)
            => rxActionFactory.FromAsync<CalendarItem>(item =>
            {
                analyticsEvent.Track(eventValue);
                return action(item);
            });

        private InputAction<CalendarItem> trackThenRunCalendarEventCreationAsync(CalendarTimeEntryCreatedType eventCreationType, CalendarContextualMenuActionType menuType, Func<CalendarItem, Task> action)
            => rxActionFactory.FromAsync<CalendarItem>(item =>
            {
                trackCalendarEventCreation(item, eventCreationType, menuType);
                return action(item);
            });

        private ViewAction trackThenDismiss(IAnalyticsEvent<CalendarContextualMenuActionType> analyticsEvent)
            => rxActionFactory.FromAction(() =>
            {
                analyticsEvent.Track(CalendarContextualMenuActionType.Dismiss);
                closeMenuDismissingUncommittedChanges();
            });

        private CalendarMenuAction createCalendarMenuActionFor(CalendarMenuActionKind calendarMenuActionKind, string title, InputAction<CalendarItem> action) 
            => new CalendarMenuAction
            {
                Kind = calendarMenuActionKind,
                Title = title,
                MenuItemAction = action
            };

        private CalendarMenuAction createCalendarMenuSaveAction(InputAction<CalendarItem> action)
            => createCalendarMenuActionFor(CalendarMenuActionKind.Save, Resources.Save, action);
        
        private CalendarMenuAction createCalendarMenuEditAction(InputAction<CalendarItem> action)
            => createCalendarMenuActionFor(CalendarMenuActionKind.Edit, Resources.Edit, action);
        
        private CalendarMenuAction createCalendarMenuDiscardAction(InputAction<CalendarItem> action)
            => createCalendarMenuActionFor(CalendarMenuActionKind.Discard, Resources.Discard, action);

        public struct CalendarContextualMenu
        {
            public ViewAction Dismiss { get; }
            public ImmutableList<CalendarMenuAction> Actions { get; }

            public CalendarContextualMenu(ImmutableList<CalendarMenuAction> actions, ViewAction dismissAction)
            {
                Actions = actions;
                Dismiss = dismissAction;
            }
        }
        
        public struct CalendarMenuAction
        {
            public string Title;
            public CalendarMenuActionKind Kind;
            public InputAction<CalendarItem> MenuItemAction;
        }
        
        public struct TimeEntryDisplayInfo
        {
            public string Description { get; }
            public string Project { get; }
            public string Task { get; }
            public string Client { get; }
            public string ProjectTaskColor { get; }

            public TimeEntryDisplayInfo(CalendarItem calendarItem)
            {
                Description = calendarItem.Description;
                Project = calendarItem.Project;
                Task = calendarItem.Task;
                Client = calendarItem.Client;
                ProjectTaskColor = calendarItem.Color;
            }
        }

        public enum CalendarMenuActionKind
        {
            Discard,
            Edit,
            Save,
            Delete,
            Copy,
            Start,
            Continue,
            Stop
        }

        private enum ContextualMenuType
        {
            NewTimeEntry,
            StoppedTimeEntry,
            RunningTimeEntry,
            CalendarEvent,
            Closed
        }
    }
}
