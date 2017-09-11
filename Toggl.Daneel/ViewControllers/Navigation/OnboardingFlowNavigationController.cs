using MvvmCross.iOS.Views;
using UIKit;

namespace Toggl.Daneel.ViewControllers.Navigation
{
    public sealed class OnboardingFlowNavigationController : MvxNavigationController
    {
        public OnboardingFlowNavigationController(UIViewController viewController)
            : base(viewController)
        {
        }

        public override void PushViewController(UIViewController viewController, bool animated)
        {
            base.PushViewController(viewController, animated);

            var navBar = viewController?.NavigationController?.NavigationBar;
            if (navBar == null) return;

            navBar.BarStyle = UIBarStyle.Black;
            navBar.ShadowImage = new UIImage();
            navBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
            UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.LightContent;
            navBar.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = UIColor.White,
                Font = UIFont.SystemFontOfSize(14, UIFontWeight.Medium)
            };
        }
    }
}
