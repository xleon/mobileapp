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
                return startPush(orchestrator);
            
            if (pullSyncQueued)
                return startPull(orchestrator);
           
            if (pushSyncQueued)
                return startPush(orchestrator);
            
            return startSleep(orchestrator);
        }

        private SyncState startPull(IStateMachineOrchestrator orchestrator)
        {
            pullSyncQueued = false;
            pulledLast = true;
            orchestrator.StartPullSync();
            return Pull;
        }

        private SyncState startPush(IStateMachineOrchestrator orchestrator)
        {
            pushSyncQueued = false;
            pulledLast = false;
            orchestrator.StartPushSync();
            return Push;
        }

        private static SyncState startSleep(IStateMachineOrchestrator orchestrator)
        {
            orchestrator.GoToSleep();
            return Sleep;
        }
    }
}
