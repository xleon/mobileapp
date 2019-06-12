using System;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.Services;
using Toggl.Networking;
using Toggl.Networking.ApiClients;
using Toggl.Shared;
using Toggl.Storage.Settings;
using static Toggl.Core.Interactors.PushNotificationTokenKeys;

namespace Toggl.Core.Interactors.PushNotifications
{
    public class SubscribeToPushNotificationsInteractor : IInteractor<IObservable<Unit>>
    {
        private readonly IKeyValueStorage keyValueStorage;
        private readonly IPushServicesApi pushServicesApi;
        private readonly IPushNotificationsTokenService pushNotificationsTokenService;

        public SubscribeToPushNotificationsInteractor(IKeyValueStorage keyValueStorage,  ITogglApi togglApi, IPushNotificationsTokenService pushNotificationsTokenService)
        {
            Ensure.Argument.IsNotNull(togglApi, nameof(togglApi));
            Ensure.Argument.IsNotNull(keyValueStorage, nameof(keyValueStorage));
            Ensure.Argument.IsNotNull(pushNotificationsTokenService, nameof(pushNotificationsTokenService));

            this.keyValueStorage = keyValueStorage;
            this.pushNotificationsTokenService = pushNotificationsTokenService;
            pushServicesApi = togglApi.PushServices;
        }

        public IObservable<Unit> Execute()
        {
            var token = pushNotificationsTokenService.Token;
            var defaultToken = default(PushNotificationsToken);
            if (!token.HasValue || (string)token.Value == (string)defaultToken)
                return Observable.Return(Unit.Default);

            var previouslyRegisteredToken = keyValueStorage.GetString(PreviouslyRegisteredTokenKey);
            if (!string.IsNullOrEmpty(previouslyRegisteredToken) && previouslyRegisteredToken == (string)token.Value)
                return Observable.Return(Unit.Default);

            return pushServicesApi
                .Subscribe(token.Value)
                .Do(_ => storeRegisteredToken(token.Value.ToString()))
                .Catch((Exception ex) => Observable.Return(Unit.Default));
        }

        private void storeRegisteredToken(string token)
        {
            keyValueStorage.SetString(PreviouslyRegisteredTokenKey, token);
        }
    }
}
