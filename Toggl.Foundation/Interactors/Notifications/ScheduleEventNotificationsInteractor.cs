using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Foundation.Calendar;
using Toggl.Foundation.Services;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Notification = Toggl.Multivac.Notification;

namespace Toggl.Foundation.Interactors.Notifications
{
    public sealed class ScheduleEventNotificationsInteractor : IInteractor<IObservable<Unit>>
    {
        private readonly TimeSpan period = TimeSpan.FromDays(7);
        private readonly TimeSpan intervalBeforeEvent = TimeSpan.FromMinutes(10);

        private readonly ITimeService timeService;
        private readonly ICalendarService calendarService;
        private readonly INotificationService notificationService;


        public ScheduleEventNotificationsInteractor(
            ITimeService timeService,
            ICalendarService calendarService,
            INotificationService notificationService)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(calendarService, nameof(calendarService));
            Ensure.Argument.IsNotNull(notificationService, nameof(notificationService));

            this.timeService = timeService;
            this.calendarService = calendarService;
            this.notificationService = notificationService;
        }

        public IObservable<Unit> Execute()
        {
            var start = timeService.CurrentDateTime;
            var end = start.Add(period);

            return calendarService
                .GetEventsInRange(start, end)
                .Select(calendarItems => calendarItems.Select(eventNotification).ToImmutableList())
                .SelectMany(notificationService.Schedule);
        }

        private Notification eventNotification(CalendarItem calendarItem)
            => new Notification(
                calendarItem.Id,
                Resources.EventReminder,
                calendarItem.Description,
                calendarItem.StartTime - intervalBeforeEvent
           );
    }
}
