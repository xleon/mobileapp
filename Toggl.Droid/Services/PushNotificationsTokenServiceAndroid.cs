using Firebase.Iid;
using Toggl.Core.Services;
using Toggl.Shared;

namespace Toggl.Droid.Services
{
    public sealed class PushNotificationsTokenServiceAndroid : IPushNotificationsTokenService
    {
        public static string PushNotificationStorageKey = "GiskardFCMTokenKey";

        public PushNotificationsToken? Token => getToken();

        public void InvalidateCurrentToken()
        {
            var dependencyContainer = AndroidDependencyContainer.Instance;
            var keyValueStorage = dependencyContainer.KeyValueStorage;
            keyValueStorage.Remove(PushNotificationStorageKey);
            FirebaseInstanceId.Instance.DeleteInstanceId();
        }

        private PushNotificationsToken? getToken()
        {
            var dependencyContainer = AndroidDependencyContainer.Instance;
            var keyValueStorage = dependencyContainer.KeyValueStorage;

            var rawFcmToken = keyValueStorage.GetString(PushNotificationStorageKey);
            if (string.IsNullOrEmpty(rawFcmToken)) 
                return null;

            return new PushNotificationsToken(rawFcmToken);
        }
    }
}
