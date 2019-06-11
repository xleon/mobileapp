using System;
using System.Reactive.Linq;
using Android.App;
using Firebase.Messaging;

namespace Toggl.Droid.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class TogglFirebaseMessagingService : FirebaseMessagingService
    {
        private IDisposable syncDisposable;
        
        public override void OnMessageReceived(RemoteMessage message)
        {
            var dependencyContainer = AndroidDependencyContainer.Instance;
            var userIsLoggedIn = dependencyContainer.UserAccessManager.CheckIfLoggedIn();
            if (!userIsLoggedIn) return;
            
            var interactorFactory = dependencyContainer.InteractorFactory;
            var dependencyContainerSchedulerProvider = dependencyContainer.SchedulerProvider;

            var syncInteractor = togglApplication().IsInForeground
                ? interactorFactory.RunPushNotificationInitiatedSyncInForeground()
                : interactorFactory.RunPushNotificationInitiatedSyncInBackground();

            syncDisposable = syncInteractor.Execute()
                .ObserveOn(dependencyContainerSchedulerProvider.BackgroundScheduler)
                .Subscribe(_ => StopSelf());
        }

        private TogglApplication togglApplication() => (TogglApplication) Application;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            
            if (!disposing) return;
            
            syncDisposable?.Dispose();
        }
    }
}
