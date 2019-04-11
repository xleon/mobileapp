using System;

namespace Toggl.Core.Sync
{
    public interface ISyncManager
    {
        SyncState State { get; }
        IObservable<SyncProgress> ProgressObservable { get; }
        IObservable<Exception> Errors { get; }
        bool IsRunningSync { get; }

        IObservable<SyncState> PushSync();
        IObservable<SyncState> ForceFullSync();
        IObservable<SyncState> CleanUp();

        IObservable<SyncState> Freeze();
    }
}
