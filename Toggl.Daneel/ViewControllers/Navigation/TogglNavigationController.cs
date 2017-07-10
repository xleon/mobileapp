using CoreGraphics;
using MvvmCross.iOS.Views;
using MvvmCross.Plugins.Color.iOS;
using Toggl.Foundation.MvvmCross.Helper;
using UIKit;

namespace Toggl.Daneel.ViewControllers.Navigation
{
    public sealed class TogglNavigationController : MvxNavigationController
    {
        private static readonly UIColor NavigationBarColor = Color.NavigationBar.BackgroundColor.ToNativeColor();
        private readonly UIImageView titleImage = new UIImageView(UIImage.FromBundle("togglLogo"));
        
        private readonly UIButton reportsButton = new UIButton(new CGRect(0, 0, 40, 40));
        private readonly UIButton settingsButton = new UIButton(new CGRect(0, 0, 40, 40));

        public TogglNavigationController(UIViewController rootViewController)
            : base(rootViewController)
        {
            reportsButton.SetImage(UIImage.FromBundle("icReports"), UIControlState.Normal);
            settingsButton.SetImage(UIImage.FromBundle("icSettings"), UIControlState.Normal);
        }

        public override void PushViewController(UIViewController viewController, bool animated)
        {
            base.PushViewController(viewController, animated);

            NavigationBar.ShadowImage = new UIImage();
            NavigationBar.BarTintColor = NavigationBarColor;
            viewController.NavigationItem.TitleView = titleImage;
            NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
            viewController.NavigationItem.RightBarButtonItems = new[]
            {
                new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace) { Width = -10 },
                new UIBarButtonItem(settingsButton),
                new UIBarButtonItem(reportsButton)
            };
        }
    }
}
