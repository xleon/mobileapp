using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using EventKit;
using Toggl.Daneel.Extensions;
using Toggl.Foundation.Calendar;
using Toggl.Foundation.MvvmCross.Services;
using Toggl.Multivac;

namespace Toggl.Daneel.Services
{
    public sealed class CalendarService : BaseCalendarService
    {
        private EKEventStore eventStore => new EKEventStore();

        public CalendarService(IPermissionsService permissionsService)
            : base (permissionsService)
        {
        }

        protected override IEnumerable<CalendarItem> NativeGetEventsInRange(DateTimeOffset start, DateTimeOffset end)
        {
            var calendars = eventStore.GetCalendars(EKEntityType.Event);

            var predicate = eventStore
                .PredicateForEvents(start.ToNSDate(), end.ToNSDate(), calendars);

            var calendarItems = eventStore
                .EventsMatching(predicate)
                ?.Where(isValidEvent)
                ?.Select(calendarItemFromEvent)
                ?? new CalendarItem[0];

            return calendarItems;
        }

        protected override IEnumerable<UserCalendar> NativeGetUserCalendars()
            => eventStore
                .GetCalendars(EKEntityType.Event)
                .Select(userCalendarFromEKCalendar);

        private UserCalendar userCalendarFromEKCalendar(EKCalendar calendar)
            => new UserCalendar(
                calendar.CalendarIdentifier,
                calendar.Title,
                calendar.Source.Title);

        private CalendarItem calendarItemFromEvent(EKEvent ev)
        {
            var startDate = ev.StartDate.ToDateTimeOffset();
            var endDate = ev.EndDate.ToDateTimeOffset();
            var duration = endDate - startDate;

            return new CalendarItem(
                CalendarItemSource.Calendar,
                startDate,
                duration,
                ev.Title,
                CalendarIconKind.Event,
                ev.Calendar.CGColor.ToHexColor(),
                calendarId: ev.Calendar.CalendarIdentifier
            );
        }

        private bool isValidEvent(EKEvent ev)
            => !ev.AllDay;
    }
}
