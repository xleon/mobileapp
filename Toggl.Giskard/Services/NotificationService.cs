using System;
using System.Collections.Immutable;
using System.Reactive;
using Toggl.Foundation.Services;
using Toggl.Multivac;

namespace Toggl.Giskard.Services
{
    public sealed class NotificationService : INotificationService
    {
        public IObservable<Unit> Schedule(IImmutableList<Multivac.Notification> notifications)
        {
            throw new NotImplementedException();
        }

        public IObservable<Unit> UnscheduleAllNotifications()
        {
            throw new NotImplementedException();
        }
    }
}
