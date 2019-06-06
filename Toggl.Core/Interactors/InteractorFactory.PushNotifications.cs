using System.Reactive;
using Toggl.Shared;

namespace Toggl.Core.Interactors
{
    public partial class InteractorFactory
    {
        public IInteractor<Unit> StoreNewTokenInteractor(string token)
            => new StoreNewTokenInteractor(keyValueStorage, token);

        public IInteractor<Unit> InvalidateCurrentToken()
            => new InvalidateCurrentToken(pushNotificationsTokenService, keyValueStorage);
    }
}
