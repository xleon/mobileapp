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

        public sealed class UnsubscribeFromsPushServices : AuthenticatedDeleteEndpointBaseTests<PushNotificationsToken>
        {
            [Fact]
            public async Task AllowsToUnsubscribeTheSameTokenRepeatedly()
            {
                var (togglApi, _) = await SetupTestUser();
                var token = randomToken();
                await togglApi.PushServices.Subscribe(token);

                await togglApi.PushServices.Unsubscribe(token);
                await togglApi.PushServices.Unsubscribe(token);

                // no exception was thrown
            }

            [Fact]
            public async Task AllowsToUnsubscribeTokenWhichWasNeverRegisteredInTheFirstPlace()
            {
                var (togglApi, _) = await SetupTestUser();
                var token = randomToken();

                await togglApi.PushServices.Unsubscribe(token);

                // no exception was thrown
            }

            protected override IObservable<PushNotificationsToken> Initialize(ITogglApi api)
            {
                var token = randomToken();
                return api.PushServices.Subscribe(token).SelectValue(token);
            }

            protected override IObservable<Unit> Delete(ITogglApi api, PushNotificationsToken entity)
                => api.PushServices.Unsubscribe(entity);
        }

        private static PushNotificationsToken randomToken()
            => new PushNotificationsToken(Guid.NewGuid().ToString());
    }
}
