using System;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.Interactors;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.Sync.States.Push
{
    public sealed class SyncPushNotificationsTokenState : ISyncState
    {
        private readonly IInteractorFactory interactorFactory;

        public StateResult Done { get; } = new StateResult();

        public SyncPushNotificationsTokenState(IInteractorFactory interactorFactory)
        {
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));

            this.interactorFactory = interactorFactory;
        }

        public IObservable<ITransition> Start()
            => interactorFactory.SubscribeToPushNotifications().Execute()
                .Catch(Observable.Return(Unit.Default))
                .SelectValue(Done.Transition());
    }
}
