using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Networking.Tests.Integration.BaseTests;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Xunit;

namespace Toggl.Networking.Tests.Integration
{
    public sealed class PushServicesApiTests
    {
        public sealed class SubscribeToPushServices : AuthenticatedPostEndpointBaseTests<Unit>
        {
            [Fact]
            public async Task AllowsToSubmitTheTokenRepeatedly()
            {
                var (togglApi, _) = await SetupTestUser();
                var token = randomToken();

                await togglApi.PushServices.Subscribe(token);
                await togglApi.PushServices.Subscribe(token);
                await togglApi.PushServices.Subscribe(token);

                // no exception was thrown
            }

            protected override IObservable<Unit> CallEndpointWith(ITogglApi togglApi)
                => togglApi.PushServices.Subscribe(randomToken());
        }    

        private static PushNotificationsToken randomToken()
            => new PushNotificationsToken(Guid.NewGuid().ToString());
    }
}
