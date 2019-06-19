using System;
using System.Reactive.Linq;
using Toggl.Core.Services;

namespace Toggl.Core.Extensions
{
    public static class RemoteConfigExtensions
    {
        public static IObservable<bool> ShouldHandlePushNotifications(this IRemoteConfigService remoteConfig)
            => remoteConfig.PushNotificationsConfiguration.Select(pushConfig => pushConfig.HandlePushNotifications);

        public static IObservable<bool> ShouldBeSubscribedToPushNotifications(this IRemoteConfigService remoteConfig)
            => remoteConfig.PushNotificationsConfiguration.Select(pushConfig => pushConfig.RegisterPushNotificationsTokenWithServer);
    }
}
