using System;

namespace Toggl.Foundation.MvvmCross.Services
{
    public interface IPermissionsService
    {
        bool CalendarPermissionGranted { get; }

        IObservable<bool> RequestCalendarAuthorization(bool force = false);

        void OpenAppSettings();
    }
}
