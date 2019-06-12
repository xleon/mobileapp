using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Core.Interactors;
using Toggl.Core.Services;
using Toggl.Core.Sync.States.Push;
using Toggl.Core.Tests.Generators;
using Toggl.Networking;
using Toggl.Shared;
using Toggl.Storage.Settings;
using Xunit;

namespace Toggl.Core.Tests.Sync.States.Push
{
    public sealed class SyncPushNotificationsTokenStateTests
    {
        public sealed class TheConstructor
        {
            [Theory]
            [ConstructorData]
            public void ThrowsWhenArgumentIsNull(bool useKeyValueStorage, bool useTogglApi, bool usePushNotificationsTokenService)
            {
                var keyValueStorage = useKeyValueStorage ? Substitute.For<IKeyValueStorage>() : null;
                var togglApi = useTogglApi ? Substitute.For<ITogglApi>() : null;
                var pushNotificationsTokenService = usePushNotificationsTokenService
                    ? Substitute.For<IPushNotificationsTokenService>()
                    : null;

                Action constructor = () => new SyncPushNotificationsTokenState(keyValueStorage, togglApi, pushNotificationsTokenService);

                constructor.Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheStartMehtod
        {
            private readonly IKeyValueStorage keyValueStorage = Substitute.For<IKeyValueStorage>();
            private readonly ITogglApi togglApi = Substitute.For<ITogglApi>();
            private readonly IPushNotificationsTokenService pushNotificationsTokenService = Substitute.For<IPushNotificationsTokenService>();

            private readonly SyncPushNotificationsTokenState state;

            public TheStartMehtod()
            {
                state = new SyncPushNotificationsTokenState(keyValueStorage, togglApi, pushNotificationsTokenService);
            }

            [Fact]
            public async Task ReturnsDoneIfInteractorWorked()
            {
                var unit = Observable.Return(Unit.Default);
                togglApi.PushServices.Subscribe(Arg.Any<PushNotificationsToken>()).Returns(unit);
                keyValueStorage.GetString(PushNotificationTokenKeys.PreviouslyRegisteredTokenKey).Returns("token");

                var transition = await state.Start();

                transition.Result.Should().Be(state.Done);
            }

            [Fact]
            public async Task DoesNotMakeAnApiRequestIfThereIsNotApiToken()
            {
                pushNotificationsTokenService.Token.Returns(default(PushNotificationsToken));

                await state.Start();

                await togglApi.PushServices.DidNotReceive().Subscribe(Arg.Any<PushNotificationsToken>());
            }

            [Theory]
            [InlineData("")]
            [InlineData(null)]
            public async Task DoesNotMakeAnApiRequestIfTheTokenIsEmpty(string token)
            {
                keyValueStorage.GetString(PushNotificationTokenKeys.PreviouslyRegisteredTokenKey).Returns(token);

                await state.Start();

                await togglApi.PushServices.DidNotReceive().Subscribe(Arg.Any<PushNotificationsToken>());
            }

            [Fact]
            public async Task ReturnsDoneEvenIfApiFailsThrows()
            {
                var throwingObservable = Observable.Throw<Unit>(new Exception());
                togglApi.PushServices.Subscribe(Arg.Any<PushNotificationsToken>()).Returns(throwingObservable);
                keyValueStorage.GetString(PushNotificationTokenKeys.PreviouslyRegisteredTokenKey).Returns("token");

                var transition = await state.Start();

                transition.Result.Should().Be(state.Done);
            }
        }
    }
}