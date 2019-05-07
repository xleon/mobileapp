using System;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Core;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers.Settings
{
    public sealed partial class NotificationSettingsViewController : ReactiveViewController<NotificationSettingsViewModel>
    {
        public NotificationSettingsViewController() : base(nameof(NotificationSettingsViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationItem.Title = Resources.NotificationSettingsTitle;
            NotificationDisabledLabel.Text = Resources.NotificationDisabledNotice;
            OpenSettingsButton.SetTitle(Resources.OpenSettingsApp, UIControlState.Normal);
            RowLabel.Text = Resources.UpcomingEvent;
            ExplainationLabel.Text = Resources.NotificationSettingExplaination;

            ViewModel.PermissionGranted
                .Invert()
                .Subscribe(OpenSettingsContainer.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.PermissionGranted
                .Subscribe(CalendarNotificationsContainer.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            OpenSettingsButton.Rx()
                .BindAction(ViewModel.RequestAccess)
                .DisposedBy(DisposeBag);

            CalendarNotificationsRow.Rx().Tap()
                .Subscribe(ViewModel.OpenUpcomingEvents.Inputs)
                .DisposedBy(DisposeBag);

            ViewModel.UpcomingEvents
                .Subscribe(CalendarNotificationsValue.Rx().Text())
                .DisposedBy(DisposeBag);
        }
    }
}
