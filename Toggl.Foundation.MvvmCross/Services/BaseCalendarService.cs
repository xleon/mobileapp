using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Toggl.Foundation.Exceptions;
using Toggl.Foundation.MvvmCross.Services;
using Toggl.Foundation.Services;
using Toggl.Multivac;

namespace Toggl.Foundation.Calendar
{
    public abstract class BaseCalendarService : ICalendarService
    {
        private readonly IPermissionsService permissionsService;

        public BaseCalendarService(IPermissionsService permissionsService)
        {
            Ensure.Argument.IsNotNull(permissionsService, nameof(permissionsService));

            this.permissionsService = permissionsService;
        }

        public IObservable<IEnumerable<UserCalendar>> GetUserCalendars()
            => Observable.Defer(() =>
            {
                var isAuthorized = permissionsService.CalendarPermissionGranted;
                if (!isAuthorized)
                {
                    return Observable.Throw<IEnumerable<UserCalendar>>(
                        new NotAuthorizedException("You don't have permission to access calendars")
                    );
                }

                var calendars = NativeGetUserCalendars();
                return Observable.Return(calendars);
            });

        public IObservable<IEnumerable<CalendarItem>> GetEventsForDate(DateTime date)
        {
            var startOfDay = new DateTimeOffset(date.Date);
            var endOfDay = startOfDay.AddDays(1);

            return GetEventsInRange(startOfDay, endOfDay);
        }

        public IObservable<IEnumerable<CalendarItem>> GetEventsInRange(DateTimeOffset start, DateTimeOffset end)
        {
            return Observable.Defer(() =>
            {
                var isAuthorized = permissionsService.CalendarPermissionGranted;
                if (!isAuthorized)
                {
                    return Observable.Throw<IEnumerable<CalendarItem>>(
                        new NotAuthorizedException("You don't have permission to access calendars")
                    );
                }

                return Observable.Return(NativeGetEventsInRange(start, end));
            });
        }
        
        protected abstract IEnumerable<UserCalendar> NativeGetUserCalendars();
        protected abstract IEnumerable<CalendarItem> NativeGetEventsInRange(DateTimeOffset start, DateTimeOffset end);
    }
}
