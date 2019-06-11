using System;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.Services;
using Toggl.Networking;
using Toggl.Networking.ApiClients;
using Toggl.Shared;
using Toggl.Storage.Settings;

namespace Toggl.Core.Interactors
{
    internal class UnsubscribeFromPushNotificationsInteractor : IInteractor<IObservable<Unit>>
    {
        private readonly IPushNotificationsTokenService pushNotificationsTokenService;
        private readonly IKeyValueStorage keyValueStorage;
        private readonly IPushServicesApi pushServicesApi;

        public UnsubscribeFromPushNotificationsInteractor(
            IPushNotificationsTokenService pushNotificationsTokenService,
            IKeyValueStorage keyValueStorage,
            ITogglApi togglApi
        )
        {
            Ensure.Argument.IsNotNull(pushNotificationsTokenService, nameof(pushNotificationsTokenService));
            Ensure.Argument.IsNotNull(keyValueStorage, nameof(keyValueStorage));
            Ensure.Argument.IsNotNull(togglApi, nameof(togglApi));

            this.pushNotificationsTokenService = pushNotificationsTokenService;
            this.keyValueStorage = keyValueStorage;
            this.pushServicesApi = togglApi.PushServices;
        }

        public IObservable<Unit> Execute()
        {
            var currentToken = pushNotificationsTokenService.Token;

            keyValueStorage.Remove(PushNotificationTokenKeys.PreviouslyRegisteredTokenKey);
            pushNotificationsTokenService.InvalidateCurrentToken();

            if (currentToken.HasValue)
            {
                return pushServicesApi.Unsubscribe(currentToken.Value)
                    .Catch<Unit, Exception>(_ => Observable.Return(Unit.Default));
            }

            return Observable.Return(Unit.Default);
        }
    }
}
