using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FsCheck;
using FsCheck.Xunit;
using NSubstitute;
using Toggl.Foundation.Calendar;
using Toggl.Foundation.Interactors.Notifications;
using Toggl.Multivac;
using Xunit;

namespace Toggl.Foundation.Tests.Interactors.Notifications
{
    public sealed class ScheduleEventNotificationsInteractorTests
    {
        public sealed class TheExecuteMethod : BaseInteractorTests
        {
            private readonly ScheduleEventNotificationsInteractor interactor;

            public TheExecuteMethod()
            {
                interactor = new ScheduleEventNotificationsInteractor(
                    TimeService,
                    CalendarService,
                    UserPreferences,
                    NotificationService
                );
            }

            [Fact, LogIfTooSlow]
            public async Task SchedulesNotificationsForAllUpcomingEventsInTheNextWeekThatBeginAfterTheCurrentDate()
            {
                var now = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
                var endOfWeek = now.AddDays(7);
                var eightHours = TimeSpan.FromHours(8);
                var tenMinutes = TimeSpan.FromMinutes(10);
                var events = Enumerable
                    .Range(0, 14)
                    .Select(number => new CalendarItem(
                        id: number.ToString(),
                        source: CalendarItemSource.Calendar,
                        startTime: now.Add(eightHours * number),
                        duration: eightHours,
                        description: number.ToString(),
                        iconKind: CalendarIconKind.None
                    ));
                var expectedNotifications = events
                    .Where(calendarItem => calendarItem.StartTime >= now + tenMinutes)
                    .Select(@event => new Notification(
                        @event.Id,
                        "Event reminder",
                        @event.Description,
                        @event.StartTime - tenMinutes
                    ));
                UserPreferences
                    .TimeSpanBeforeCalendarNotifications
                    .Returns(Observable.Return(tenMinutes));
                CalendarService
                    .GetEventsInRange(now, endOfWeek)
                    .Returns(Observable.Return(events));
                TimeService.CurrentDateTime.Returns(now);

                await interactor.Execute();

                await NotificationService
                    .Received()
                    .Schedule(Arg.Is<IImmutableList<Notification>>(
                        notifications => notifications.SequenceEqual(expectedNotifications))
                    );
            }

            [Property]
            public void SubtractsTheUserConfiguredTimeSpanFromTheEventsStartDate(long ticks)
            {
                if (ticks < 0) return;

                var timeSpan = TimeSpan.FromTicks(ticks);

                var now = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
                var endOfWeek = now.AddDays(7);
                var eightHours = TimeSpan.FromHours(8);
                var events = Enumerable
                    .Range(0, 14)
                    .Select(number => new CalendarItem(
                        id: number.ToString(),
                        source: CalendarItemSource.Calendar,
                        startTime: now.Add(eightHours * number),
                        duration: eightHours,
                        description: number.ToString(),
                        iconKind: CalendarIconKind.None
                    ));
                var expectedNotifications = events
                    .Where(calendarItem => calendarItem.StartTime >= now + timeSpan)
                    .Select(@event => new Notification(
                        @event.Id,
                        "Event reminder",
                        @event.Description,
                        @event.StartTime - timeSpan
                    ));
                UserPreferences
                    .TimeSpanBeforeCalendarNotifications
                    .Returns(Observable.Return(timeSpan));
                CalendarService
                    .GetEventsInRange(now, endOfWeek)
                    .Returns(Observable.Return(events));
                TimeService.CurrentDateTime.Returns(now);
                NotificationService.ClearReceivedCalls();

                interactor.Execute().Wait();

                NotificationService
                    .Received()
                    .Schedule(Arg.Is<IImmutableList<Notification>>(
                        notifications => notifications.SequenceEqual(expectedNotifications))
                    );
            }
        }
    }
}
