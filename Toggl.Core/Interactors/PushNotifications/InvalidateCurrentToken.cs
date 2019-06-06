using System.Reactive;
using Toggl.Core.Services;
using Toggl.Shared;
using Toggl.Storage.Settings;

namespace Toggl.Core.Interactors
{
    internal class InvalidateCurrentToken : IInteractor<Unit>
    {
        private readonly IPushNotificationsTokenService pushNotificationsTokenService;
        private readonly IKeyValueStorage keyValueStorage;

        public InvalidateCurrentToken(
            IPushNotificationsTokenService pushNotificationsTokenService,
            IKeyValueStorage keyValueStorage
        )
        {
            Ensure.Argument.IsNotNull(pushNotificationsTokenService, nameof(pushNotificationsTokenService));
            Ensure.Argument.IsNotNull(keyValueStorage, nameof(keyValueStorage));

            this.pushNotificationsTokenService = pushNotificationsTokenService;
            this.keyValueStorage = keyValueStorage;
        }

        public Unit Execute()
        {
            keyValueStorage.Remove(PushNotificationTokenKeys.TokenKey);
            pushNotificationsTokenService.InvalidateCurrentToken();
            return Unit.Default;
        }
    }
}
