namespace Toggl.Foundation.Sync
{
    public interface ISyncStateQueue
    {
        void QueuePushSync();
        void QueuePullSync();

        SyncState StartNextQueuedState(IStateMachineOrchestrator orchestrator);
    }
}
