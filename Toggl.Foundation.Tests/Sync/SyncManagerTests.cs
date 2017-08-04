using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Foundation.Sync;
using Xunit;
using FsCheck.Xunit;
using Toggl.Foundation.Tests.Generators;
using static Toggl.Foundation.Sync.SyncState;

namespace Toggl.Foundation.Tests.Sync
{
    public class SyncManagerTests
    {
        public abstract class SyncManagerTestBase
        {
            protected Subject<SyncState> OrchestratorSyncComplete { get; } = new Subject<SyncState>();
            protected Subject<SyncState> OrchestratorStates { get; } = new Subject<SyncState>();
            protected ISyncStateQueue Queue { get; } = Substitute.For<ISyncStateQueue>();
            protected IStateMachineOrchestrator Orchestrator { get; } = Substitute.For<IStateMachineOrchestrator>();
            protected SyncManager SyncManager { get; }

            protected SyncManagerTestBase()
            {
                Orchestrator.SyncCompleteObservable.Returns(OrchestratorSyncComplete.AsObservable());
                Orchestrator.StateObservable.Returns(OrchestratorStates.AsObservable());
                SyncManager = new SyncManager(Queue, Orchestrator);
            }
        }

        public class TheConstuctor : SyncManagerTestBase
        {
            [Theory]
            [ClassData(typeof(TwoParameterConstructorTestData))]
            public void ThrowsIfAnyArgumentIsNull(bool useQueue, bool useOrchestrator)
            {
                var queue = useQueue ? Queue : null;
                var orchestrator = useOrchestrator ? Orchestrator : null;

                // ReSharper disable once ObjectCreationAsStatement
                Action constructor = () => new SyncManager(queue, orchestrator);

                constructor.ShouldThrow<ArgumentNullException>();
            }
        }

        public class TheStateProperty : SyncManagerTestBase
        {
            [Property]
            public void ShouldReturnStateFromOrchestrator(int stateValue)
            {
                Orchestrator.State.Returns((SyncState)stateValue);

                SyncManager.State.Should().Be((SyncState)stateValue);
            }
        }

        public class TheStateObservable : SyncManagerTestBase
        {
            [Fact]
            public void ShouldReturnObservableFromOrchestrator()
            {
                var expectedObservable = Substitute.For<IObservable<SyncState>>();
                Orchestrator.StateObservable.Returns(expectedObservable);

                SyncManager.StateObservable.Should().Be(expectedObservable);
            }
        }

        public abstract class ThreadSafeQueingMethodTests : SyncManagerTestBase
        {
            protected abstract void CallMethod();

            [Fact]
            public void AgainTellsQueueToStartSyncAfterCompletingPreviousFullSync()
            {
                Queue.StartNextQueuedState(Orchestrator).Returns(Pull);
                SyncManager.ForceFullSync();
                Queue.StartNextQueuedState(Orchestrator).Returns(Sleep);
                OrchestratorSyncComplete.OnNext(Pull);
                Queue.ClearReceivedCalls();

                CallMethod();

                Queue.Received().StartNextQueuedState(Orchestrator);
            }

            [Fact]
            public void AgainTellsQueueToStartSyncAfterCompletingPreviousPushSync()
            {
                Queue.StartNextQueuedState(Orchestrator).Returns(Pull);
                SyncManager.PushSync();
                Queue.StartNextQueuedState(Orchestrator).Returns(Sleep);
                OrchestratorSyncComplete.OnNext(Push);
                Queue.ClearReceivedCalls();

                CallMethod();

                Queue.Received().StartNextQueuedState(Orchestrator);
            }

            [Fact]
            public async Task DoesNotQueueUntilOtherCompletedEventReturns()
            {
                await ensureMethodIsThreadSafeWith(() => OrchestratorSyncComplete.OnNext(0));
            }

            [Fact]
            public async Task DoesNotQueueUntilOtherCallToPushSyncReturns()
            {
                await ensureMethodIsThreadSafeWith(() => SyncManager.PushSync());
            }

            [Fact]
            public async Task DoesNotQueueUntilOtherCallToForceFullSyncReturns()
            {
                await ensureMethodIsThreadSafeWith(() => SyncManager.ForceFullSync());
            }

            private async Task ensureMethodIsThreadSafeWith(Action otherMethod)
            {
                var endFirstCall = new AutoResetEvent(false);
                var startedFirstCall = new AutoResetEvent(false);
                var startedSecondCall = new AutoResetEvent(false);
                var endedSecondCall = new AutoResetEvent(false);
                var isFirstCall = true;
                Queue.StartNextQueuedState(Orchestrator).Returns(Sleep).AndDoes(_ =>
                {
                    // the second time we call this we don't want to do anything
                    if (!isFirstCall) return;
                    isFirstCall = false;
                    startedFirstCall.Set();
                    endFirstCall.WaitOne();
                });
                var firstCall = new Thread(() =>
                {
                    otherMethod();
                });
                firstCall.Start();
                // ensure the first call gets inside locked code before starting second one
                startedFirstCall.WaitOne();
                
                var endedSecondCallBool = false;
                var secondCall = new Thread(() =>
                {
                    startedSecondCall.Set();
                    // here is where we call the method that should be blocked until the first returns
                    CallMethod();
                    endedSecondCallBool = true;
                    endedSecondCall.Set();
                });
                secondCall.Start();
                startedSecondCall.WaitOne();

                // ensure that the second call has time to get to the lock
                // this could probably do with much less time, but I rather make sure
                await Task.Delay(100);

                // the first call is still stuck on `endFirstCall`, so the second has to wait
                endedSecondCallBool.Should().BeFalse();

                // end the first call, which should trigger the second to enter the lock,
                // and complete soon after
                endFirstCall.Set();
                endedSecondCall.WaitOne();

                endedSecondCallBool.Should().BeTrue();
            }
        }

        public class TheOrchestratorCompleteObservable : ThreadSafeQueingMethodTests
        {
            protected override void CallMethod()
                => OrchestratorSyncComplete.OnNext(0);

            [Fact]
            public void TellsQueueToStartSync()
            {
                CallMethod();

                Queue.Received().StartNextQueuedState(Orchestrator);
            }

            [Fact]
            public void DoesNotQueuePullSync()
            {
                CallMethod();

                Queue.DidNotReceive().QueuePullSync();
            }

            [Fact]
            public void DoesNotQueuePushSync()
            {
                CallMethod();

                Queue.DidNotReceive().QueuePullSync();
            }

            [Fact]
            public void TellsQueueToStartOrchestratorWhenAlreadyRunningFullSync()
            {
                Queue.StartNextQueuedState(Orchestrator).Returns(Pull);
                SyncManager.ForceFullSync();
                Queue.ClearReceivedCalls();

                CallMethod();

                Queue.Received().StartNextQueuedState(Orchestrator);
            }

            [Fact]
            public void TellsQueueToStartOrchestratorWhenAlreadyRunningPushSync()
            {
                Queue.StartNextQueuedState(Orchestrator).Returns(Pull);
                SyncManager.PushSync();
                Queue.ClearReceivedCalls();

                CallMethod();

                Queue.Received().StartNextQueuedState(Orchestrator);
            }

            [Fact]
            public void TellsQueueToStartOrchestratorWhenInSecondPartOfMultiPhaseSync()
            {
                Queue.StartNextQueuedState(Orchestrator).Returns(Pull);
                SyncManager.ForceFullSync();
                OrchestratorSyncComplete.OnNext(Push);
                Queue.ClearReceivedCalls();

                CallMethod();

                Queue.Received().StartNextQueuedState(Orchestrator);
            }
        }

        public abstract class SyncMethodTests : ThreadSafeQueingMethodTests
        {
            protected override void CallMethod() => CallSyncMethod();

            protected abstract IObservable<SyncState> CallSyncMethod();

            [Property]
            public void ReturnsObservableThatReplaysSyncStatesUntilSleep(bool[] beforeSleep, bool[] afterSleep)
            {
                var observable = CallSyncMethod();
                
                var beforeSleepStates = (beforeSleep ?? Enumerable.Empty<bool>())
                    .Select(b => b ? Push : Pull);
                var afterSleepStates = (afterSleep ?? Enumerable.Empty<bool>())
                    .Select(b => b ? Push : Pull);

                var expectedStates = beforeSleepStates.Concat(new[] { Sleep }).ToList();

                foreach (var states in expectedStates.Concat(afterSleepStates))
                    OrchestratorStates.OnNext(states);

                var actual = observable.ToList().Wait();

                actual.ShouldBeEquivalentTo(expectedStates);
            }

            [Fact]
            public void DoesNotTellQueueToStartOrchestratorWhenAlreadyRunningFullSync()
            {
                Queue.StartNextQueuedState(Orchestrator).Returns(Pull);
                SyncManager.ForceFullSync();
                Queue.ClearReceivedCalls();

                CallMethod();

                Queue.DidNotReceive().StartNextQueuedState(Orchestrator);
            }

            [Fact]
            public void DoesNotTellQueueToStartOrchestratorWhenAlreadyRunningPushSync()
            {
                Queue.StartNextQueuedState(Orchestrator).Returns(Pull);
                SyncManager.PushSync();
                Queue.ClearReceivedCalls();

                CallMethod();

                Queue.DidNotReceive().StartNextQueuedState(Orchestrator);
            }
            
            [Fact]
            public void DoesNotTellQueueToStartOrchestratorWhenInSecondPartOfMultiPhaseSync()
            {
                Queue.StartNextQueuedState(Orchestrator).Returns(Pull);
                SyncManager.ForceFullSync();
                OrchestratorSyncComplete.OnNext(Push);
                Queue.ClearReceivedCalls();

                CallMethod();

                Queue.DidNotReceive().StartNextQueuedState(Orchestrator);
            }
        }

        public class ThePushSyncMethod : SyncMethodTests
        {
            protected override IObservable<SyncState> CallSyncMethod()
                => SyncManager.PushSync();

            [Fact]
            public void TellsQueueToStartSyncAfterQueingPush()
            {
                CallMethod();

                Received.InOrder(() =>
                {
                    Queue.QueuePushSync();
                    Queue.StartNextQueuedState(Orchestrator);
                });
            }
        }

        public class TheForceFullSyncMethod : SyncMethodTests
        {
            protected override IObservable<SyncState> CallSyncMethod()
                => SyncManager.ForceFullSync();

            [Fact]
            public void TellsQueueToStartSyncAfterQueingPull()
            {
                CallMethod();

                Received.InOrder(() =>
                {
                    Queue.QueuePullSync();
                    Queue.StartNextQueuedState(Orchestrator);
                });
            }
        }
    }
}
