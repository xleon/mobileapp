using System;
using Android.App;
using Android.Runtime;
using Toggl.Core;
using Toggl.Networking;

namespace Toggl.Droid
{
    [Application(AllowBackup = false)]
    public class TogglApplication : Application
    {
        private const ApiEnvironment environment =
#if USE_PRODUCTION_API
                        ApiEnvironment.Production;
#else
                        ApiEnvironment.Staging;
#endif

        public TogglApplication(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            Firebase.FirebaseApp.InitializeApp(this);

            var applicationContext = Context;
            var packageInfo = applicationContext.PackageManager.GetPackageInfo(applicationContext.PackageName, 0);
            AndroidDependencyContainer.EnsureInitialized(environment, Platform.Giskard, packageInfo.VersionName);
#if USE_ANALYTICS
            Microsoft.AppCenter.AppCenter.Start(
                "{TOGGL_APP_CENTER_ID_DROID}",
                typeof(Microsoft.AppCenter.Crashes.Crashes),
                typeof(Microsoft.AppCenter.Analytics.Analytics));
#endif
        }
    }
}
