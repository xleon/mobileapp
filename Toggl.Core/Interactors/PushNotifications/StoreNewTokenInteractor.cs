using System.Reactive;
using Toggl.Core.Services;
using Toggl.Shared;
using Toggl.Storage.Settings;

namespace Toggl.Core.Interactors
{
    internal class StoreNewTokenInteractor : IInteractor<Unit>
    {
        private readonly IKeyValueStorage keyValueStorage;
        private readonly string token;

        public StoreNewTokenInteractor(IKeyValueStorage keyValueStorage, string token)
        {
            Ensure.Argument.IsNotNull(keyValueStorage, nameof(keyValueStorage));
            Ensure.Argument.IsNotNull(token, nameof(token));

            this.keyValueStorage = keyValueStorage;
            this.token = token;
        }

        public Unit Execute()
        {
            keyValueStorage.SetString(PushNotificationTokenKeys.TokenKey, token);
            return Unit.Default;
        }
    }
}
