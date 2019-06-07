using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Android.App;
using Firebase.Iid;
using Toggl.Core;
using Toggl.Networking;
using Toggl.Shared.Extensions;

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

        private CompositeDisposable disposeBag = new CompositeDisposable();
        
        public override void OnTokenRefresh()
        {
            var applicationContext = Application.Context;
            var packageInfo = applicationContext.PackageManager.GetPackageInfo(applicationContext.PackageName, 0);
            AndroidDependencyContainer.EnsureInitialized(environment, Platform.Giskard, packageInfo.VersionName);

            var token = FirebaseInstanceId.Instance.Token;

            var dependencyContainer = AndroidDependencyContainer.Instance;
            dependencyContainer.InteractorFactory.StoreNewTokenInteractor(token).Execute();

            registerTokenIfNecessary(dependencyContainer);
        }

        private void registerTokenIfNecessary(AndroidDependencyContainer dependencyContainer)
        {
            var userLoggedIn = dependencyContainer.UserAccessManager.CheckIfLoggedIn();
            if (!userLoggedIn) return;
            
            var subscribeToPushNotificationsInteractor = dependencyContainer.InteractorFactory.SubscribeToPushNotifications();
            subscribeToPushNotificationsInteractor
                .Execute()
                .ObserveOn(dependencyContainer.SchedulerProvider.BackgroundScheduler)
                .Subscribe(_ => StopSelf())
                .DisposedBy(disposeBag);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;
            
            disposeBag?.Dispose();
        }
    }
}
