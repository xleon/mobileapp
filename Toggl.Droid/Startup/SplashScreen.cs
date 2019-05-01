using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using MvvmCross.Droid.Support.V7.AppCompat;
using Toggl.Core;
using Toggl.Core.Services;
using Toggl.Core.UI;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.BroadcastReceivers;
using Toggl.Droid.Helper;
using Toggl.Networking;
using static Android.Content.Intent;

namespace Toggl.Droid
{
    [Activity(Label = "Toggl for Devs",
              MainLauncher = true,
              Icon = "@mipmap/ic_launcher",
              Theme = "@style/Theme.Splash",
              ScreenOrientation = ScreenOrientation.Portrait,
              ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    [IntentFilter(
        new[] { "android.intent.action.VIEW", "android.intent.action.EDIT" },
        Categories = new[] { "android.intent.category.BROWSABLE", "android.intent.category.DEFAULT" },
        DataSchemes = new[] { "toggl" },
        DataHost = "*")]
    [IntentFilter(
        new[] { "android.intent.action.PROCESS_TEXT" },
        Categories = new[] { "android.intent.category.DEFAULT" },
        DataMimeType = "text/plain")]
    public class SplashScreen : AppCompatActivity
    {
        public SplashScreen()
            : base()
        {
#if !USE_PRODUCTION_API
            System.Net.ServicePointManager.ServerCertificateValidationCallback
                  += (sender, certificate, chain, sslPolicyErrors) => true;
#endif
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SplashScreen);

            var dependencyContainer = AndroidDependencyContainer.Instance;
            var app = new App<LoginViewModel, CredentialsParameter>(dependencyContainer);

            ApplicationContext.RegisterReceiver(new TimezoneChangedBroadcastReceiver(dependencyContainer.TimeService),
                new IntentFilter(ActionTimezoneChanged));

            createApplicationLifecycleObserver(dependencyContainer.BackgroundService);

            app.Start().ContinueWith(_ =>
            {
                Finish();
            });

            // TODO: Reimplement this when working on deeplinking
            //var navigationUrl = Intent.Data?.ToString() ?? getTrackUrlFromProcessedText();
            //var navigationService = AndroidDependencyContainer.Instance.NavigationService;
            //if (string.IsNullOrEmpty(navigationUrl))
            //{
            //    Finish();
            //    return;
            //}

            //navigationService.Navigate(navigationUrl).ContinueWith(_ =>
            //{
            //    Finish();
            //});
        }

        private string getTrackUrlFromProcessedText()
        {
            if (MarshmallowApis.AreNotAvailable)
                return null;

            var description = Intent.GetStringExtra(ExtraProcessText);
            if (string.IsNullOrWhiteSpace(description))
                return null;

            var applicationUrl = ApplicationUrls.Main.Track(description);
            return applicationUrl;
        }


        private void createApplicationLifecycleObserver(IBackgroundService backgroundService)
        {
            //TODO: Reimplement this
            //var mvxApplication = MvxAndroidApplication.Instance;
            //var appLifecycleObserver = new ApplicationLifecycleObserver(backgroundService);
            //mvxApplication.RegisterActivityLifecycleCallbacks(appLifecycleObserver);
            //mvxApplication.RegisterComponentCallbacks(appLifecycleObserver);
        }
}
}
