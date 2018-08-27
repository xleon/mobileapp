using System.Collections.Immutable;
using Toggl.Multivac;

namespace Toggl.Foundation.Services
{
    public interface INotificationService
    {
        void Schedule(IImmutableList<Notification> notifications);
        void UnscheduleAllNotifications();
    }
}
