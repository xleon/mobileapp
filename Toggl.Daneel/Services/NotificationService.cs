using System;
using System.Collections.Immutable;
using Toggl.Foundation.Services;
using Toggl.Multivac;

namespace Toggl.Daneel.Services
{
    public sealed class NotificationService : INotificationService
    {
        public void Schedule(IImmutableList<Notification> notifications)
        {
            throw new NotImplementedException();
        }

        public void UnscheduleAllNotifications()
        {
            throw new NotImplementedException();
        }
    }
}
