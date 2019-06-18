using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
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

        public sealed class GetAllStoredTokens : AuthenticatedGetAllEndpointBaseTests<PushNotificationsToken>
        {
            [Fact, LogTestInfo]
            public async Task ReturnsAllSubscribedTokens()
            {
                var (togglClient, user) = await SetupTestUser();
                var firstToken = new PushNotificationsToken(Guid.NewGuid().ToString());
                await togglClient.PushServices.Subscribe(firstToken);
                var secondToken = new PushNotificationsToken(Guid.NewGuid().ToString());
                await togglClient.PushServices.Subscribe(secondToken);

                var tokens = await CallEndpointWith(togglClient);

                tokens.Should().HaveCount(2)
                    .And.Contain(firstToken)
                    .And.Contain(secondToken);
            }

            [Fact, LogTestInfo]
            public async Task DoesNotReturnUnsubscribedToken()
            {
                var (togglClient, user) = await SetupTestUser();
                var firstToken = new PushNotificationsToken(Guid.NewGuid().ToString());
                await togglClient.PushServices.Subscribe(firstToken);
                var secondToken = new PushNotificationsToken(Guid.NewGuid().ToString());
                await togglClient.PushServices.Subscribe(secondToken);
                await togglClient.PushServices.Unsubscribe(firstToken);

                var tokens = await CallEndpointWith(togglClient);

                tokens.Should().HaveCount(1).And.Contain(secondToken);
            }

            protected override IObservable<List<PushNotificationsToken>> CallEndpointWith(ITogglApi togglApi)
                => togglApi.PushServices.GetAll();
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
