using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Core.Interactors;
using Toggl.Core.Sync.States.Push;
using Xunit;

namespace Toggl.Core.Tests.Sync.States.Push
{
    public sealed class SyncPushNotificationsTokenStateTests
    {
        public sealed class TheConstructor
        {
            [Fact]
            public void ThrowsWhenArgumentIsNull()
            {
                Action constructor = () => new SyncPushNotificationsTokenState(null);

                constructor.Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheStartMehtod
        {
            private readonly IInteractorFactory factory = Substitute.For<IInteractorFactory>();
            private readonly SyncPushNotificationsTokenState state;

            public TheStartMehtod()
            {
                state = new SyncPushNotificationsTokenState(factory);
            }

            [Fact]
            public async Task ReturnsDoneIfInteractorWorked()
            {
                var unit = Observable.Return(Unit.Default);
                factory.SubscribeToPushNotifications().Execute().Returns(unit);

                var transition = await state.Start();

                transition.Result.Should().Be(state.Done);
            }

            [Fact]
            public async Task ReturnsDoneEvenIfInteractorThrows()
            {
                var throwingObservable = Observable.Throw<Unit>(new Exception());
                factory.SubscribeToPushNotifications().Execute().Returns(throwingObservable);

                var transition = await state.Start();

                transition.Result.Should().Be(state.Done);
            }
        }
    }
}
