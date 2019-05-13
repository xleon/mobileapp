using Toggl.iOS.Extensions;
using Toggl.Core.UI.ViewModels.Calendar;
using CoreGraphics;
using UIKit;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Shared.Extensions;
using Toggl.Core;

namespace Toggl.iOS.ViewControllers.Calendar
{
    public sealed partial class CalendarPermissionDeniedViewController
        : ReactiveViewController<CalendarPermissionDeniedViewModel>
    {
        private const float cardHeight = 342;

        public CalendarPermissionDeniedViewController(CalendarPermissionDeniedViewModel viewModel) : base(viewModel)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            HeadingLabel.Text = Resources.NoWorries;
            MessageLabel.Text = Resources.EnableAccessLater;
            ContinueWithoutAccessButton.SetTitle(Resources.ContinueWithoutAccess, UIControlState.Normal);

            var screenWidth = UIScreen.MainScreen.Bounds.Width;
            PreferredContentSize = new CGSize
            {
                // ScreenWidth - 32 for 16pt margins on both sides
                Width = screenWidth > 320 ? screenWidth - 32 : 312,
                Height = cardHeight
            };

            EnableAccessButton.Rx()
                .BindAction(ViewModel.EnableAccess)
                .DisposedBy(DisposeBag);

            ContinueWithoutAccessButton.Rx()
                .BindAction(ViewModel.Close)
                .DisposedBy(DisposeBag);
        }
    }
}

