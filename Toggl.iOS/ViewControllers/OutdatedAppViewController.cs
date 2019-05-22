using MvvmCross.Platforms.Ios.Presenters.Attributes;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Core;
using Toggl.Core.UI.ViewModels;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    [MvxRootPresentation]
    public sealed partial class OutdatedAppViewController : ReactiveViewController<OutdatedAppViewModel>
    {
        public OutdatedAppViewController()
            : base(nameof(OutdatedAppViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            HeadingLabel.Text = Resources.Oops;
            TextLabel.Text = Resources.AppOutdatedMessage;
            UpdateButton.SetTitle(Resources.UpdateTheApp, UIControlState.Normal);

            UpdateButton.Rx()
                .BindAction(ViewModel.UpdateApp)
                .DisposedBy(DisposeBag);

            WebsiteButton.Rx()
                .BindAction(ViewModel.OpenWebsite)
                .DisposedBy(DisposeBag);
        }
    }
}
