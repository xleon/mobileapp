using System.Reactive;
using System.Threading.Tasks;
using NSubstitute;
using Toggl.Core.Interactors;
using Xunit;

namespace Toggl.Core.Tests.Interactors.PushNotifications
{
    public class InvalidateCurrentTokenInteractorTests : BaseInteractorTests
    {
        private readonly IInteractor<Unit> interactor;

        public InvalidateCurrentTokenInteractorTests()
        {
            interactor = new InvalidateCurrentToken(
                PushNotificationsTokenService,
                KeyValueStorage);
        }

        [Fact, LogIfTooSlow]
        public async Task ClearsTheKeyValueStorage()
        {
            interactor.Execute();

            KeyValueStorage.Received().Remove(PushNotificationTokenKeys.TokenKey);
        }

        [Fact, LogIfTooSlow]
        public async Task InvalidatesTheToken()
        {
            interactor.Execute();

            PushNotificationsTokenService.Received().InvalidateCurrentToken();
        }
    }
}
