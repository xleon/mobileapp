using System.Reactive;
using Foundation;
using Toggl.Core;
using Toggl.Core.UI;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Presentation;
using Toggl.Networking;
using UIKit;
using UserNotifications;

namespace Toggl.iOS
{
    [Register(nameof(AppDelegate))]
    public sealed partial class AppDelegate : UIApplicationDelegate, IUNUserNotificationCenterDelegate
    {
        private const ApiEnvironment environment =
#if USE_PRODUCTION_API
                        ApiEnvironment.Production;
#else
                        ApiEnvironment.Staging;
#endif

        public override UIWindow Window { get; set; }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
#if !USE_PRODUCTION_API
            System.Net.ServicePointManager.ServerCertificateValidationCallback
                  += (sender, certificate, chain, sslPolicyErrors) => true;
#endif

            initializeAnalytics();

            base.FinishedLaunching(application, launchOptions);

            var version = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"].ToString();
            IosDependencyContainer.EnsureInitialized(new TogglPresenter(this, Window), environment, Platform.Daneel, version);
            var app = new App<OnboardingViewModel, Unit>(IosDependencyContainer.Instance);

            app.Start();

            setupNavigationBar();
            setupTabBar();

            UNUserNotificationCenter.Current.Delegate = this;

            #if ENABLE_TEST_CLOUD
            Xamarin.Calabash.Start();
            #endif

            return true;
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
