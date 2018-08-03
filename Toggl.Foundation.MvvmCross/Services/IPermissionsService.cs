using System;

namespace Toggl.Foundation.MvvmCross.Services
{
    public interface IPermissionsService
    {
        IObservable<bool> CalendarAuthorizationStatus { get; }

        void RequestCalendarAuthorization(bool force = false);

        void EnterForeground();
    }
}
