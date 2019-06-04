using Android.App;
using Firebase.Iid;
using Toggl.Core;
using Toggl.Networking;

namespace Toggl.Droid.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class TogglFirebaseIIDService : FirebaseInstanceIdService
    {
        private const ApiEnvironment environment =
        #if USE_PRODUCTION_API
            ApiEnvironment.Production;
        #else
            ApiEnvironment.Staging;
        #endif
        
        public override void OnTokenRefresh()
        {
            var applicationContext = Application.Context;
            var packageInfo = applicationContext.PackageManager.GetPackageInfo(applicationContext.PackageName, 0);
            AndroidDependencyContainer.EnsureInitialized(environment, Platform.Giskard, packageInfo.VersionName);
            
            var dependencyContainer = AndroidDependencyContainer.Instance;
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            var keyValueStorage = dependencyContainer.KeyValueStorage;
            keyValueStorage.SetString(PushNotificationsTokenServiceAndroid.PushNotificationStorageKey, refreshedToken);
        }
    }
}