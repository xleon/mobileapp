using System;
using System.Collections.Generic;
using System.Linq;
using FsCheck.Xunit;
using Toggl.Foundation.Sync;
using static Toggl.Foundation.Sync.SyncState;
using Xunit;

namespace Toggl.Foundation.Tests.Sync
{
    public class SyncStateQueueTests
    {
        private class OrchestratorMock : IStateMachineOrchestrator
        {
            private readonly List<SyncState> calls;

            public SyncState State => throw new NotImplementedException();
            public IObservable<SyncState> StateObservable => throw new NotImplementedException();
            public IObservable<SyncState> SyncCompleteObservable => throw new NotImplementedException();

            public OrchestratorMock(List<SyncState> calls)
            {
                this.calls = calls;
            }

            public void StartPushSync() => calls.Add(Push);
            public void StartPullSync() => calls.Add(Pull);
            public void GoToSleep() => calls.Add(Sleep);
        }

        public class TheStartNextQueuedStateMethod
        {
            private readonly SyncStateQueue queue = new SyncStateQueue();
            private readonly List<SyncState> orchestratorCalls = new List<SyncState>();
            private readonly IStateMachineOrchestrator orchestrator;

            public TheStartNextQueuedStateMethod()
            {
                orchestrator = new OrchestratorMock(orchestratorCalls);
            }

            private void queueSyncs(params SyncState[] states)
            {
                foreach (var state in states)
                    switch (state)
                    {
                        case Pull:
                            queue.QueuePullSync();
                            break;
                        case Push:
                            queue.QueuePushSync();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
            }

            private SyncState callMethod()
            {
                return queue.StartNextQueuedState(orchestrator);
            }

            private List<SyncState> callMethodUntilSleep()
            {
                var returnValues = new List<SyncState>();

                while (true)
                {
                    returnValues.Add(callMethod());

                    if (orchestratorCalls.Last() == Sleep)
                        break;
                }

                return returnValues;
            }

            [Fact]
            public void StartsSleepIfNothingQueued()
            {
                callMethod();

                orchestratorCalls.ShouldBeSameEventsAs(
                    Sleep
                );
            }

            [Fact]
            public void StartsPullIfOnlyPullQueued()
            {
                queueSyncs(Pull);

                callMethod();

                orchestratorCalls.ShouldBeSameEventsAs(
                    Pull
                );
            }

            [Fact]
            public void StartsPushIfOnlyPushQueued()
            {
                queueSyncs(Push);

                callMethod();

                orchestratorCalls.ShouldBeSameEventsAs(
                    Push
                );
            }

            [Property]
            public void AlwaysReturnsTheStateItRuns(bool[] pushPull)
            {
                orchestratorCalls.Clear();
                var states = (pushPull ?? Enumerable.Empty<bool>())
                    .Select(b => b ? Push : Pull).ToArray();
                queueSyncs(states);

                var returnValues = callMethodUntilSleep();

                returnValues.ShouldBeSameEventsAs(orchestratorCalls.ToArray());
            }

            [Property]
            public void RunsFullCycleOnceNoMatterWhatIsQueuedIfPullIsQueued(bool[] pushPull)
            {
                orchestratorCalls.Clear();
                var states = (pushPull ?? Enumerable.Empty<bool>())
                    .Select(b => b ? Push : Pull).ToArray();
                queueSyncs(states);
                if (!states.Contains(Pull))
                    queueSyncs(Pull);

                callMethodUntilSleep();

                orchestratorCalls.ShouldBeSameEventsAs(
                    Pull, Push, Sleep
                );
            }

            [Fact]
            public void RunsFullCycleIfPullIsQueuedAfterPushStarted()
            {
                queueSyncs(Push);
                callMethod();
                queueSyncs(Pull);

                callMethodUntilSleep();

                orchestratorCalls.ShouldBeSameEventsAs(
                    Push, Pull, Push, Sleep
                );
            }

            [Fact]
            public void RunsTwoFullCyclesIfPullIsQueuedAfterFirstPullStarted()
            {
                queueSyncs(Pull);
                callMethod();
                queueSyncs(Pull);

                callMethodUntilSleep();

                orchestratorCalls.ShouldBeSameEventsAs(
                    Pull, Push, Pull, Push, Sleep
                );
            }
        }
    }
}
