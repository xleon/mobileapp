using System;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.Calendar;
using Toggl.Core.Exceptions;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Services;
using Toggl.Shared;

namespace Toggl.Core.Suggestions
{
    public sealed class CalendarSuggestionProvider : ISuggestionProvider
    {
        private readonly ITimeService timeService;
        private readonly ICalendarService calendarService;
        private readonly IInteractor<IObservable<IThreadSafeWorkspace>> defaultWorkspaceInteractor;

        private readonly TimeSpan lookBackTimeSpan = TimeSpan.FromHours(1);
        private readonly TimeSpan lookAheadTimeSpan = TimeSpan.FromHours(1);

        public CalendarSuggestionProvider(
            ITimeService timeService,
            ICalendarService calendarService,
            IInteractor<IObservable<IThreadSafeWorkspace>> defaultWorkspaceInteractor)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(calendarService, nameof(calendarService));
            Ensure.Argument.IsNotNull(defaultWorkspaceInteractor, nameof(defaultWorkspaceInteractor));

            this.timeService = timeService;
            this.calendarService = calendarService;
            this.defaultWorkspaceInteractor = defaultWorkspaceInteractor;
        }

        public IObservable<Suggestion> GetSuggestions()
        {
            var now = timeService.CurrentDateTime;
            var startOfRange = now - lookBackTimeSpan;
            var endOfRange = now + lookAheadTimeSpan;

            var eventsObservable = calendarService
                .GetEventsInRange(startOfRange, endOfRange)
                .SelectMany(events => events.Where(eventHasDescription));

            return defaultWorkspaceInteractor.Execute()
                .CombineLatest(
                    eventsObservable,
                    (workspace, calendarItem) => suggestionFromEvent(calendarItem, workspace.Id))
                .Catch((NotAuthorizedException _) => Observable.Empty<Suggestion>());
        }

        private Suggestion suggestionFromEvent(CalendarItem calendarItem, long workspaceId)
            => new Suggestion(calendarItem, workspaceId);

        private bool eventHasDescription(CalendarItem calendarItem)
            => !string.IsNullOrEmpty(calendarItem.Description);
    }
}
