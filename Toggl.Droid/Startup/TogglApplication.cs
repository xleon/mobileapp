using System;
using Android.App;
using Android.Runtime;
using Toggl.Droid.BroadcastReceivers;

namespace Toggl.Droid
{
    [Application(AllowBackup = false)]
    public class TogglApplication : Application
    {
        public TimezoneChangedBroadcastReceiver TimezoneChangedBroadcastReceiver { get; set; }
        
        public ApplicationLifecycleObserver ApplicationLifecycleObserver { get; set; }
        
        public TogglApplication(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            Firebase.FirebaseApp.InitializeApp(this);

            var applicationContext = Context;
            AndroidDependencyContainer.EnsureInitialized(Context);
#if USE_APPCENTER
            Microsoft.AppCenter.AppCenter.Start(
                "{TOGGL_APP_CENTER_ID_DROID}",
                typeof(Microsoft.AppCenter.Crashes.Crashes),
                typeof(Microsoft.AppCenter.Analytics.Analytics));
#endif
        }
    }
}
