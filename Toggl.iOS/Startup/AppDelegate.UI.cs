using Toggl.Core.UI.Helper;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS
{
    public partial class AppDelegate
    {
        private void setupTabBar()
        {
            UITabBar.Appearance.SelectedImageTintColor = Colors.TabBar.SelectedImageTintColor.ToNativeColor();
            UITabBarItem.Appearance.TitlePositionAdjustment = new UIOffset(0, 200);
        }

        private void setupNavigationBar()
        {
            //Back button title
            var attributes = new UITextAttributes
            {
                Font = UIFont.SystemFontOfSize(14, UIFontWeight.Medium),
                TextColor = Colors.NavigationBar.BackButton.ToNativeColor()
            };
            UIBarButtonItem.Appearance.SetTitleTextAttributes(attributes, UIControlState.Normal);
            UIBarButtonItem.Appearance.SetTitleTextAttributes(attributes, UIControlState.Highlighted);
            UIBarButtonItem.Appearance.SetBackButtonTitlePositionAdjustment(new UIOffset(6, 0), UIBarMetrics.Default);

            //Back button icon
            var image = UIImage.FromBundle("icBackNoPadding");
            UINavigationBar.Appearance.BackIndicatorImage = image;
            UINavigationBar.Appearance.BackIndicatorTransitionMaskImage = image;

            //Title and background
            var barBackgroundColor = Colors.NavigationBar.BackgroundColor.ToNativeColor();
            UINavigationBar.Appearance.ShadowImage = new UIImage();
            UINavigationBar.Appearance.BarTintColor = barBackgroundColor;
            UINavigationBar.Appearance.BackgroundColor = barBackgroundColor;
            UINavigationBar.Appearance.TintColor = Colors.NavigationBar.BackButton.ToNativeColor();
            UINavigationBar.Appearance.SetBackgroundImage(ImageExtension.ImageWithColor(barBackgroundColor), UIBarMetrics.Default);
            UINavigationBar.Appearance.TitleTextAttributes = new UIStringAttributes
            {
                Font = UIFont.SystemFontOfSize(14, UIFontWeight.Medium),
                ForegroundColor = UIColor.Black
            };
        }
    }
}
