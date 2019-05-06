using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using EventKit;
using Foundation;
using Toggl.Core.UI.Services;
using Toggl.iOS.Helper;
using UIKit;
using UserNotifications;

namespace Toggl.iOS.Services
{
    [Preserve(AllMembers = true)]
    public sealed class PermissionsCheckerIos : IPermissionsChecker
    {
        private readonly UNAuthorizationOptions options =
            UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;

        public IObservable<bool> CalendarPermissionGranted
            => PermissionsHelper.CalendarPermissionGranted;

        public IObservable<bool> NotificationPermissionGranted
            => PermissionsHelper.NotificationPermissionGranted;

        public IObservable<bool> RequestCalendarAuthorization(bool force = false)
            => PermissionsHelper.RequestCalendarAuthorization(force);

        public IObservable<bool> RequestNotificationAuthorization(bool force = false)
            => PermissionsHelper.RequestNotificationAuthorization(force);

        public void OpenAppSettings()
        {
            PermissionsHelper.OpenAppSettings();
        }
    }
}
