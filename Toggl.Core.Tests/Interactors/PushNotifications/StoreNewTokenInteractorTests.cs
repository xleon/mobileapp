using System.Reactive;
using System.Threading.Tasks;
using NSubstitute;
using Toggl.Core.Interactors;
using Xunit;

namespace Toggl.Core.Tests.Interactors.PushNotifications
{
    public class StoreNewTokenInteractorTests : BaseInteractorTests
    {
        public StoreNewTokenInteractorTests()
        {

        }

        [Fact, LogIfTooSlow]
        public async Task SetsTheTokenInTheKeyValueStorage()
        {
            var token = "token";
            var interactor = new StoreNewTokenInteractor(KeyValueStorage, token);

            interactor.Execute();

            KeyValueStorage.Received().SetString(PushNotificationTokenKeys.TokenKey, token);
        }
    }
}
