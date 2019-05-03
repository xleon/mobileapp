using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Android;
using Android.App;
using Android.Content.PM;
using Android.Support.V4.Content;
using MvvmCross;
using Toggl.Core.UI.Services;
using Toggl.Droid.Helper;

namespace Toggl.Droid.Services
{
    [Preserve(AllMembers = true)]
    public sealed class PermissionsCheckerAndroid : IPermissionsChecker
    {
        public IObservable<bool> CalendarPermissionGranted
            => Observable.Start(() => checkPermissions(Manifest.Permission.ReadCalendar));

        public IObservable<bool> NotificationPermissionGranted
            => Observable.Return(true);

        private bool checkPermissions(params string[] permissionsToCheck)
        {
            foreach (var permission in permissionsToCheck)
            {
                if (MarshmallowApis.AreAvailable)
                {
                    if (ContextCompat.CheckSelfPermission(Application.Context, permission) != Permission.Granted)
                        return false;
                }
                else
                {
                    if (PermissionChecker.CheckSelfPermission(Application.Context, permission) != PermissionChecker.PermissionGranted)
                        return false;
                }
            }

            return true;
        }

        public IObservable<bool> RequestCalendarAuthorization(bool force = false)
        {
            throw new NotImplementedException("Use the IView IPermissionRequester interface");
        }

        public IObservable<bool> RequestNotificationAuthorization(bool force = false)
        {
            throw new NotImplementedException("Use the IView IPermissionRequester interface");
        }

        public void OpenAppSettings()
        {
            throw new NotImplementedException("Use the IView IPermissionRequester interface");
        }
    }
}
