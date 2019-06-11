using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using Toggl.Core.Interactors;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Shared;
using Xunit;

namespace Toggl.Core.Tests.Interactors.PushNotifications
{
    public class UnsubscribeFromPushNotificationsInteractorTests : BaseInteractorTests
    {
        private readonly IInteractor<IObservable<Unit>> interactor;

        public UnsubscribeFromPushNotificationsInteractorTests()
        {
            interactor = new UnsubscribeFromPushNotificationsInteractor(
                PushNotificationsTokenService,
                KeyValueStorage,
                Api);
        }

        [Fact, LogIfTooSlow]
        public async Task ClearsTheKeyValueStorage()
        {
            await interactor.Execute();

            KeyValueStorage.Received().Remove(PushNotificationTokenKeys.PreviouslyRegisteredTokenKey);
        }

        [Fact, LogIfTooSlow]
        public async Task InvalidatesTheToken()
        {
            await interactor.Execute();

            PushNotificationsTokenService.Received().InvalidateCurrentToken();
        }

        [Fact, LogIfTooSlow]
        public async Task UnsubscribesFromNotifications()
        {
            var token = new PushNotificationsToken("token");
            PushNotificationsTokenService.Token.Returns(token);

            await interactor.Execute();

            Api.PushServices.Received().Unsubscribe(token);
        }

        [Fact, LogIfTooSlow]
        public async Task DoesntErrorOutIfTokenIsNull()
        {
            PushNotificationsTokenService.Token.Returns(_ => null);

            var testScheduler = new TestScheduler();
            var observer = testScheduler.CreateObserver<Unit>();

            interactor.Execute().Subscribe(observer);
            testScheduler.Start();

            observer.SingleEmittedValue().Should().Be(Unit.Default);
        }

        [Fact, LogIfTooSlow]
        public async Task IgnoresServerErrors()
        {
            var token = new PushNotificationsToken("token");
            PushNotificationsTokenService.Token.Returns(token);
            Api.PushServices.Unsubscribe(token).Returns(Observable.Throw<Unit>(new Exception("Whatever")));

            var testScheduler = new TestScheduler();
            var observer = testScheduler.CreateObserver<Unit>();

            interactor.Execute().Subscribe(observer);
            testScheduler.Start();

            observer.SingleEmittedValue().Should().Be(Unit.Default);
        }
    }
}
