using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Core.Interactors.PushNotifications;
using Toggl.Core.Tests.Generators;
using Toggl.Networking.ApiClients;
using Toggl.Shared;
using Xunit;
using static Toggl.Core.Interactors.PushNotificationTokenKeys;

namespace Toggl.Core.Tests.Interactors.PushNotifications
{
    public class SubscribeToPushNotificationsInteractorTests : BaseInteractorTests
    {
        public sealed class TheConstructor : BaseInteractorTests
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useKeyValueStorage, bool usePushServicesApi, bool usePushNotificationTokenService)
            {
                Action tryingToConstructWithNull = () => new SubscribeToPushNotificationsInteractor(
                    useKeyValueStorage ? KeyValueStorage : null,
                    usePushServicesApi ? Api : null,
                    usePushNotificationTokenService ? PushNotificationsTokenService : null);

                tryingToConstructWithNull.Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheExecuteMethod : BaseInteractorTests
        {
            private SubscribeToPushNotificationsInteractor interactor;
            private IPushServicesApi pushServicesApi = Substitute.For<IPushServicesApi>();

            public TheExecuteMethod()
            {
                Api.PushServices.Returns(pushServicesApi);
                interactor = new SubscribeToPushNotificationsInteractor(KeyValueStorage, Api, PushNotificationsTokenService);
            }

            [Fact]
            public async Task DoesNothingWhenPushNotificationsTokenServiceTokenIsNull()
            {
                PushNotificationsTokenService.Token?.Returns(null);

                (await interactor.Execute().SingleAsync()).Should().Be(Unit.Default);
                pushServicesApi.DidNotReceive().Subscribe(Arg.Any<PushNotificationsToken>());
                pushServicesApi.DidNotReceive().Unsubscribe(Arg.Any<PushNotificationsToken>());
            }

            [Fact]
            public async Task DoesNothingWhenPushNotificationsTokenServiceTokenIsEmpty()
            {
                PushNotificationsTokenService.Token.Returns(default(PushNotificationsToken));

                (await interactor.Execute().SingleAsync()).Should().Be(Unit.Default);
                pushServicesApi.DidNotReceive().Subscribe(Arg.Any<PushNotificationsToken>());
                pushServicesApi.DidNotReceive().Unsubscribe(Arg.Any<PushNotificationsToken>());
            }

            [Fact]
            public async Task DoesNothingWhenTheTokenHasAlreadyBeenRegistered()
            {
                KeyValueStorage.GetString(PreviouslyRegisteredTokenKey).Returns("tokenA");
                PushNotificationsTokenService.Token.Returns(new PushNotificationsToken("tokenA"));

                (await interactor.Execute().SingleAsync()).Should().Be(Unit.Default);
                pushServicesApi.DidNotReceive().Subscribe(Arg.Any<PushNotificationsToken>());
                pushServicesApi.DidNotReceive().Unsubscribe(Arg.Any<PushNotificationsToken>());
            }

            [Fact]
            public async Task CallsTheApiToSubscribeForPushNotificationsWhenNoOtherTokenHasBeenStored()
            {
                KeyValueStorage.GetString(PreviouslyRegisteredTokenKey).Returns(default(string));
                var expectedPushNotificationToken = new PushNotificationsToken("tokenA");
                PushNotificationsTokenService.Token.Returns(expectedPushNotificationToken);
                pushServicesApi.Subscribe(Arg.Any<PushNotificationsToken>()).Returns(Observable.Return(Unit.Default));

                (await interactor.Execute().SingleAsync()).Should().Be(Unit.Default);
                pushServicesApi.Received().Subscribe(expectedPushNotificationToken);
            }

            [Fact]
            public async Task StoresTheTokenWhenSucceedsToRegisterIt()
            {
                KeyValueStorage.GetString(PreviouslyRegisteredTokenKey).Returns(default(string));
                var expectedPushNotificationToken = new PushNotificationsToken("tokenA");
                PushNotificationsTokenService.Token.Returns(expectedPushNotificationToken);
                pushServicesApi.Subscribe(Arg.Any<PushNotificationsToken>()).Returns(Observable.Return(Unit.Default));

                (await interactor.Execute().SingleAsync()).Should().Be(Unit.Default);
                KeyValueStorage.Received().SetString(PreviouslyRegisteredTokenKey, expectedPushNotificationToken.ToString());
            }

            [Fact]
            public async Task DoesNotStoreTheTokenWhenItFailsToRegisterIt()
            {
                KeyValueStorage.GetString(PreviouslyRegisteredTokenKey).Returns(default(string));
                var expectedPushNotificationToken = new PushNotificationsToken("tokenA");
                PushNotificationsTokenService.Token.Returns(expectedPushNotificationToken);
                pushServicesApi.Subscribe(Arg.Any<PushNotificationsToken>()).Returns(Observable.Throw<Unit>(new Exception()));

                (await interactor.Execute().SingleAsync()).Should().Be(Unit.Default);
                pushServicesApi.Received().Subscribe(expectedPushNotificationToken);
                KeyValueStorage.DidNotReceive().SetString(PreviouslyRegisteredTokenKey, expectedPushNotificationToken.ToString());
            }
        }
    }
}