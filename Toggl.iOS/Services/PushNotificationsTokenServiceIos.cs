using System;
using Firebase.InstanceID;
using Toggl.Core.Services;
using Toggl.Shared;

namespace Toggl.iOS.Services
{
    public sealed class PushNotificationsTokenServiceIos : IPushNotificationsTokenService
    {
        public PushNotificationsToken? Token => getToken();

        public void InvalidateCurrentToken()
        {
            InstanceId.SharedInstance.DeleteId(error =>
            {
                Console.WriteLine("Error deleting instance");
            });
        }

        private PushNotificationsToken? getToken()
        {
            var refreshedToken = Firebase.CloudMessaging.Messaging.SharedInstance.FcmToken;
            if (string.IsNullOrEmpty(refreshedToken))
                return null;

            return new PushNotificationsToken(refreshedToken);
        }
    }
}
