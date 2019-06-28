using Firebase.RemoteConfig;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Toggl.Core.Services;
using Toggl.Shared;
using static Toggl.Core.Services.RemoteConfigKeys;

namespace Toggl.iOS.Services
{
    public sealed class RemoteConfigServiceIos : IRemoteConfigService
    {
        private const string remoteConfigDefaultsFileName = "RemoteConfigDefaults";

        public RemoteConfigServiceIos()
        {
            var remoteConfig = RemoteConfig.SharedInstance;
            remoteConfig.SetDefaults(plistFileName: remoteConfigDefaultsFileName);
        }

        public IObservable<RatingViewConfiguration> RatingViewConfiguration
            => FetchConfiguration(extractRatingViewConfiguration);

        public IObservable<PushNotificationsConfiguration> PushNotificationsConfiguration
            => Observable.Return(new PushNotificationsConfiguration(false, false));

        private RatingViewConfiguration extractRatingViewConfiguration(RemoteConfig remoteConfig)
            => new RatingViewConfiguration(
                remoteConfig[RatingViewDelayParameter].NumberValue.Int32Value,
                remoteConfig[RatingViewTriggerParameter].StringValue.ToRatingViewCriterion());

        private PushNotificationsConfiguration extractPushNotificationsConfiguration(RemoteConfig remoteConfig)
            => new PushNotificationsConfiguration(
                remoteConfig[RegisterPushNotificationsTokenWithServerParameter].BoolValue,
                remoteConfig[HandlePushNotificationsParameter].BoolValue);

        private IObservable<TConfiguration> FetchConfiguration<TConfiguration>(Func<RemoteConfig, TConfiguration> remoteConfigExtractor)
            => Observable.Create<TConfiguration>(observer =>
            {
                var remoteConfig = RemoteConfig.SharedInstance;
                remoteConfig.Fetch((status, error) =>
                {
                    if (error == null)
                        remoteConfig.ActivateFetched();

                    observer.OnNext(remoteConfigExtractor(remoteConfig));
                    observer.OnCompleted();
                });
                return Disposable.Empty;
            });
    }
}
