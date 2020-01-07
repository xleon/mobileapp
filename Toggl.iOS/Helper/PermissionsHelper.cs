using EventKit;
using Foundation;
using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Toggl.Core.UI.Services;
using UIKit;
using UserNotifications;

namespace Toggl.iOS.Helper
{
    public static class PermissionsHelper
    {
        private static readonly UNAuthorizationOptions notificationOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;

        public static IObservable<bool> CalendarPermissionGranted
            => Observable.Return(
                EKEventStore.GetAuthorizationStatus(EKEntityType.Event) == EKAuthorizationStatus.Authorized
            );

        public static IObservable<PermissionStatus> NotificationPermissionGranted
            => UNUserNotificationCenter.Current
                .GetNotificationSettingsAsync()
                .ToObservable()
                .Select(settings =>
                {
                    switch (settings.AuthorizationStatus)
                    {
                        case UNAuthorizationStatus.Authorized:
                        case UNAuthorizationStatus.Provisional:
                            return PermissionStatus.Authorized;
                        case UNAuthorizationStatus.Denied:
                            return PermissionStatus.Rejected;
                        default:
                            return PermissionStatus.Unknown;
                    }
                });

        public static IObservable<bool> RequestCalendarAuthorization(bool force = false)
            => requestPermission(
                CalendarPermissionGranted,
                () => new EKEventStore().RequestAccessAsync(EKEntityType.Event),
                force
            );

        public static IObservable<bool> RequestNotificationAuthorization(bool force = false)
            => requestPermission(
                NotificationPermissionGranted,
                () => UNUserNotificationCenter.Current.RequestAuthorizationAsync(notificationOptions),
                force
            );

        public static void OpenAppSettings()
        {
            UIApplication.SharedApplication.OpenUrl(
                NSUrl.FromString(UIApplication.OpenSettingsUrlString)
            );
        }

        private static IObservable<bool> requestPermission(
            IObservable<bool> permissionChecker,
            Func<Task<Tuple<bool, NSError>>> permissionRequestFunction,
            bool force)
            => Observable.DeferAsync(async cancellationToken =>
            {
                var permissionGranted = await permissionChecker;

                if (permissionGranted)
                    return Observable.Return(true);

                if (force)
                {
                    //Fact: If the user changes any permissions through the settings, this app gets restarted
                    //and in that case we don't care about the value returned from this method.
                    //We care about the returned value in the case, when user opens settings
                    //and comes back to this app without altering any permissions. In that case
                    //returning the current permission status is the correct behaviour.
                    OpenAppSettings();
                    return Observable.Return(false);
                }

                return Observable
                    .FromAsync(permissionRequestFunction)
                    .Select(tuple => tuple.Item1);
            });

        private static IObservable<bool> requestPermission(
            IObservable<PermissionStatus> permissionChecker,
            Func<Task<Tuple<bool, NSError>>> permissionRequestFunction,
            bool force)
            => Observable.DeferAsync(async cancellationToken =>
            {
                var status = await permissionChecker;

                switch (status)
                {
                    case PermissionStatus.Authorized:
                        return Observable.Return(true);
                    case PermissionStatus.Rejected when force:
                        //Fact: If the user changes any permissions through the settings, this app gets restarted
                        //and in that case we don't care about the value returned from this method.
                        //We care about the returned value in the case, when user opens settings
                        //and comes back to this app without altering any permissions. In that case
                        //returning the current permission status is the correct behaviour.
                        OpenAppSettings();
                        return Observable.Return(false);
                    default:
                        return Observable
                            .FromAsync(permissionRequestFunction)
                            .Select(tuple => tuple.Item1);
                }
            });
    }
}
