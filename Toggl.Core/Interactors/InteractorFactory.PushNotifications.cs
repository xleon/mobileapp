using System;
using System.Reactive;
using Toggl.Core.Interactors.PushNotifications;

namespace Toggl.Core.Interactors
{
    public partial class InteractorFactory
    {
        public IInteractor<Unit> StoreNewTokenInteractor(string token)
            => new StoreNewTokenInteractor(keyValueStorage, token);

        public IInteractor<IObservable<Unit>> InvalidateCurrentToken()
            => new InvalidateCurrentTokenInteractor(pushNotificationsTokenService, keyValueStorage, api);

        public IInteractor<IObservable<Unit>> SubscribeToPushNotifications()
            => new SubscribeToPushNotificationsInteractor(keyValueStorage, api, pushNotificationsTokenService);
    }
}
