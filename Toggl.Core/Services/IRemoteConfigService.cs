using System;
using Toggl.Shared;

namespace Toggl.Core.Services
{
    public interface IRemoteConfigService
    {
        IObservable<RatingViewConfiguration> RatingViewConfiguration { get; }
        IObservable<PushNotificationsConfiguration> PushNotificationsConfiguration { get; }
    }

    public static class RemoteConfigKeys
    {
        public static string RatingViewDelayParameter = "day_count";
        public static string RatingViewTriggerParameter = "criterion";
        public static string RegisterPushNotificationsTokenWithServerParameter = "register_push_notifications_token_with_server";
        public static string HandlePushNotificationsParameter = "handle_push_notifications";
    }
}
