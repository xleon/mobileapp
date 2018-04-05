using Android.App;
using Android.Content.PM;
using MvvmCross.Droid.Views;
using MvvmCross.Platform;
using MvvmCross.Core.Navigation;

namespace Toggl.Giskard
{
    [Activity(Label = "Toggl", 
              NoHistory = true, 
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
    public class SplashScreen : MvxSplashScreenActivity
    {
        public SplashScreen()
            : base(Resource.Layout.SplashScreen)
        {
        }

        protected override void TriggerFirstNavigate()
        {
            base.TriggerFirstNavigate();

            var navigationUrl = Intent.Data?.ToString();
            if (string.IsNullOrEmpty(navigationUrl))
                return;

            Mvx.Resolve<IMvxNavigationService>().Navigate(navigationUrl);
        }

        #if USE_ANALYTICS
        protected override void OnCreate(Android.OS.Bundle bundle)
        {
            base.OnCreate(bundle);

            Microsoft.AppCenter.AppCenter.Start(
                "{TOGGL_APP_CENTER_ID_DROID}",
                typeof(Microsoft.AppCenter.Crashes.Crashes));
        }
        #endif
    }
}
