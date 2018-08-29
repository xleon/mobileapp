using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Foundation;
using Toggl.Daneel.Extensions;
using Toggl.Foundation;
using Toggl.Foundation.Calendar;
using Toggl.Foundation.MvvmCross.Services;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using UserNotifications;
using Notification = Toggl.Multivac.Notification;

namespace Toggl.Daneel.Services
{
    public sealed class NotificationService : PermissionAwareNotificationService
    {
        private const string calendarEventIdKey = "Id";

        private readonly ITimeService timeService;

        public NotificationService(IPermissionsService permissionsService, ITimeService timeService)
            : base(permissionsService)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.timeService = timeService;
        }

        protected override IObservable<Unit> NativeSchedule(IImmutableList<Notification> notifications)
            => Observable.FromAsync(async () =>
                await notifications
                    .Select(notificationRequest)
                    .Select(UNUserNotificationCenter.Current.AddNotificationRequestAsync)
                    .Apply(Task.WhenAll)
            );

        protected override IObservable<Unit> NativeUnscheduleAllNotifications()
        {
            UNUserNotificationCenter.Current.RemoveAllPendingNotificationRequests();

            return Observable.Return(Unit.Default);
        }

        private UNNotificationRequest notificationRequest(Notification notification)
        {
            var identifier = notification.GetIdentifier();

            var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(
                notification.GetTriggerTime(timeService.CurrentDateTime),
                repeats: false
            );

            var content = new UNMutableNotificationContent
            {
                Title = notification.Title,
                Body = notification.Description,
                Sound = UNNotificationSound.Default,
                UserInfo = new NSDictionary(calendarEventIdKey, notification.Id)
            };

            return UNNotificationRequest.FromIdentifier(identifier, content, trigger);
        }
    }
}
