using System;
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
        }

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            Console.WriteLine("Got a PN.");
        }
    }
}
