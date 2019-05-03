using System;

namespace Toggl.Core.UI.Services
{
    public interface IPermissionRequester
    {
        IObservable<bool> RequestCalendarAuthorization(bool force = false);

        IObservable<bool> RequestNotificationAuthorization(bool force = false);

        void OpenAppSettings();
    }
}
