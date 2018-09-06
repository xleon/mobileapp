using System.Reactive.Linq;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using Toggl.Daneel.Extensions;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.MvvmCross.ViewModels.Settings;
using Toggl.Multivac.Extensions;

namespace Toggl.Daneel.ViewControllers.Settings
{
    [MvxChildPresentation]
    public sealed partial class NotificationSettingsViewController : ReactiveViewController<NotificationSettingsViewModel>
    {
        public NotificationSettingsViewController() : base(nameof(NotificationSettingsViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.Bind(ViewModel.PermissionGranted.Invert(), OpenSettingsContainer.BindIsVisible());
            this.Bind(ViewModel.PermissionGranted, CalendarNotificationsContainer.BindIsVisible());

            this.Bind(OpenSettingsButton.Tapped(), ViewModel.RequestAccessAction);

            this.Bind(CalendarNotificationsRow.Tapped(), ViewModel.OpenUpcomingEvents);
            this.Bind(ViewModel.UpcomingEvents, CalendarNotificationsValue.BindText());
        }
    }
}
