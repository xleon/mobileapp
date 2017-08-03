using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FluentAssertions;
using NSubstitute;
using Toggl.Foundation.Sync;
using Toggl.Foundation.Tests.Generators;
using Toggl.Multivac;
using Xunit;
using static Toggl.Foundation.Sync.SyncState;

namespace Toggl.Foundation.Tests.Sync
{
    public class StateMachineOrchestratorTests
    {
        public abstract class StateMachineOrchestratorBaseTests
        {
            private readonly Subject<StateMachineEvent> stateMachineEventSubject
                = new Subject<StateMachineEvent>();

            protected IStateMachine StateMachine = Substitute.For<IStateMachine>();
            protected StateMachineEntryPoints EntryPoints { get; } = new StateMachineEntryPoints();
            protected IStateMachineOrchestrator Orchestrator { get; }
            protected List<SyncState> StateEvents { get; } = new List<SyncState>();
            protected List<SyncState> CompletedEvents { get; } = new List<SyncState>();

            protected StateMachineOrchestratorBaseTests()
            {
                StateMachine.StateTransitions.Returns(stateMachineEventSubject.AsObservable());

                Orchestrator = new StateMachineOrchestrator(StateMachine, EntryPoints);

                Orchestrator.StateObservable.Subscribe(StateEvents.Add);
                Orchestrator.SyncCompleteObservable.Subscribe(CompletedEvents.Add);
            }

            protected void SendStateMachineEvent(StateMachineEvent @event)
            {
                stateMachineEventSubject.OnNext(@event);
            }
        }

        public class TheConstructor
        {
            [Theory]
            [ClassData(typeof(TwoParameterConstructorTestData))]
            public void ThrowsIfAnyArgumentIsNull(bool useStateMachine, bool useEntryPoints)
            {
                var stateMachine = useStateMachine ? Substitute.For<IStateMachine>() : null;
                var entryPoints = useEntryPoints ? new StateMachineEntryPoints() : null;
                
                // ReSharper disable once ObjectCreationAsStatement
                Action constructing = () => new StateMachineOrchestrator(stateMachine, entryPoints);

                constructing.ShouldThrow<ArgumentNullException>();
            }

            [Fact]
            public void SubscribesToStateMachineEvents()
            {
                var stateMachine = Substitute.For<IStateMachine>();
                var transitions = Substitute.For<IObservable<StateMachineEvent>>();
                stateMachine.StateTransitions.Returns(transitions);

                // ReSharper disable once ObjectCreationAsStatement
                new StateMachineOrchestrator(stateMachine, new StateMachineEntryPoints());
                
                transitions.Received().Subscribe(Arg.Any<IObserver<StateMachineEvent>>());
            }
        }

        public class TheStateProperty : StateMachineOrchestratorBaseTests
        {
            [Fact]
            public void StartsWithSleep()
            {
                Orchestrator.State.Should().Be(Sleep);
            }
        }
        
        public abstract class StateChangeMethodTests : StateMachineOrchestratorBaseTests
        {
            protected abstract SyncState ExpectedState { get; }
            protected abstract void CallMethod();

            private Action callingMethod => CallMethod;

            [Fact]
            public void DoesNotThrowIfNotSyncing()
            {
                callingMethod.ShouldNotThrow();
            }

            [Fact]
            public void DoesNotThrowIfSleepWasCalledLast()
            {
                Orchestrator.GoToSleep();

                callingMethod.ShouldNotThrow();
            }

            [Fact]
            public void ShouldThrowIfPullSyncing()
            {
                Orchestrator.StartPullSync();

                callingMethod.ShouldThrow<InvalidOperationException>();
            }

            [Fact]
            public void ShouldThrowIfPushSyncing()
            {
                Orchestrator.StartPushSync();

                callingMethod.ShouldThrow<InvalidOperationException>();
            }

            [Fact]
            public void ShouldNotThrowIfPullSyncingCompleted()
            {
                Orchestrator.StartPullSync();
                SendStateMachineEvent(new StateMachineDeadEnd(null));

                callingMethod.ShouldNotThrow();
            }

            [Fact]
            public void ShouldNotThrowIfPushSyncingCompleted()
            {
                Orchestrator.StartPushSync();
                SendStateMachineEvent(new StateMachineDeadEnd(null));

                callingMethod.ShouldNotThrow();
            }

            [Fact]
            public void ShouldNotThrowIfPullSyncingFailed()
            {
                Orchestrator.StartPullSync();
                SendStateMachineEvent(new StateMachineError(null));

                callingMethod.ShouldNotThrow();
            }

            [Fact]
            public void ShouldNotThrowIfPushSyncingFailed()
            {
                Orchestrator.StartPushSync();
                SendStateMachineEvent(new StateMachineError(null));

                callingMethod.ShouldNotThrow();
            }

            [Fact]
            public void ShouldResultInExpectedState()
            {
                CallMethod();

                Orchestrator.State.Should().Be(ExpectedState);
            }

            [Fact]
            public void ShouldCauseExpectedStateEvent()
            {
                StateEvents.Clear();

                CallMethod();

                StateEvents.ShouldBeSameEventsAs(
                    ExpectedState
                );
            }

            [Fact]
            public void ShouldNotCauseCompletedEvent()
            {
                CallMethod();

                CompletedEvents.Should().BeEmpty();
            }
        }

        public abstract class PullPushSyncMethodTests : StateChangeMethodTests
        {
            protected abstract StateResult EntryPoint { get; }
            
            [Fact]
            public void ShouldStartStateMachineWithCorrectEntryPoint()
            {
                CallMethod();

                StateMachine.Received().Start(
                    Arg.Is<ITransition>(t => t.Result == EntryPoint)
                );
            }

            [Fact]
            public void ShouldCauseExpectedCompletedEventWhenSyncingCompletes()
            {
                CallMethod();
                SendStateMachineEvent(new StateMachineDeadEnd(null));

                CompletedEvents.ShouldBeSameEventsAs(
                    ExpectedState
                );
            }

            [Fact]
            public void ShouldCauseExpectedCompletedEventWhenSyncingFails()
            {
                CallMethod();
                SendStateMachineEvent(new StateMachineError(null));

                CompletedEvents.ShouldBeSameEventsAs(
                    ExpectedState
                );
            }
        }

        public class TheStartPullSyncMethod : PullPushSyncMethodTests
        {
            protected override SyncState ExpectedState => Pull;
            protected override StateResult EntryPoint => EntryPoints.StartPullSync;
            protected override void CallMethod() => Orchestrator.StartPullSync();
        }

        public class TheStartPushSyncMethod : PullPushSyncMethodTests
        {
            protected override SyncState ExpectedState => Push;
            protected override StateResult EntryPoint => EntryPoints.StartPushSync;
            protected override void CallMethod() => Orchestrator.StartPushSync();
        }

        public class ThGoToSleepMethod : StateChangeMethodTests
        {
            protected override SyncState ExpectedState => Sleep;
            protected override void CallMethod() => Orchestrator.GoToSleep();
            
            [Fact]
            public void ShouldNotStartStateMachine()
            {
                CallMethod();

                StateMachine.DidNotReceive().Start(Arg.Any<ITransition>());
            }
        }
    }

    internal static class StateMachineOrchestratorTestExtensions
    {
        public static void ShouldBeSameEventsAs(this List<SyncState> actualEvents,
            params SyncState[] expectedEvents)
        {
            Ensure.Argument.IsNotNull(expectedEvents, nameof(expectedEvents));

            actualEvents.Should().HaveCount(expectedEvents.Length);

            for (var i = 0; i < expectedEvents.Length; i++)
            {
                var actual = actualEvents[i];
                var expected = expectedEvents[i];

                try
                {
                    actual.Should().Be(expected);
                }
                catch (Exception e)
                {
                    throw new Exception($"Found unexpected event at index {i}.", e);
                }
            }
        }
    }
}
