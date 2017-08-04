using static Toggl.Foundation.Sync.SyncState;

namespace Toggl.Foundation.Sync
{
    internal sealed class SyncStateQueue : ISyncStateQueue
    {
        private bool pulledLast;
        private bool pullSyncQueued;
        private bool pushSyncQueued;
        
        public void QueuePushSync()
        {
            pushSyncQueued = true;
        }

        public void QueuePullSync()
        {
            pullSyncQueued = true;
        }

        public SyncState StartNextQueuedState(IStateMachineOrchestrator orchestrator)
        {
            if (pulledLast)
            {
                startPush(orchestrator);
                return Push;
            }
            if (pullSyncQueued)
            {
                startPull(orchestrator);
                return Pull;
            }
            if (pushSyncQueued)
            {
                startPush(orchestrator);
                return Push;
            }
            startSleep(orchestrator);
            return Sleep;
        }

        private void startPull(IStateMachineOrchestrator orchestrator)
        {
            pullSyncQueued = false;
            pulledLast = true;
            orchestrator.StartPullSync();
        }

        private void startPush(IStateMachineOrchestrator orchestrator)
        {
            pushSyncQueued = false;
            pulledLast = false;
            orchestrator.StartPushSync();
        }

        private static void startSleep(IStateMachineOrchestrator orchestrator)
        {
            orchestrator.GoToSleep();
        }
    }
}
