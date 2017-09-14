using System;
using System.Collections.Generic;
using System.Linq;
using FsCheck.Xunit;
using Toggl.Foundation.Sync;
using static Toggl.Foundation.Sync.SyncState;
using Xunit;

namespace Toggl.Foundation.Tests.Sync
{
    public sealed class SyncStateQueueTests
    {
        public sealed class TheStartNextQueuedStateMethod
        {
            private readonly SyncStateQueue queue = new SyncStateQueue();
            private readonly List<SyncState> dequeuedStates = new List<SyncState>();

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
                var state = queue.Dequeue();
                dequeuedStates.Add(state);
                return state;
            }

            private List<SyncState> callMethodUntilSleep()
            {
                var returnValues = new List<SyncState>();

                while (true)
                {
                    returnValues.Add(callMethod());

                    if (dequeuedStates.Last() == Sleep)
                        break;
                }

                return returnValues;
            }

            [Fact]
            public void StartsSleepIfNothingQueued()
            {
                callMethod();

                dequeuedStates.ShouldBeSameEventsAs(
                    Sleep
                );
            }

            [Fact]
            public void StartsPullIfOnlyPullQueued()
            {
                queueSyncs(Pull);

                callMethod();

                dequeuedStates.ShouldBeSameEventsAs(
                    Pull
                );
            }

            [Fact]
            public void StartsPushIfOnlyPushQueued()
            {
                queueSyncs(Push);

                callMethod();

                dequeuedStates.ShouldBeSameEventsAs(
                    Push
                );
            }

            [Property]
            public void AlwaysReturnsTheStateItRuns(bool[] pushPull)
            {
                dequeuedStates.Clear();
                var states = (pushPull ?? Enumerable.Empty<bool>())
                    .Select(b => b ? Push : Pull).ToArray();
                queueSyncs(states);

                var returnValues = callMethodUntilSleep();

                returnValues.ShouldBeSameEventsAs(dequeuedStates.ToArray());
            }

            [Property]
            public void RunsFullCycleOnceNoMatterWhatIsQueuedIfPullIsQueued(bool[] pushPull)
            {
                dequeuedStates.Clear();
                var states = (pushPull ?? Enumerable.Empty<bool>())
                    .Select(b => b ? Push : Pull).ToArray();
                queueSyncs(states);
                if (!states.Contains(Pull))
                    queueSyncs(Pull);

                callMethodUntilSleep();

                dequeuedStates.ShouldBeSameEventsAs(
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

                dequeuedStates.ShouldBeSameEventsAs(
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

                dequeuedStates.ShouldBeSameEventsAs(
                    Pull, Push, Pull, Push, Sleep
                );
            }
        }
    }
}
