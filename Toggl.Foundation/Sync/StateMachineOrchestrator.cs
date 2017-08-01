using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Multivac;

namespace Toggl.Foundation.Sync
{
    internal sealed class StateMachineOrchestrator : IStateMachineOrchestrator
    {
        private readonly IStateMachine stateMachine;
        private readonly StateMachineEntryPoints entryPoints;

        private readonly BehaviorSubject<SyncState> stateEntered = new BehaviorSubject<SyncState>(SyncState.Sleep);
        private readonly Subject<SyncState> syncComplete = new Subject<SyncState>();
        
        private bool syncing;

        public SyncState State => stateEntered.Value;
        public IObservable<SyncState> StateObservable { get; }
        public IObservable<SyncState> SyncCompleteObservable { get; }

        public StateMachineOrchestrator(IStateMachine stateMachine, StateMachineEntryPoints entryPoints)
        {
            Ensure.Argument.IsNotNull(stateMachine, nameof(stateMachine));
            Ensure.Argument.IsNotNull(entryPoints, nameof(entryPoints));

            this.stateMachine = stateMachine;
            this.entryPoints = entryPoints;
            StateObservable = stateEntered.AsObservable();
            SyncCompleteObservable = syncComplete.AsObservable();

            stateMachine.StateTransitions.Subscribe(onStateEvent);
        }

        private void onStateEvent(StateMachineEvent @event)
        {
            if (@event is StateMachineDeadEnd || @event is StateMachineError)
                completeCurrentSync();
        }

        private void completeCurrentSync()
        {
            syncing = false;
            syncComplete.OnNext(State);
        }

        public void StartPushSync()
        {
            startSync(SyncState.Push, entryPoints.StartPushSync);
        }

        public void StartPullSync()
        {
            startSync(SyncState.Pull, entryPoints.StartPullSync);
        }

        private void startSync(SyncState newState, StateResult entryPoint)
        {
            ensureNotSyncing();

            syncing = true;
            stateEntered.OnNext(newState);
            stateMachine.Start(entryPoint.Transition());
        }

        public void GoToSleep()
        {
            ensureNotSyncing();

            stateEntered.OnNext(SyncState.Sleep);
        }
        
        private void ensureNotSyncing()
        {
            if (syncing)
                throw new InvalidOperationException("Cannot start syncing again if already in progress.");
        }
    }
}
