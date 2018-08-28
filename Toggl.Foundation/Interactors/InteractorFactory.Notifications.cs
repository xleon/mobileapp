using System.Reactive;
using Toggl.Foundation.Interactors.Notifications;

namespace Toggl.Foundation.Interactors
{
    public sealed partial class InteractorFactory : IInteractorFactory
    {
        public IInteractor<Unit> UnshceduleAllNotifications()
            => new UnscheduleAllNotificationsInteractor(notificationService);
    }
}
