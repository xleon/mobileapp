using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
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
                    NotificationService
                );
            }

            [Fact, LogIfTooSlow]
            public async Task SchedulesNotificationsForAllUpcomingEventsInTheNextWeek()
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
                    .Select(@event => new Notification(
                        @event.Id,
                        "Event reminder",
                        @event.Description,
                        @event.StartTime - tenMinutes
                    ));
                CalendarService
                    .GetEventsInRange(now, endOfWeek)
                    .Returns(Observable.Return(events));
                TimeService.CurrentDateTime.Returns(now);

                await interactor.Execute();

                NotificationService
                    .Received()
                    .Schedule(Arg.Is<IImmutableList<Notification>>(
                        notifications => notifications.SequenceEqual(expectedNotifications)));
            }
        }
    }
}
