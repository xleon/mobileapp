using System;
using System.Reactive;

namespace Toggl.Core.Services
{
    public interface IUpdateRemoteConfigCacheService
    {
        IObservable<Unit> RemoteConfigChanged { get; }
        void FetchAndStoreRemoteConfigData();
        bool NeedsToUpdateStoredRemoteConfigData();
    }

    public static class RemoteConfigKeys
    {
        public static string RatingViewDelayParameter { get; } = "day_count";
        public static string RatingViewTriggerParameter { get; } = "criterion";
        public static string RegisterPushNotificationsTokenWithServerParameter { get; } = "register_push_notifications_token_with_server";
        public static string HandlePushNotificationsParameter { get; } = "handle_push_notifications";
        public static string LastFetchAtKey { get; } = "LastFetchAtKey";
    }
}
