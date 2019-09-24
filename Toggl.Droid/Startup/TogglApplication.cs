using System;
using System.Threading.Tasks;
using Android.App;
using Android.Arch.Lifecycle;
using Android.Content;
using Android.Runtime;
using Android.Views.Accessibility;
using Java.Interop;
using Toggl.Core;
using Toggl.Core.UI;
using Toggl.Droid.BroadcastReceivers;
using Toggl.Droid.Helper;
using static Android.Support.V7.App.AppCompatDelegate;
using static Android.Content.Intent;

namespace Toggl.Droid
{
    [Application(AllowBackup = false)]
    public class TogglApplication : Application, ILifecycleObserver
    {
        public AppStart AppStart { get; private set; }

        public TimezoneChangedBroadcastReceiver TimezoneChangedBroadcastReceiver { get; set; }

        public bool IsInForeground { get; private set; } = false;

        public TogglApplication(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            ProcessLifecycleOwner.Get().Lifecycle.AddObserver(this);

#if !DEBUG
            Firebase.FirebaseApp.InitializeApp(this);
#endif

#if USE_APPCENTER
            Microsoft.AppCenter.AppCenter.Start(
                "{TOGGL_APP_CENTER_ID_DROID}",
                typeof(Microsoft.AppCenter.Crashes.Crashes),
                typeof(Microsoft.AppCenter.Analytics.Analytics));
#endif

            DefaultNightMode = QApis.AreAvailable ? ModeNightFollowSystem : ModeNightAuto;

            AndroidDependencyContainer.EnsureInitialized(Context);
            AppStart = new AppStart(AndroidDependencyContainer.Instance);

            Task.Run(secondaryInitialization);

            var accessLevel = AppStart.GetAccessLevel();
            if (accessLevel == AccessLevel.TokenRevoked || accessLevel == AccessLevel.LoggedIn)
            {
                AndroidDependencyContainer.Instance
                    .UserAccessManager
                    .LoginWithSavedCredentials();
            }

#if DEBUG
            // Add or remove `Detect*` chains to detect unwanted behaviour
            // Change the `Penalty*` to change how the StrictMode works, allowing it to crash the app if necessary
            // Try not to misinterpret the logs/penalties; You should only be looking for behaviour that shouldn't
            // be happening
            Android.OS.StrictMode.SetVmPolicy(
                new Android.OS.StrictMode.VmPolicy.Builder()
                    .DetectActivityLeaks()
                    .DetectLeakedClosableObjects()
                    .DetectLeakedRegistrationObjects()
                    .DetectLeakedSqlLiteObjects()
                    .PenaltyLog()
                    .Build());
            Android.OS.StrictMode.SetThreadPolicy(
                new Android.OS.StrictMode.ThreadPolicy.Builder()
                    .DetectCustomSlowCalls()
                    .PenaltyLog()
                    .Build());
#endif
        }

        private async Task secondaryInitialization()
        {
            await Task.Yield();

            var container = AndroidDependencyContainer.Instance;

            AppStart.LoadLocalizationConfiguration();
            AppStart.SetupBackgroundSync();
            AppStart.SetFirstOpened();
            AppStart.UpdateOnboardingProgress();

            var accessibilityManager = GetSystemService(AccessibilityService) as AccessibilityManager;
            if (accessibilityManager != null)
            {
                var accessibilityEnabled = accessibilityManager.IsTouchExplorationEnabled;
                AndroidDependencyContainer.Instance.AnalyticsService.AccessibilityEnabled.Track(accessibilityEnabled);
            }

            var currentTimezoneChangedBroadcastReceiver = TimezoneChangedBroadcastReceiver;
            if (currentTimezoneChangedBroadcastReceiver != null)
            {
                UnregisterReceiver(currentTimezoneChangedBroadcastReceiver);
            }

            TimezoneChangedBroadcastReceiver = new TimezoneChangedBroadcastReceiver(container.TimeService);
            ApplicationContext.RegisterReceiver(TimezoneChangedBroadcastReceiver, new IntentFilter(ActionTimezoneChanged));
        }

        [Export]
        [Lifecycle.Event.OnStart]
        public void OnEnterForeground()
        {
            IsInForeground = true;
            var backgroundService = AndroidDependencyContainer.Instance?.BackgroundService;
            backgroundService?.EnterForeground();
        }

        [Export]
        [Lifecycle.Event.OnStop]
        public void OnEnterBackground()
        {
            IsInForeground = false;
            var backgroundService = AndroidDependencyContainer.Instance?.BackgroundService;
            backgroundService?.EnterBackground();
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);
            switch (level)
            {
                case TrimMemory.RunningCritical:
                case TrimMemory.RunningLow:
                    AndroidDependencyContainer.Instance
                        ?.AnalyticsService
                        ?.ReceivedLowMemoryWarning
                        ?.Track(Platform.Giskard);
                    break;
            }
        }
    }
}
