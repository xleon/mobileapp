using System;
using System.Reactive.Linq;
using MvvmCross;
using Toggl.Foundation.MvvmCross.Services;

namespace Toggl.Giskard.Services
{
    [Preserve(AllMembers = true)]
    public sealed class PermissionsService : IPermissionsService
    {
        public IObservable<bool> CalendarAuthorizationStatus => Observable.Return(false);

        public void RequestCalendarAuthorization(bool force = false)
        {
            // Nothing to do here...
        }

        public void EnterForeground()
        {
            // Nothing to do here...
        }
    }
}
