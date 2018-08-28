using System.Reactive;
using Toggl.Foundation.Services;
using Toggl.Multivac;

namespace Toggl.Foundation.Interactors.Notifications
{
    public class UnscheduleAllNotificationsInteractor : IInteractor<Unit>
    {
        private readonly INotificationService notificationService;

        public UnscheduleAllNotificationsInteractor(INotificationService notificationService)
        {
            Ensure.Argument.IsNotNull(notificationService, nameof(notificationService));

            this.notificationService = notificationService;
        }

        public Unit Execute()
        {
            notificationService.UnscheduleAllNotifications();
            return Unit.Default;
        }
    }
}
