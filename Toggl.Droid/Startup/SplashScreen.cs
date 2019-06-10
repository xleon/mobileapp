using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using System.Reactive;
using Toggl.Core;
using Toggl.Core.Services;
using Toggl.Core.UI;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Activities;
using Toggl.Droid.BroadcastReceivers;
using Toggl.Droid.Presentation;
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
    public partial class SplashScreen : AppCompatActivity
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

            var dependencyContainer = AndroidDependencyContainer.Instance;

            registerTimezoneChangedBroadcastReceiver(dependencyContainer.TimeService);
            registerApplicationLifecycleObserver(dependencyContainer.BackgroundService);

            var app = new AppStart(dependencyContainer);
            app.UpdateOnboardingProgress();

            var accessLevel = app.GetAccessLevel();
            if (accessLevel != AccessLevel.LoggedIn)
            {
                navigateAccordingToAccessLevel(accessLevel);
                Finish();
                return;
            }
            
            clearAllViewModelsAndSetupRootViewModel(dependencyContainer.ViewModelCache, dependencyContainer.ViewModelLoader); 
            
            var navigationUrl = Intent.Data?.ToString() ?? getTrackUrlFromProcessedText();
            if (string.IsNullOrEmpty(navigationUrl))
            {
                StartActivity(typeof(MainTabBarActivity));
                Finish();
                return;
            }

            handleDeepLink(new Uri(navigationUrl), dependencyContainer);
            return;
        }

        private void clearAllViewModelsAndSetupRootViewModel(ViewModelCache viewModelCache, ViewModelLoader viewModelLoader)
        {
            viewModelCache.ClearAll();
            var viewModel = (MainTabBarViewModel)viewModelLoader.Load<Unit, Unit>(typeof(MainTabBarViewModel), Unit.Default)
                .GetAwaiter()
                .GetResult();
            viewModelCache.Cache(viewModel);  
        }

        private void registerApplicationLifecycleObserver(IBackgroundService backgroundService)
        {
            var togglApplication = getTogglApplication();
            var currentAppLifecycleObserver = togglApplication.ApplicationLifecycleObserver;
            if (currentAppLifecycleObserver != null)
            {
                Application.UnregisterActivityLifecycleCallbacks(currentAppLifecycleObserver);
                Application.UnregisterComponentCallbacks(currentAppLifecycleObserver);
            }
            
            togglApplication.ApplicationLifecycleObserver = new ApplicationLifecycleObserver(backgroundService);
            Application.RegisterActivityLifecycleCallbacks(togglApplication.ApplicationLifecycleObserver);
            Application.RegisterComponentCallbacks(togglApplication.ApplicationLifecycleObserver);
        }

        private void registerTimezoneChangedBroadcastReceiver(ITimeService timeService)
        {
            var togglApplication = getTogglApplication();
            var currentTimezoneChangedBroadcastReceiver = togglApplication.TimezoneChangedBroadcastReceiver;
            if (currentTimezoneChangedBroadcastReceiver != null)
            {
                Application.UnregisterReceiver(currentTimezoneChangedBroadcastReceiver);
            }

            togglApplication.TimezoneChangedBroadcastReceiver = new TimezoneChangedBroadcastReceiver(timeService);
            ApplicationContext.RegisterReceiver(togglApplication.TimezoneChangedBroadcastReceiver, new IntentFilter(ActionTimezoneChanged));    
        }

        private TogglApplication getTogglApplication()
            => (TogglApplication)Application;

        private void navigateAccordingToAccessLevel(AccessLevel accessLevel)
        {
            var navigationService = AndroidDependencyContainer.Instance.NavigationService;

            switch (accessLevel)
            {
                case AccessLevel.AccessRestricted:
                    navigationService.Navigate<OutdatedAppViewModel>(null);
                    return;
                case AccessLevel.NotLoggedIn:
                    navigationService.Navigate<LoginViewModel, CredentialsParameter>(new CredentialsParameter(), null);
                    return;
                case AccessLevel.TokenRevoked:
                    navigationService.Navigate<TokenResetViewModel>(null);
                    return;
            }
        }
    }
}
