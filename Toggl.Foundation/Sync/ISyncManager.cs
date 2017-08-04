using System;

namespace Toggl.Foundation.Sync
{
    public interface ISyncManager
    {
        SyncState State { get; }
        IObservable<SyncState> StateObservable { get; }

        IObservable<SyncState> PushSync();
        IObservable<SyncState> ForceFullSync();
    }
}
