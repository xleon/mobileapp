using MvvmCross.iOS.Views;
using MvvmCross.Plugins.Color.iOS;
using Toggl.Foundation.MvvmCross.Helper;
using UIKit;

namespace Toggl.Daneel.ViewControllers.Navigation
{
    public sealed class TogglNavigationController : MvxNavigationController
    {
        public TogglNavigationController(UIViewController rootViewController)
            : base(rootViewController)
        {
        }

        public override void PushViewController(UIViewController viewController, bool animated)
        {
            base.PushViewController(viewController, animated);

            NavigationBar.ShadowImage = new UIImage();
            NavigationBar.BarTintColor = UIColor.Clear;
            NavigationBar.BackgroundColor = UIColor.Clear;
            NavigationBar.TintColor = Color.NavigationBar.BackButton.ToNativeColor();
            NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
        }
    }
}
