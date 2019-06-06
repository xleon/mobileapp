using System;
using System.Reactive.Linq;
using Firebase.CloudMessaging;
using Foundation;
using UIKit;

namespace Toggl.iOS
{
    public partial class AppDelegate
    {
        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            Messaging.SharedInstance.ApnsToken = deviceToken;
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
        }

        [Export("messaging:didReceiveRegistrationToken:")]
        public void DidReceiveRegistrationToken(Messaging messaging, string fcmToken)
        {
            IosDependencyContainer.Instance.InteractorFactory.StoreNewTokenInteractor(fcmToken).Execute();
        }

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            var interactorFactory = IosDependencyContainer.Instance.InteractorFactory;

            var syncInteractor = application.ApplicationState == UIApplicationState.Active
                ? interactorFactory.RunPushNotificationInitiatedSyncInForeground()
                : interactorFactory.RunPushNotificationInitiatedSyncInBackground();

            interactorFactory.GetCurrentUser().Execute()
                .SelectMany(_ => syncInteractor.Execute())
                .Select(mapToNativeOutcomes)
                .Subscribe(completionHandler);
        }
    }
}
