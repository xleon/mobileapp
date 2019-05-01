using Foundation;
using UIKit;
using UserNotifications;

namespace Toggl.iOS
{
    [Register(nameof(AppDelegate))]
    public sealed class AppDelegate : MvxApplicationDelegate<Setup, App<OnboardingViewModel>>, IUNUserNotificationCenterDelegate
    {
        public override UIWindow Window { get; set; }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            initializeAnalytics();

            base.FinishedLaunching(application, launchOptions);

            UNUserNotificationCenter.Current.Delegate = this;

            #if ENABLE_TEST_CLOUD
            Xamarin.Calabash.Start();
            #endif

            return true;
        }

        protected override void RunAppStart(object hint = null)
        {
            base.RunAppStart(hint);

            setupNavigationBar();
            setupTabBar();
        }
        
        #if USE_ANALYTICS
        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            var openUrlOptions = new UIApplicationOpenUrlOptions(options);

            return Google.SignIn.SignIn.SharedInstance.HandleUrl(url, openUrlOptions.SourceApplication, openUrlOptions.Annotation);
        }
        #endif

        public override void ApplicationSignificantTimeChange(UIApplication application)
        {
            IosDependencyContainer.Instance.TimeService.SignificantTimeChanged();
        }
    }
}
