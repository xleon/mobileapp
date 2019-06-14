using System;
using System.Reactive;
using System.Reactive.Linq;
using Android.App;
using Firebase.Messaging;
using Toggl.Core.Extensions;
using Toggl.Shared.Extensions;

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

            var shouldHandlePushNotifications = dependencyContainer.RemoteConfigService.ShouldHandlePushNotifications(); 

            syncDisposable = shouldHandlePushNotifications
                .SelectMany(willHandlePushNotification => willHandlePushNotification
                    ? syncInteractor.Execute().SelectUnit()
                    : Observable.Return(Unit.Default))
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
