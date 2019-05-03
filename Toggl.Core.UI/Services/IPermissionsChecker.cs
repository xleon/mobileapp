using System;

namespace Toggl.Core.UI.Services
{
    public interface IPermissionsChecker : IPermissionRequester
    {
        IObservable<bool> CalendarPermissionGranted { get; }

        IObservable<bool> NotificationPermissionGranted { get; }
    }
}
